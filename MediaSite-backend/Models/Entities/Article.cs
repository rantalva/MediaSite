using System.ComponentModel;

namespace MediaSite_backend.Models.Entities
{
    public class Article
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string HeroImage { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastEditDate { get; set; }
        public Category? Category { get; set; }
    }
}
