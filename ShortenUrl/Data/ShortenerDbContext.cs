// ShortenUrl/Data/ShortenerDbContext.cs
using Microsoft.EntityFrameworkCore;
using ShortenUrl.Models;

namespace ShortenUrl.Data
{
    public class ShortenerDbContext : DbContext
    {
        public ShortenerDbContext(DbContextOptions<ShortenerDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenedUrl>()
                .HasIndex(u => u.ShortCode)
                .IsUnique();
        }
    }
}