namespace HaversineAlgo;

public class AmenityDistanceCalculator
{

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


}
