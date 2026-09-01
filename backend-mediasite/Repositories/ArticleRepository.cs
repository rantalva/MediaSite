using MediaSite_backend.Data;
using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace MediaSite_backend.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ArticleRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<Article> CreateAsync(ArticleDto articleDto)
        {
            if (await _applicationDbContext.Articles.AnyAsync(a => a.Title == articleDto.Title)) 
            {
                return null;
            }
            // this should maybe be refactored to ArticleService.cs in the future? :)
            var newArticle = new Article();

            newArticle.Title = articleDto.Title;
            newArticle.Slug = articleDto.Slug;
            newArticle.Content = articleDto.Content;
            newArticle.HeroImage = articleDto.HeroImage;
            newArticle.CategoryId = articleDto.CategoryId;
            newArticle.AuthorId = articleDto.AuthorId;

            _applicationDbContext.Articles.Add(newArticle);
            await _applicationDbContext.SaveChangesAsync();

            return newArticle;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var article = await _applicationDbContext.Articles
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                return false;
            }

            _applicationDbContext.Articles.Remove(article);

            await _applicationDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync()
        {
            return await _applicationDbContext.Articles
                .Select(a => new ArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Content = a.Content,
                    HeroImage = a.HeroImage,
                    CreatedDate = a.CreatedDate,
                    LastEditDate = a.LastEditDate,
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category!.Name,

                    AuthorId = a.AuthorId,
                    AuthorName = a.Author != null
                        ? a.Author.FirstName + " " + a.Author.LastName
                        : null
                })
                .ToListAsync();
        }

        public async Task<Article?> GetByIdAsync(Guid id)
        {
            return await _applicationDbContext.Articles.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Article?> GetBySlugAsync(string slug)
        {
            return await _applicationDbContext.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
        }

        public async Task<Article?> UpdateAsync(Guid id, ArticleDto articleDto)
        {
            var article = await _applicationDbContext.Articles.FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                return null;
            }

            article.Title = articleDto.Title;
            article.Slug = articleDto.Slug;
            article.Content = articleDto.Content;
            article.HeroImage = articleDto.HeroImage;
            article.CategoryId = articleDto.CategoryId;
            article.AuthorId = articleDto.AuthorId;
            article.LastEditDate = DateTime.UtcNow;

            await _applicationDbContext.SaveChangesAsync();

            return article;
        }
    }
}
