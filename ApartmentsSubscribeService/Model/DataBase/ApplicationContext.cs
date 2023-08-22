using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ApartmentsSubscribeService.Model.DataBase
{
    public class ApplicationContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Apartment> Apartments { get; set; } = null!;

        public ApplicationContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        public ApplicationContext(ILoggerFactory loggerFactory) {
            _loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
            optionsBuilder.UseSqlite("Data Source=apartments_subscribe_service.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasAlternateKey(k => k.Email);
            modelBuilder.Entity<Apartment>().HasAlternateKey(a => a.Url);
            modelBuilder
                .Entity<Apartment>()
                .HasMany(a => a.Users)
                .WithMany(u => u.Apartments)
                .UsingEntity<UsersApartments>(
                    j => j
                        .HasOne(ju => ju.User)
                        .WithMany(jt => jt.UsersApartments)
                        .HasForeignKey(ju => ju.UserId),
                    j => j
                        .HasOne(ja => ja.Apartment)
                        .WithMany(jt => jt.UsersApartments)
                        .HasForeignKey(ja => ja.ApartmentId),
                    j =>
                    {
                        j.HasKey(t => new { t.UserId, t.ApartmentId });
                        j.ToTable("users_apartments");
                    });
        }
    }
}
