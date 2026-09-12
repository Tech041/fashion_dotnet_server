
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using EcommerceServer.Data;
using EcommerceServer.Dtos;
using EcommerceServer.Entities;
using EcommerceServer.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceServer.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UserService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<GetUser> CreateAsync(CreateUser user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var newUser = new Entities.User
            {
                Id = Guid.NewGuid().ToString(),
                Name = user.Name,
                Email = user.Email,
                
            };

            var passwordHasher = new PasswordHasher<Entities.User>();
            newUser.PasswordHash = passwordHasher.HashPassword(newUser, user.Password);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new GetUser
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Email = newUser.Email,
            };
        }

        public async Task<TokenResponse> LoginUserAsync(Login login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var passwordHasher = new PasswordHasher<Entities.User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Invalid credentials.");
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var keyString = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                throw new InvalidOperationException("JWT configuration is missing. Set Jwt:Key in configuration.");
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

            return new TokenResponse
            {
                Token = tokenString,
                Message = "Login successful.",
                Status = "Success"
                //Expires = token.ValidTo,
                //User = new GetUser { Id = user.Id, Name = user.Name, Email = user.Email }
            };
        }
    }
}

