using Microsoft.AspNetCore.Mvc;
using ShortenUrl.Domain.Services;

namespace ShortenUrl.API.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class ShortenUrlController(IShortenUrlService service) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ShortenUrlAsync([FromBody] string originalUrl)
    {
        var shortUrl = await service.ShortenAsync(originalUrl);
        return Created(string.Empty, new { ShortUrl = shortUrl });
    }

    [HttpGet("{shortCode}")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RedirectToOriginalUrlAsync(string shortCode)
    {
        var originalUrl = await service.GetOriginalUrlAsync(shortCode);

        if (string.IsNullOrEmpty(originalUrl))
            return NotFound("URL not found");
        
        return Redirect(originalUrl);
    }
}