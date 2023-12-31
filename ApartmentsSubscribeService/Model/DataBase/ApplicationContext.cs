﻿using ApartmentsSubscribeService.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ApartmentsSubscribeService.Model.DataBase
{
    public class ApplicationContext : DbContext
    {
        private readonly ILogger _logger;
        public ApplicationContext(ILogger logger)
        {
            _logger = logger;
        }
        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Apartment> Apartments { get; set; } = null!;

        public ApplicationContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=apartments_subscribe_service.db");
            if (_logger != null)
            {
                optionsBuilder.LogTo(message => _logger.LogInformation(message), LogLevel.Information);
            }
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
