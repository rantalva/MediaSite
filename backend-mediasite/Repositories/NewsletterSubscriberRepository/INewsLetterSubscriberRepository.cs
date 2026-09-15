using MediaSite_backend.Models.Dtos.Category;
using MediaSite_backend.Models.Dtos.NewsletterSubscriber;
using MediaSite_backend.Models.Entities;

namespace MediaSite_backend.Repositories.NewsletterSubscriberRepository
{
    public interface INewsLetterSubscriberRepository
    {
        Task<IEnumerable<NewsletterSubscriber>> GetNewsletterSubscribersAsync();
        Task<NewsletterSubscriber> GetNewsletterSubscriberIdAsync(Guid id);
        Task<NewsletterSubscriber> EditNewsletterSubscriberEmailAsync(Guid id, NewsletterSubscriberDto newsletterSubscriberDto);
        Task<NewsletterSubscriber> EditNewsletterSubscriberStatusAsync(Guid id, NewsletterSubscriberStatusDto newsletterSubscriberDto);
        Task<NewsletterSubscriber> AddNewsletterSubscriberAsync(NewsletterSubscriberDto newsletterSubscriberDto);
        Task<bool> DeleteNewsletterSubscriberAsync(Guid id);
    }
}
