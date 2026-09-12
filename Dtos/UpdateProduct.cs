namespace EcommerceServer.Dtos
{
    public class UpdateProduct
    {
        
        public string name { get; set; } = String.Empty;
        public decimal price { get; set; }
        public string collection { get; set; } = String.Empty;
        public string description { get; set; } = String.Empty;
       
      
        
    }
}
