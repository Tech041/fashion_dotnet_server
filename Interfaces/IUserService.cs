using EcommerceServer.Dtos;

namespace EcommerceServer.Interfaces
{
    public interface IUserService
    {
        Task<GetUser> CreateAsync(CreateUser user);
        Task<TokenResponse> LoginUserAsync(Login login);
    }
}
