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
    public Dictionary<string, double> AmenityDistances { get; set; } = new();
}
