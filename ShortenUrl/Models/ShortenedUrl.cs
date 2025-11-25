// ShortenUrl/Models/ShortenedUrl.cs
using System.ComponentModel.DataAnnotations;

namespace ShortenUrl.Models
{
    public class ShortenedUrl
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string ShortCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(2048)]
        public string OriginalUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastAccessed { get; set; }
        public int HitCount { get; set; } = 0;
    }
}