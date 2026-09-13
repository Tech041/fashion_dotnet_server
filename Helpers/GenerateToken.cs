using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceServer.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceServer.Helpers
{
    public class GenerateToken
    {

        private readonly IConfiguration _config;

        public GenerateToken(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> CreateToken(CreateUserToken user)
        {

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var keyString = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                throw new InvalidOperationException("JWT configuration is missing.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresMinutesString = _config["Jwt:ExpiresMinutes"] ?? "120";
            if (!double.TryParse(expiresMinutesString, out var expiresMinutes))
            {
                expiresMinutes = 120;
            }

            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;

        }
    }

}
