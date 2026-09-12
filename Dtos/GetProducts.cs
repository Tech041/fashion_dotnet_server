public class GetProducts
{
    public string _id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string slug { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public decimal price { get; set; }
    public List<string> sizes { get; set; } = new();
    public string collection { get; set; } = string.Empty;
    public string image { get; set; } = string.Empty;
    public string publicId { get; set; } = string.Empty;
}
