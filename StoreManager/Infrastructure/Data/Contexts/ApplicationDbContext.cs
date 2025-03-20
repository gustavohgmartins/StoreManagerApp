using Microsoft.EntityFrameworkCore;
using StoreManager.Core.Entities;

namespace StoreManager.Infrastructure.Data.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<ProductStore> ProductStore { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public ApplicationDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductStore>()
                .HasKey(ps => new { ps.ProductId, ps.StoreId });
        }
    }
}
