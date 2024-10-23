using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FoodHub.Models;

namespace FoodHub.DAL
{
    public class ItemDbContext : IdentityDbContext
    {
        public ItemDbContext(DbContextOptions<ItemDbContext> options) : base(options)
        {

        }

        public DbSet<Item> Items { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<Allergen> Allergens { get; set; }
        public DbSet<ItemAllergen> ItemAllergens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
                optionsBuilder.UseLazyLoadingProxies();
        }
    }
}