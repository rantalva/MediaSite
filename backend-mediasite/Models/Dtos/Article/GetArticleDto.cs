namespace MediaSite_backend.Models.Dtos.Article
{
    public class GetArticleDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; } = string.Empty;
        public required string Slug { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public required string HeroImage { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastEditDate { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
    }
}
