using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaSite_backend.Models.Entities;
using MediaSite_backend.Data;
using MediaSite_backend.Models.Dtos.NewsletterSubscriber;

[Route("api/[controller]")]
[ApiController]
public class NewsletterSubscribersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public NewsletterSubscribersController(ApplicationDbContext context)
    {
        _context = context;
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
        var newsletterSubscribers = await _context.NewsletterSubscribers.ToListAsync();

        if (newsletterSubscribers.Any(n => n.Email.Contains(newsletterSubscriberDto.Email)))
        {
            return BadRequest();
        }

        var newsletterSubscriber = new NewsletterSubscriber();

        newsletterSubscriber.Email = newsletterSubscriberDto.Email;

        _context.NewsletterSubscribers.Add(newsletterSubscriber);
        await _context.SaveChangesAsync();

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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStatus(Guid id, bool isActive)
    {
        var subscriber = await _context.NewsletterSubscribers
            .FindAsync(id);

        if (subscriber == null)
            return NotFound();

        subscriber.IsActive = isActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id:guid}")]
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
