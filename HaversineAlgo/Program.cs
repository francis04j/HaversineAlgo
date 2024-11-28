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
        var matchingHouses = await FindBestHouses(houses, userAmenities, userBudget, userHouseType, userPreferredArea, apiKey);

        // Display results
        foreach (var house in matchingHouses)
        {
            Console.WriteLine($"House ID: {house.Id}, Price: {house.Price}, Type: {house.Type}, Total Distance Score: {house.DistanceScore}");
            foreach (var amenity in house.AmenityDistances)
            {
                Console.WriteLine($"  - {amenity.Key}: {amenity.Value:F2} km");
            }
        }
    }
}

    

    