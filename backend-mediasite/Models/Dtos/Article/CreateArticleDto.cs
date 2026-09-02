using MediaSite_backend.Models.Entities;
using System.Text.RegularExpressions;

namespace MediaSite_backend.Models.Dtos.Article
{
    public class CreateArticleDto
    {
        public required string Title { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public required string HeroImage { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid AuthorId { get; set; }
    }

}
