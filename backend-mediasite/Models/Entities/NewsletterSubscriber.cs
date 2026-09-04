using System.ComponentModel.DataAnnotations;

namespace MediaSite_backend.Models.Entities
{
    public class NewsletterSubscriber
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
