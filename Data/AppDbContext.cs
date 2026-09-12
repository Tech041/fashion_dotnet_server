using EcommerceServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceServer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Visitor> Visitors { get; set; } = null!;

    }
}
