using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OrderFoodApplication.Models;
using System.Reflection.Emit;


namespace OrderFoodApplication.ContextDBConfig
{
    public class TastyBitesDBContext:IdentityDbContext<ApplicationUser>
    {
        public TastyBitesDBContext(DbContextOptions<TastyBitesDBContext> options): base(options) 
        {
            
        }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Order>().HasKey(o => o.Id);
        }
        

    }
}
