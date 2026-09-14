using System.Text;
using CloudinaryDotNet;
using EcommerceServer.Data;
using EcommerceServer.Interfaces;
using EcommerceServer.Middlewares;
using EcommerceServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Bind Cloudinary settings from secrets/env vars
var cloudinarySection = builder.Configuration.GetSection("Cloudinary");
var account = new Account(
    cloudinarySection["CloudName"],
    cloudinarySection["ApiKey"],
    cloudinarySection["ApiSecret"]
);

// Auth global middleware to check expiry of token and send error message and status code to client so that axios interceptor can redirect to login page
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero // no grace period, expires immediately
        };

        // Customize error responses
        options.Events = new JwtBearerEvents

        { // To bypass for endpoints having AlloyAnonymous
            OnMessageReceived = ctx =>
            {
                var endpoint = ctx.HttpContext.GetEndpoint();
                var allowAnon = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>();
                if (allowAnon != null)
                {
                    ctx.NoResult(); // skip validation
                }
                return Task.CompletedTask;
            },
            // checks for failed authentication
            OnAuthenticationFailed = async ctx =>
            {
                ctx.Response.ContentType = "application/json";
                if (ctx.Exception is SecurityTokenExpiredException)
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await ctx.Response.WriteAsJsonAsync(new { error = "Token expired" });
                }
                else
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await ctx.Response.WriteAsJsonAsync(new { error = "Invalid JWT Token" });
                }
                await ctx.Response.CompleteAsync();// flush and close response
            },
            OnChallenge = async ctx =>
            {
                // Prevent default HTML response sent by browser and sends json to client so axios can intercept and redirect
                ctx.HandleResponse();
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                ctx.Response.ContentType = "application/json";
                await ctx.Response.WriteAsJsonAsync(new { error = "No token provided" });
            }
        };
    });

builder.Services.AddAuthorization();

// Register Cloudinary
builder.Services.AddSingleton(new Cloudinary(account));

// Register your services
builder.Services.AddScoped<IProducts, Products>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVisitorTracker,VisitorTrackerService>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
        policy.WithOrigins(
            "http://localhost:3000",
            "https://fashion-react-client.vercel.app"
            )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("AllowClient");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
