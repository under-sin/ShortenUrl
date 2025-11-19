using Microsoft.AspNetCore.Mvc;
using ShortenUrl.API.Requests;
using ShortenUrl.Domain.Services;

namespace ShortenUrl.API.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class ShortenUrlController(IShortenUrlService service) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ShortenUrlAsync([FromBody] ShortenRequest request, CancellationToken ct)
    {
        var shortUrl = await service.ShortenAsync(request.OriginalUrl, ct);
        return Created(string.Empty, new { ShortUrl = shortUrl });
    }

    [HttpGet("{shortCode}")]
    [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RedirectToOriginalUrlAsync(string shortCode, CancellationToken ct)
    {
        var originalUrl = await service.GetOriginalUrlAsync(shortCode, ct);

        if (string.IsNullOrEmpty(originalUrl))
            return NotFound("URL not found");
        
        return RedirectPermanent(originalUrl);
    }
}