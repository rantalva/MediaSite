using System.ComponentModel.DataAnnotations;

namespace MediaSite_backend.Models.Dtos.NewsletterSubscriber
{
    public class NewsletterSubscriberDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
