using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.Maps.v3;
using Google.Apis.Maps.v3.Data;

class Program
{
    static async Task Main(string[] args)
    {
        // Example user input
        var userAmenities = new List<string> { "gym", "nursery", "park", "pub" };
        double userBudget = 500000; // Budget in your currency
        string userHouseType = "apartment";
        string userPreferredArea = "Downtown";
        var userOfficeLocation = new Location(40.758896, -73.985130); // Example office location (Times Square)

        // Real estate houses
        var houses = new List<House>
        {
            new House { Id = 1, Price = 450000, Type = "apartment", Latitude = 40.7128, Longitude = -74.0060, Area = "Downtown" },
            new House { Id = 2, Price = 550000, Type = "villa", Latitude = 40.730610, Longitude = -73.935242, Area = "Uptown" },
            new House { Id = 3, Price = 490000, Type = "apartment", Latitude = 40.706192, Longitude = -74.009160, Area = "Downtown" }
        };

        // Google Maps API Key
        string apiKey = "YOUR_GOOGLE_MAPS_API_KEY";

        // Find best matching houses
        var matchingHouses = await FindBestHouses(houses, userAmenities, userBudget, userHouseType, userPreferredArea, userOfficeLocation, apiKey);

        // Display results
        foreach (var house in matchingHouses)
        {
            Console.WriteLine($"House ID: {house.Id}, Price: {house.Price}, Type: {house.Type}, Total Distance Score: {house.DistanceScore}");
            Console.WriteLine($"  - Distance to Office: {house.DistanceToOffice:F2} km");
            foreach (var amenity in house.AmenityDistances)
            {
                Console.WriteLine($"  - {amenity.Key}: {amenity.Value:F2} km");
            }
        }
    }

    static async Task<List<House>> FindBestHouses(
        List<House> houses,
        List<string> amenities,
        double budget,
        string houseType,
        string preferredArea,
        Location officeLocation,
        string apiKey)
    {
        var matchingHouses = new List<House>();

        foreach (var house in houses)
        {
            if (house.Price > budget || house.Type != houseType || house.Area != preferredArea)
                continue;

            house.AmenityDistances = new Dictionary<string, double>();
            double distanceScore = 0;

            // Calculate distance to office
            house.DistanceToOffice = HaversineDistance(house.Latitude, house.Longitude, officeLocation.Lat, officeLocation.Lng);
            distanceScore += house.DistanceToOffice;

            // Calculate aggregate distance to all amenities
            foreach (var amenity in amenities)
            {
                var distance = await GetDistanceToAmenity(house.Latitude, house.Longitude, amenity, apiKey);
                if (distance == double.MaxValue) // If amenity not found
                {
                    distanceScore = double.MaxValue;
                    break;
                }

                house.AmenityDistances[amenity] = distance; // Store distance
                distanceScore += distance;
            }

            if (distanceScore != double.MaxValue)
            {
                house.DistanceScore = distanceScore;
                matchingHouses.Add(house);
            }
        }

        // Sort by distance score (lower is better)
        return matchingHouses.OrderBy(h => h.DistanceScore).ToList();
    }

    static async Task<double> GetDistanceToAmenity(double latitude, double longitude, string amenity, string apiKey)
    {
        try
        {
            var client = new MapsService(new BaseClientService.Initializer
            {
                ApiKey = apiKey
            });

            var request = client.Places.NearbySearch(new NearbySearchRequest
            {
                Location = new Location(latitude, longitude),
                Radius = 2000, // Search within 2km
                Keyword = amenity
            });

            var response = await request.ExecuteAsync();
            if (response.Results.Count > 0)
            {
                // Calculate distance to the closest amenity
                var closestAmenity = response.Results.First();
                return HaversineDistance(latitude, longitude, closestAmenity.Geometry.Location.Lat, closestAmenity.Geometry.Location.Lng);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching amenity '{amenity}': {ex.Message}");
        }

        return double.MaxValue; // Return max value if no amenity found
    }

    static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371; // Earth's radius in km
        double dLat = (lat2 - lat1) * Math.PI / 180;
        double dLon = (lon2 - lon1) * Math.PI / 180;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // Distance in km
    }
}

// House class
class House
{
    public int Id { get; set; }
    public double Price { get; set; }
    public string Type { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Area { get; set; }
    public double DistanceScore { get; set; } = double.MaxValue;
    public double DistanceToOffice { get; set; }
    public Dictionary<string, double> AmenityDistances { get; set; } = new();
}

// Location class for office coordinates
class Location
{
    public double Lat { get; set; }
    public double Lng { get; set; }

    public Location(double lat, double lng)
    {
        Lat = lat;
        Lng = lng;
    }
}
