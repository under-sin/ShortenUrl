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
    public async Task<IActionResult> ShortenUrl([FromBody] string originalUrl)
    {
        var shortUrl = await service.Shorten(originalUrl);
        return Created(string.Empty, new { ShortUrl = shortUrl });
    }

    [HttpGet("{shortCode}")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RedirectToOriginalUrl(string shortCode)
    {
        var originalUrl = await service.GetOriginalUrl(shortCode);

        if (string.IsNullOrEmpty(originalUrl))
            return NotFound("URL not found");
        
        return Redirect(originalUrl);
    }
}