// ShortenUrl/Models/ShortUrlResponseDto.cs

namespace ShortenUrl.Models
{
    public class ShortUrlResponseDto
    {
        public int Id { get; set; }
        public string ShortCode { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;

        public string ShortLink { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? LastAccessed { get; set; }
        public int HitCount { get; set; }
    }
}