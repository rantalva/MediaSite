using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Dtos.Category;
using MediaSite_backend.Models.Entities;
using MediaSite_backend.Repositories.ArticleRepository;
using MediaSite_backend.Repositories.CategoryRepository;
using Microsoft.AspNetCore.Identity;

namespace MediaSite_backend.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var categoryRepository = services.GetRequiredService<ICategoryRepository>();

            CategoryDto[] categories =
            {
                new CategoryDto()
                {
                    Name  = "Style"
                },
                new CategoryDto()
                {
                    Name  = "Shopping"
                },
                new CategoryDto()
                {
                    Name  = "Culture"
                },
                new CategoryDto()
                {
                    Name  = "Sports"
                },
            };

            foreach (var category in categories) 
            {
                var existingCategory = await categoryRepository.GetCategoryWithArticlesByNameAsync(category.Name);

                if (existingCategory == null) 
                {
                    await categoryRepository.CreateCategoryAsync(category);
                }
            }

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            string[] roles =
            {
                ApplicationUserRoles.Admin,
                ApplicationUserRoles.Editor,
                ApplicationUserRoles.Author,
                ApplicationUserRoles.Member
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            var existingUser =
                await userManager.FindByEmailAsync("test@gmail.com");

            if (existingUser == null)
            {
                var testUser = new ApplicationUser
                {
                    Email = "test@gmail.com",
                    UserName = "test@gmail.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    testUser,
                    "TestPassword123!"
                );

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        testUser,
                        ApplicationUserRoles.Admin
                    );
                }
            }

            var articleRepository = services.GetRequiredService<IArticleRepository>();

            CreateArticleDto[] createArticleDtos =
            {
                new CreateArticleDto()
                {
                    Title = "Syksyn 2026 hajuvesiuutuudet miehille",
                    Content = "1. Hugo Boss bottled Absolu 2. Giorgio Armani I Will 3. Yves Saint Laurent MYSLF EDT Intense 4. Dolce & Gabbana The One Parfum 5. Valentino Vendetta Uomo",
                    HeroImage = "/Uploads/Menswear-closet.jpg",
                    CategoryId = Guid.Parse("7de610c5-ee94-4cfd-8a8b-43d1a7077c6d"),
                    AuthorId = Guid.Parse("01a0a63e-dfda-7d01-99a5-b2e4df5d1135"),
                }
            };

            foreach (var article in createArticleDtos) 
            {
               var testArticle = await articleRepository.GetBySlugAsync("syksyn-2026-hajuvesiuutuudet-miehille");

                if (testArticle == null) 
                {
                    await articleRepository.CreateAsync(article);
                }
            }

        }

    }
}
