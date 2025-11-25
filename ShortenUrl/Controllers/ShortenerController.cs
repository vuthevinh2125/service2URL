// ShortenUrl/Controllers/ShortenerController.cs
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc;
using ShortenUrl.Interfaces;
using ShortenUrl.Models;
using ShortenUrl.Services;

namespace ShortenUrl.Controllers 
{
    public record CreateUrlDto([System.ComponentModel.DataAnnotations.Required] string OriginalUrl);

    [ApiController]
    [Route("api/[controller]")]
    public class ShortenerController : ControllerBase
    {
        private readonly IUrlRepository _repository;
        private readonly ShortCodeGenerator _codeGenerator;
        private readonly ILogger<ShortenerController> _logger;

        public ShortenerController(IUrlRepository repository, ShortCodeGenerator codeGenerator, ILogger<ShortenerController> logger)
        {
            _repository = repository;
            _codeGenerator = codeGenerator;
            _logger = logger;
        }

        // POST api/shortener/shorten: T?o URL ng?n (CREATE)
        [HttpPost("shorten")]
        public async Task<ActionResult<ShortUrlResponseDto>> CreateShortUrl([FromBody] CreateUrlDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Validation URL
            if (!Uri.TryCreate(dto.OriginalUrl, UriKind.Absolute, out var uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                return BadRequest(new { error = "Invalid URL format or scheme." });
            }

            ShortenedUrl? createdUrl = null;
            for (int i = 0; i < 5; i++) 
            {
                var shortCode = _codeGenerator.GenerateUniqueCode();
                var existingUrl = await _repository.GetByShortCodeAsync(shortCode);

                if (existingUrl == null)
                {
                    var newUrl = new ShortenedUrl
                    {
                        OriginalUrl = dto.OriginalUrl,
                        ShortCode = shortCode
                    };
                    createdUrl = await _repository.AddAsync(newUrl);
                    break;
                }
            }

            if (createdUrl == null)
            {
                _logger.LogError("Failed to generate unique short code after 5 attempts.");
                return StatusCode(500, new { error = "Could not generate a unique short code." });
            }


            // Đoạn này chủ yếu hiểu logic à gán thêm 1 link cố định có cổng port 7575 ( để mix vào random code cho ra link hoàn chinhr)
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var shortLink = $"{baseUrl}/code/{createdUrl.ShortCode}";

            var responseDto = new ShortUrlResponseDto
            {
                Id = createdUrl.Id,
                ShortCode = createdUrl.ShortCode,
                OriginalUrl = createdUrl.OriginalUrl,
                CreatedAt = createdUrl.CreatedAt,
                LastAccessed = createdUrl.LastAccessed,
                HitCount = createdUrl.HitCount,
                ShortLink = shortLink
            };

            return CreatedAtAction(nameof(RedirectToUrl),
                                   new { code = createdUrl.ShortCode },
                                   responseDto);
        }

        [HttpGet("/code/{code}")] // <-- Moi: Route so là: https://localhost:7575/code/0kMr26
        public async Task<IActionResult> RedirectToUrl(string code)
        {
            var urlEntry = await _repository.GetByShortCodeAsync(code);

            if (urlEntry == null)
            {
                return NotFound(new { error = $"ShortCode '{code}' not found." });
            }

            // Cap nhat HitCount
            await _repository.UpdateAccessAsync(code);

            // Tra ve` HTTP 302 Found
            return Redirect(urlEntry.OriginalUrl);
        }
    }
}