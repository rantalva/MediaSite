using System.ComponentModel.DataAnnotations;

namespace MediaSite_backend.Models.Entities
{
    public enum NewsletterSubscriberStatus
    {
        Active,
        InActive,
        RemovalRequested
    }

    public class NewsletterSubscriber
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public NewsletterSubscriberStatus Status { get; set; } = NewsletterSubscriberStatus.Active;
    }
}
