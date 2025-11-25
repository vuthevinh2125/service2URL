// ShortenUrl/Repositories/UrlRepository.cs
using Microsoft.EntityFrameworkCore;
using ShortenUrl.Data;
using ShortenUrl.Interfaces;
using ShortenUrl.Models;

namespace ShortenUrl.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly ShortenerDbContext _context;

        public UrlRepository(ShortenerDbContext context)
        {
            _context = context;
        }

        public async Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode) =>
            await _context.ShortenedUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

        public async Task<ShortenedUrl> AddAsync(ShortenedUrl url)
        {
            _context.ShortenedUrls.Add(url);
            await _context.SaveChangesAsync();
            return url;
        }

        public async Task<ShortenedUrl?> UpdateAccessAsync(string shortCode)
        {
            var existing = await _context.ShortenedUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (existing == null) return null;

            existing.HitCount++;
            existing.LastAccessed = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return existing;
        }
    }
}