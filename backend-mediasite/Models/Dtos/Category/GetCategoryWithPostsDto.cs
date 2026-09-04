using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Entities;

namespace MediaSite_backend.Models.Dtos.Category
{
    public class GetCategoryWithPostsDto
    {
        public string? Name { get; set; }
        public ICollection<CategoryArticleDto> Articles { get; set; } = new List<CategoryArticleDto>();
    }
}
