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
    private readonly ApplicationDbContext _context;
    private readonly INewsLetterSubscriberRepository _newsLetterSubscriberRepository;
    public NewsletterSubscribersController(ApplicationDbContext context, INewsLetterSubscriberRepository newsLetterSubscriberRepository)
    {
        _context = context;
        _newsLetterSubscriberRepository = newsLetterSubscriberRepository;
    }

    // GET: api/NewsletterSubscriber
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewsletterSubscriber>>> GetNewsletterSubscriber()
    {
        return await _context.NewsletterSubscribers.ToListAsync();
    }

    // GET: api/NewsletterSubscriber/5
    [HttpGet("{id}")]
    public async Task<ActionResult<NewsletterSubscriber>> GetNewsletterSubscriber(System.Guid id)
    {
        var newslettersubscriber = await _context.NewsletterSubscribers.FindAsync(id);

        if (newslettersubscriber == null)
        {
            return NotFound();
        }

        return newslettersubscriber;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutNewsletterSubscriber(Guid? id, NewsletterSubscriber newslettersubscriber)
    {
        if (id != newslettersubscriber.Id)
        {
            return BadRequest();
        }

        _context.Entry(newslettersubscriber).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!NewsletterSubscriberExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/NewsletterSubscriber
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

    // DELETE: api/NewsletterSubscriber/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNewsletterSubscriber(System.Guid? id)
    {
        var newslettersubscriber = await _context.NewsletterSubscribers.FindAsync(id);
        if (newslettersubscriber == null)
        {
            return NotFound();
        }

        _context.NewsletterSubscribers.Remove(newslettersubscriber);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] NewsletterSubscriberStatusDto newsletterSubscriberStatusDto)
    {
        var subscriber = await _context.NewsletterSubscribers
            .FindAsync(id);

        if (subscriber == null)
            return NotFound();

        subscriber.Status = newsletterSubscriberStatusDto.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id:guid}/email")]
    public async Task<IActionResult> UpdateEmail(Guid id, [FromBody] NewsletterSubscriberDto newsletterSubscriberDto)
    {
        var subscriber = await _context.NewsletterSubscribers
            .FindAsync(id);

        if (subscriber == null)
            return NotFound();

        subscriber.Email = newsletterSubscriberDto.Email;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool NewsletterSubscriberExists(System.Guid? id)
    {
        return _context.NewsletterSubscribers.Any(e => e.Id == id);
    }
}
