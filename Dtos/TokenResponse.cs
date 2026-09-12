namespace EcommerceServer.Dtos
{
    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        //public DateTime Expires { get; set; }
        //public GetUser User { get; set; } = new GetUser();
    }
}
