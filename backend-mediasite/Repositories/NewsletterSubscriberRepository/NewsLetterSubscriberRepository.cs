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

        public Task<bool> DeleteNewsletterSubscriberAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<NewsletterSubscriber> EditNewsletterSubscriberAsync(Guid id, NewsletterSubscriberDto newsletterSubscriberDto)
        {
            throw new NotImplementedException();
        }

        public Task<NewsletterSubscriber> GetNewsletterSubscriberIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<NewsletterSubscriberDto>> GetNewsletterSubscribersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
