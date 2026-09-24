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
        private static string _testAuthorGuid = "01a075c5-ca0f-717f-a73f-8117fab80e80";
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

            ApplicationUser[] seedUsers =
            {
                new ApplicationUser
                {
                    Email = "test@gmail.com",
                    UserName = "test@gmail.com",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Email = "alvari.rantapelkonen@gmail.com",
                    UserName = "alvari.rantapelkonen@gmail.com",
                    EmailConfirmed = true
                }
            };

            foreach (var user in seedUsers)
            {
                var userManager =
                    services.GetRequiredService<UserManager<ApplicationUser>>();

                var existingUser =
                    await userManager.FindByEmailAsync(user.Email);

                if (existingUser == null)
                {
                    var result = await userManager.CreateAsync(user, "TestPassword123!");

                    if (result.Succeeded)
                    {
                        if (user.Email != "alvari.rantapelkonen@gmail.com")
                        {
                            await userManager.AddToRoleAsync(user, ApplicationUserRoles.Admin);
                        }
                        else
                        {
                            await userManager.AddToRoleAsync(user, ApplicationUserRoles.Author);
                        }
                    }
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
                    CategoryId = Guid.Parse("6425fd8a-65aa-4973-beb2-82733f34deca"),
                    AuthorId = Guid.Parse(_testAuthorGuid),
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
