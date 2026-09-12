namespace EcommerceServer.Dtos
{
    public class UploadProduct
    {
        public string Name { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public string Collection { get; set; } = String.Empty;
        public string Description { get; set; }= String.Empty;
        public List<string> Sizes { get; set; } = new();
        public string ProductSlug { get; set; } = string.Empty;
        public IFormFile Image { get; set; } = null!;


    }
}
