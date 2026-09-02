namespace MediaSite_backend.Models.Dtos.Article
{
    public class EditArticleDto
    {
        public required string Title { get; set; } = string.Empty;
        public required string Slug { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public required string HeroImage { get; set; } = string.Empty;
        public DateTime? LastEditDate { get; set; }
        public Guid CategoryId { get; set; }
        public Guid AuthorId { get; set; }
    }
}
