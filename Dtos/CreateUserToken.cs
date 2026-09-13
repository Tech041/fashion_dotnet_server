namespace EcommerceServer.Dtos
{
    public class CreateUserToken
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
    }
}
