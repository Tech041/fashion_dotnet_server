
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
using EcommerceServer.Helpers;

namespace EcommerceServer.Services
{
    public class UserService(AppDbContext context, IConfiguration config) : IUserService
    {
      

        public async Task<GetUser> CreateAsync(CreateUser user)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var newUser = new User
            {
                
                Name = user.Name,
                Email = user.Email,
                
            };

            var passwordHasher = new PasswordHasher<User>();
            newUser.PasswordHash = passwordHasher.HashPassword(newUser, user.Password);
            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            return new GetUser
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Email = newUser.Email,
            };
        }

        public async Task<TokenResponse> LoginUserAsync(Login login)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Invalid credentials.");
            }

            
            var tokenGenerator = new GenerateToken(config);
            var token = await tokenGenerator.CreateToken(new CreateUserToken
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            });
           

            return new TokenResponse
            {
                Token = token,
                Message = "Login successful.",
                Status = "Success"
                //Expires = token.ValidTo,
                //User = new GetUser { Id = user.Id, Name = user.Name, Email = user.Email }
            };
        }
    }
}

