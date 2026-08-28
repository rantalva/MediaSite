using System.Text.RegularExpressions;

namespace MediaSite_backend.Models.Dtos.Article
{
    public class ArticleDto
    {
        public required string Title { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public required string HeroImage { get; set; } = string.Empty;

        public string GenerateSlugFromTitle(string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            var slug = title.Trim().ToLowerInvariant();

            slug = Regex.Replace(slug, @"[^\p{L}\p{N}\s-]", "");

            slug = Regex.Replace(slug, @"[\s-]+", "-");

            return slug.Trim('-');
        }

    }

}
