using AhorraYa.Entities;
using AhorraYa.Entities.MicrosoftIdentity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AhorraYa.DataAccess
{
    public class DbDataAccess : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public DbDataAccess(DbContextOptions<DbDataAccess> options) : base(options) { }
        public DbDataAccess() { }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<MeasurementUnit> MeasurementUnits { get; set; }
        public virtual DbSet<PriceOfShop> PriceOfShops { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<Country> Countries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.LogTo(Console.WriteLine).EnableDetailedErrors();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
