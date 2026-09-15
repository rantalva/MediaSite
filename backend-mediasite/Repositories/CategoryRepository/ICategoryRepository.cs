using MediaSite_backend.Models.Dtos.Category;
using MediaSite_backend.Models.Entities;

namespace MediaSite_backend.Repositories.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<GetCategoryWithPostsDto>> GetCategoriesWithArticlesAsync();
        Task<Category> GetCategoryByIdAsync(Guid id);
        Task<GetCategoryWithPostsDto?> GetCategoryWithArticlesByIdAsync(Guid id);
        Task<Category> EditCategoryAsync(Guid id, CategoryDto categoryDto);
        Task<Category> CreateCategoryAsync(CategoryDto categoryDto);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<GetCategoryWithPostsDto> GetCategoryWithArticlesByNameAsync(string categoryName);
    }
}
