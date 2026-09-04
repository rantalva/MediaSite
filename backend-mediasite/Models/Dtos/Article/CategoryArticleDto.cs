namespace MediaSite_backend.Models.Dtos.Article
{
    public class CategoryArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string HeroImage { get; set; } = string.Empty;
    }
}
