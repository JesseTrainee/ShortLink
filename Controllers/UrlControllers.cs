using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShortLink.Data;
using ShortLink.Models;

namespace ShortLink.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpPost("shorten")]
    public async Task<IActionResult> Shorten([FromBody] ShortenRequest request)
    {
        var shortCode = GenerateShortCode();

        var entry = new UrlEntry
        {
            ShortCode = shortCode,
            OriginalUrl = request.OriginalUrl,
            CreatedAt = DateTime.UtcNow
        };

        _context.UrlEntries.Add(entry);
        await _context.SaveChangesAsync();

        return Ok(new {shortCode, shortUrl = $"http://localhost:5089/{shortCode}"});
    }

    [HttpGet("/{code}")]
    public async Task<IActionResult> RedirectToUrl(string code)
    {
        var entry = await _context.UrlEntries
            .FirstOrDefaultAsync(u => u.ShortCode == code);

        if (entry is null)
            return NotFound();

        return Redirect(entry.OriginalUrl);
    }

    private static string GenerateShortCode()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Range(0, 6)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }
}