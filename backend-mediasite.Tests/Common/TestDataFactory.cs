using MediaSite_backend.Models.Entities;
using Slugify;

namespace MediaSite_backend.Tests.Common;

public static class TestDataFactory
{
    public static SlugHelper SlugHelper { get; } = new();

    public static Category NewCategory(string name = "Style") => new() { Name = name };

    public static ApplicationUser NewUser(
        string email = "test@gmail.com",
        string firstName = "Alvari",
        string lastName = "Rantapelkonen")
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName
        };
    }

    public static Article NewArticle(
        string title = "Syksyn hajuvesi",
        string content = "Hugo Boss, Armani, YSL",
        string heroImage = "/Uploads/Menswear-closet.jpg",
        Guid? categoryId = null,
        Guid? authorId = null,
        DateTime? createdDate = null)
    {
        return new Article
        {
            Id = Guid.NewGuid(),
            Title = title,
            Slug = SlugHelper.GenerateSlug(title),
            Content = content,
            HeroImage = heroImage,
            CategoryId = categoryId ?? Guid.NewGuid(),
            AuthorId = authorId ?? Guid.NewGuid(),
            CreatedDate = createdDate ?? DateTime.UtcNow
        };
    }
}
