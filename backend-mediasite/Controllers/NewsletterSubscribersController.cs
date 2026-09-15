using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaSite_backend.Models.Entities;
using MediaSite_backend.Data;
using MediaSite_backend.Models.Dtos.NewsletterSubscriber;
using MediaSite_backend.Repositories.NewsletterSubscriberRepository;

[Route("api/[controller]")]
[ApiController]
public class NewsletterSubscribersController : ControllerBase
{
    private readonly INewsLetterSubscriberRepository _newsLetterSubscriberRepository;
    public NewsletterSubscribersController(INewsLetterSubscriberRepository newsLetterSubscriberRepository)
    {
        _newsLetterSubscriberRepository = newsLetterSubscriberRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewsletterSubscriber>>> GetNewsletterSubscriber()
    {
        var newsletterSubscribers = await _newsLetterSubscriberRepository.GetNewsletterSubscribersAsync();

        if (newsletterSubscribers == null) 
        {
            return BadRequest();
        }

        return Ok(newsletterSubscribers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NewsletterSubscriber>> GetNewsletterSubscriber(Guid id)
    {
        var newslettersubscriber = await _newsLetterSubscriberRepository.GetNewsletterSubscriberIdAsync(id);

        if (newslettersubscriber == null)
        {
            return NotFound();
        }

        return newslettersubscriber;
    }
    [HttpPost]
    public async Task<ActionResult<NewsletterSubscriberDto>> PostNewsletterSubscriber([FromBody]NewsletterSubscriberDto newsletterSubscriberDto)
    {
        var newsletterSubscriber = await _newsLetterSubscriberRepository.AddNewsletterSubscriberAsync(newsletterSubscriberDto);

        if (newsletterSubscriber == null)
        {
            return BadRequest();
        }

        return CreatedAtAction("GetNewsletterSubscriber", new { id = newsletterSubscriber.Id }, newsletterSubscriber);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNewsletterSubscriber(Guid id)
    {
        var deleted = await _newsLetterSubscriberRepository.DeleteNewsletterSubscriberAsync(id);

        if (!deleted)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] NewsletterSubscriberStatusDto newsletterSubscriberStatusDto)
    {
        var subscriber = await _newsLetterSubscriberRepository.EditNewsletterSubscriberStatusAsync(id, newsletterSubscriberStatusDto);

        if (subscriber == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id:guid}/email")]
    public async Task<IActionResult> UpdateEmail(Guid id, [FromBody] NewsletterSubscriberDto newsletterSubscriberDto)
    {
        var subscriber = await _newsLetterSubscriberRepository.EditNewsletterSubscriberEmailAsync(id, newsletterSubscriberDto);

        if (subscriber == null)
        {
            return NotFound();
        }

        return NoContent();
    }
}
