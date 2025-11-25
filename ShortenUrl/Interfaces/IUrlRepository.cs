// ShortenUrl/Interfaces/IUrlRepository.cs
using ShortenUrl.Models;

namespace ShortenUrl.Interfaces
{
    public interface IUrlRepository
    {
        Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode);
        Task<ShortenedUrl> AddAsync(ShortenedUrl url);
        Task<ShortenedUrl?> UpdateAccessAsync(string shortCode);
    }
}