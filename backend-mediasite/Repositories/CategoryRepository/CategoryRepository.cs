using MediaSite_backend.Data;
using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Dtos.Category;
using MediaSite_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaSite_backend.Repositories.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public CategoryRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<Category> CreateCategoryAsync(CategoryDto categoryDto)
        {
            if (await _applicationDbContext.Categories.AnyAsync(c => c.Name == categoryDto.Name))
            {
                return null;
            }

            var newCategory = new Category();

            newCategory.Name = categoryDto.Name;

            await _applicationDbContext.Categories.AddAsync(newCategory);

            await _applicationDbContext.SaveChangesAsync();

            return newCategory;
        }
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _applicationDbContext.Categories.FindAsync(id);

            if (category == null)
                return false;

            _applicationDbContext.Categories.Remove(category);
            await _applicationDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<Category> EditCategoryAsync(Guid id, CategoryDto categoryDto)
        {
            var category = await _applicationDbContext.Categories.FindAsync(id);

            if (category != null)
            {
                category.Name = categoryDto.Name;

                await _applicationDbContext.SaveChangesAsync();

                return category;
            }

            return null;
        }

        public async Task<GetCategoryWithPostsDto?> GetCategoryWithArticlesByIdAsync(Guid id)
        {
            return await _applicationDbContext.Categories
                .Where(c => c.Id == id)
                .Select(c => new GetCategoryWithPostsDto
                {
                    Name = c.Name,

                    Articles = c.Articles
                        .Select(a => new CategoryArticleDto
                        {
                            Id = a.Id,
                            Title = a.Title,
                            Slug = a.Slug,
                            HeroImage = a.HeroImage
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            var category = await _applicationDbContext.Categories.FindAsync(id);

            if (category == null)
            {
                return null;
            }

            return category;
        }

        public async Task<IEnumerable<GetCategoryWithPostsDto>> GetCategoriesWithArticlesAsync()
        {
            return await _applicationDbContext.Categories
                .OrderBy(c => c.Name)
                .Select(c => new GetCategoryWithPostsDto
                {
                    Name = c.Name,

                    Articles = c.Articles
                        .Select(a => new CategoryArticleDto
                        {
                            Id = a.Id,
                            Title = a.Title,
                            Slug = a.Slug,
                            HeroImage = a.HeroImage
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<GetCategoryWithPostsDto> GetCategoryWithArticlesByNameAsync(string? categoryName)
        {
            return await _applicationDbContext.Categories
               .Where(c => c.Name == categoryName)
               .Select(c => new GetCategoryWithPostsDto
               {
                   Name = c.Name,

                   Articles = c.Articles
                       .Select(a => new CategoryArticleDto
                       {
                           Id = a.Id,
                           Title = a.Title,
                           Slug = a.Slug,
                           HeroImage = a.HeroImage
                       })
                       .ToList()
               })
               .FirstOrDefaultAsync();
        }
    }
}
