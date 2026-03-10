using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Data
{
    public class NZWalksDbContext : DbContext
    {
        public NZWalksDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Walk> Walks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for Difficulties
            // Easey, Medium, Hard

            var difficulties = new List<Difficulty>()
            {
                new Difficulty()
                {
                    Id = Guid.Parse("a1b2c3d4-e5f6-7890-1234-56789abcdef0"),
                    Name = "Easy"
                },
                new Difficulty()
                {
                    Id = Guid.Parse("b1c2d3e4-f5a6-7890-1234-56789abcdef0"),
                    Name = "Medium"
                },
                new Difficulty()
                {
                    Id = Guid.Parse("c1d2e3f4-a5b6-7890-1234-56789abcdef0"),
                    Name = "Hard"
                }
            };

            // Seed difficulties to the database
            modelBuilder.Entity<Difficulty>().HasData(difficulties);

            // Seed data for Regions
            var regions = new List<Region>
            {
                new Region()
                {
                    Id = Guid.Parse("d1e2f3a4-b5c6-7890-1234-56789abcdef0"),
                    Code = "AKL",
                    Name = "Auckland",
                    RegionImageUrl = "https://example.com/images/auckland.jpg"
                },
                new Region()
                {
                    Id = Guid.Parse("e1f2a3b4-c5d6-7890-1234-56789abcdef0"),
                    Code = "WLG",
                    Name = "Wellington",
                    RegionImageUrl = "https://example.com/images/wellington.jpg"
                },
                new Region()
                {
                    Id = Guid.Parse("f1a2b3c4-d5e6-7890-1234-56789abcdef0"),
                    Code = "CHC",
                    Name = "Christchurch",
                    RegionImageUrl = "https://example.com/images/christchurch.jpg"
                }
            };

            modelBuilder.Entity<Region>().HasData(regions);
        }
    }
}
