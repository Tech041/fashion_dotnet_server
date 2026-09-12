using CloudinaryDotNet;
using EcommerceServer.Data;
using EcommerceServer.Interfaces;
using EcommerceServer.Services;
using Microsoft.EntityFrameworkCore;

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

// Register Cloudinary
builder.Services.AddSingleton(new Cloudinary(account));

// Register your services
builder.Services.AddScoped<IProducts, Products>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVisitorTracker,VisitorTrackerService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
        policy.WithOrigins("http://localhost:3000")
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
app.UseAuthorization();

app.MapControllers();

app.Run();
