using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Entities;

namespace MediaSite_backend.Repositories
{
    public interface IArticleRepository
    {
        Task<IEnumerable<GetArticleDto>> GetAllArticlesAsync();
        Task<Article?> GetByIdAsync(Guid id);
        Task<Article?> GetBySlugAsync(string slug);
        Task<Article> CreateAsync(CreateArticleDto createArticleDto);
        Task<Article> UpdateAsync(Guid id, EditArticleDto editArticleDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
