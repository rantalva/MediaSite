using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Entities;

namespace MediaSite_backend.Repositories
{
    public interface IArticleRepository
    {
        Task<IEnumerable<ArticleDto>> GetAllArticlesAsync();
        Task<Article?> GetByIdAsync(Guid id);
        Task<Article?> GetBySlugAsync(string slug);
        Task<Article> CreateAsync(ArticleDto articleDto);
        Task<Article> UpdateAsync(Guid id, ArticleDto articleDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
