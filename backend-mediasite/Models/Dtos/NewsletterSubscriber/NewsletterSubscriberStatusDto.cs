using MediaSite_backend.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MediaSite_backend.Models.Dtos.NewsletterSubscriber
{
    public class NewsletterSubscriberStatusDto
    {
        public NewsletterSubscriberStatus Status { get; set; } = NewsletterSubscriberStatus.Active;
    }
}
