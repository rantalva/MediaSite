using MediaSite_backend.Models.Dtos.Category;
using MediaSite_backend.Models.Dtos.NewsletterSubscriber;
using MediaSite_backend.Models.Entities;

namespace MediaSite_backend.Repositories.NewsletterSubscriberRepository
{
    public interface INewsLetterSubscriberRepository
    {
        Task<IEnumerable<NewsletterSubscriberDto>> GetNewsletterSubscribersAsync();
        Task<NewsletterSubscriber> GetNewsletterSubscriberIdAsync(Guid id);
        Task<NewsletterSubscriber> EditNewsletterSubscriberAsync(Guid id, NewsletterSubscriberDto newsletterSubscriberDto);
        Task<NewsletterSubscriber> AddNewsletterSubscriberAsync(NewsletterSubscriberDto newsletterSubscriberDto);
        Task<bool> DeleteNewsletterSubscriberAsync(Guid id);
    }
}
