using MediaSite_backend.Data;
using MediaSite_backend.Models.Dtos.NewsletterSubscriber;
using MediaSite_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaSite_backend.Repositories.NewsletterSubscriberRepository
{

    public class NewsLetterSubscriberRepository : INewsLetterSubscriberRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public NewsLetterSubscriberRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<NewsletterSubscriber> AddNewsletterSubscriberAsync(NewsletterSubscriberDto newsletterSubscriberDto)
        {

            if (await _applicationDbContext.NewsletterSubscribers.AnyAsync(n => n.Email == newsletterSubscriberDto.Email)) 
            {
                return null;
            }

            var newSubscriber = new NewsletterSubscriber();

            newSubscriber.Email = newsletterSubscriberDto.Email;

            await _applicationDbContext.NewsletterSubscribers.AddAsync(newSubscriber);
            await _applicationDbContext.SaveChangesAsync();

            return newSubscriber;
        }

        public async Task<bool> DeleteNewsletterSubscriberAsync(Guid id)
        {
            var newsLetterSubscriber = await _applicationDbContext.NewsletterSubscribers.FindAsync(id);

            if (newsLetterSubscriber == null)
            {
                return false;
            }

            _applicationDbContext.NewsletterSubscribers.Remove(newsLetterSubscriber);
            await _applicationDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<NewsletterSubscriber> EditNewsletterSubscriberEmailAsync(Guid id, NewsletterSubscriberDto newsletterSubscriberDto)
        {
            var subscriber = await _applicationDbContext.NewsletterSubscribers.FindAsync(id);

            if (subscriber == null) 
            {
                return null;
            }

            subscriber.Email = newsletterSubscriberDto.Email;

            return subscriber;
        }

        public async Task<NewsletterSubscriber> EditNewsletterSubscriberStatusAsync(Guid id, NewsletterSubscriberStatusDto newsletterSubscriberDto)
        {
            var subscriber = await _applicationDbContext.NewsletterSubscribers.FindAsync(id);

            if (subscriber == null)
            {
                return null;
            }

            subscriber.Status = newsletterSubscriberDto.Status;

            await _applicationDbContext.SaveChangesAsync();

            return subscriber;
        }

        public async Task<NewsletterSubscriber> GetNewsletterSubscriberIdAsync(Guid id)
        {
            return await _applicationDbContext.NewsletterSubscribers.FindAsync(id);
        }

        public async Task<IEnumerable<NewsletterSubscriber>> GetNewsletterSubscribersAsync()
        {
            return await _applicationDbContext.NewsletterSubscribers.ToListAsync();
        }
    }
}
