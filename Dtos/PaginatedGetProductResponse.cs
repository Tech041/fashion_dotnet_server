namespace EcommerceServer.Dtos
{
    public class PaginatedGetProductResponse<T>
    {

        public bool success { get; set; }
        public List<T> products { get; set; } = new();
        public int total { get; set; }
        public int page { get; set; }
        public int pages { get; set; }
    }
}
