public class HouseSearch
{
    static async Task<List<House>> FindBestHouses(List<House> houses, List<string> amenities, double budget, string houseType, string preferredArea, string apiKey)
    {
        var matchingHouses = new List<House>();

        foreach (var house in houses)
        {
            if (house.Price > budget || house.Type != houseType || house.Area != preferredArea)
                continue;

            house.AmenityDistances = new Dictionary<string, double>();
            double distanceScore = 0;

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
}
