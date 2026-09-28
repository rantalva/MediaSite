using MediaSite_backend.Data;
using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Repositories.ArticleRepository;
using MediaSite_backend.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Slugify;

namespace MediaSite_backend.Tests.Repositories;

[Collection(DatabaseCollection.Name)]
[Trait("Category", "Integration")]
public class ArticleRepositoryTests
{
    private readonly PostgresFixture _fixture;

    public ArticleRepositoryTests(PostgresFixture fixture) => _fixture = fixture;

    private ArticleRepository CreateRepository(ApplicationDbContext context) =>
        new(context, new SlugHelper());

    /// <summary>Npgsql rejects DateTimeKind.Unspecified against a timestamptz column.</summary>
    private static DateTime Utc(int year, int month, int day) =>
        new(year, month, day, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>Articles require an existing author and category because both FKs are non-nullable Guid.</summary>
    private async Task<(Guid CategoryId, Guid AuthorId)> SeedDependenciesAsync(ApplicationDbContext context)
    {
        var category = TestDataFactory.NewCategory();
        var author = TestDataFactory.NewUser();

        context.Categories.Add(category);
        context.Users.Add(author);
        await context.SaveChangesAsync();

        return (category.Id, author.Id);
    }

    [Fact]
    public async Task CreateAsync_persists_every_supplied_field()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var (categoryId, authorId) = await SeedDependenciesAsync(context);

        var dto = new CreateArticleDto
        {
            Title = "Syksyn hajuvesi",
            Content = "Hugo Boss, Armani, YSL",
            HeroImage = "/Uploads/Menswear-closet.jpg",
            CategoryId = categoryId,
            AuthorId = authorId
        };

        var created = await CreateRepository(context).CreateAsync(dto);

        var stored = await context.Articles.SingleAsync();
        stored.Title.Should().Be(dto.Title);
        stored.Content.Should().Be(dto.Content);
        stored.HeroImage.Should().Be(dto.HeroImage);
        stored.CategoryId.Should().Be(categoryId);
        stored.AuthorId.Should().Be(authorId);
        created.Id.Should().Be(stored.Id);
    }

    [Theory]
    [InlineData("Syksyn 2026 hajuvesiuutuudet", "syksyn-2026-hajuvesiuutuudet")]
    [InlineData("Äänekosken 5 vinkkiä", "aanekosken-5-vinkkia")]
    [InlineData("  Ylimääräiset   välit  ", "ylimaaraiset-valit")]
    public async Task CreateAsync_generates_slug_from_the_title(string title, string expectedSlug)
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var (categoryId, authorId) = await SeedDependenciesAsync(context);

        await CreateRepository(context).CreateAsync(new CreateArticleDto
        {
            Title = title,
            Content = "sisalto",
            HeroImage = "/Uploads/x.jpg",
            CategoryId = categoryId,
            AuthorId = authorId
        });

        (await context.Articles.SingleAsync()).Slug.Should().Be(expectedSlug);
    }

    [Fact]
    public async Task CreateAsync_sets_CreatedDate()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var (categoryId, authorId) = await SeedDependenciesAsync(context);
        var before = DateTime.UtcNow;

        await CreateRepository(context).CreateAsync(new CreateArticleDto
        {
            Title = "Uusi",
            Content = "sisalto",
            HeroImage = "/Uploads/x.jpg",
            CategoryId = categoryId,
            AuthorId = authorId
        });

        var created = (await context.Articles.SingleAsync()).CreatedDate;
        created.Should().NotBeNull();
        created.Should().BeOnOrAfter(before);
    }

    [Fact]
    public async Task CreateAsync_returns_null_for_duplicate_title()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var (categoryId, authorId) = await SeedDependenciesAsync(context);
        var repository = CreateRepository(context);

        var dto = new CreateArticleDto
        {
            Title = "Duplikaatti",
            Content = "sisalto",
            HeroImage = "/Uploads/x.jpg",
            CategoryId = categoryId,
            AuthorId = authorId
        };

        await repository.CreateAsync(dto);
        var second = await repository.CreateAsync(dto);

        second.Should().BeNull();
        (await context.Articles.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_fails_when_author_does_not_exist()
    {
        // Article.AuthorId is a non-nullable Guid with a required FK, so the
        // `a.Author != null ? ... : null` branch in GetAllArticlesAsync is unreachable:
        // a row can never be persisted without an author.
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var category = TestDataFactory.NewCategory();
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var act = () => CreateRepository(context).CreateAsync(new CreateArticleDto
        {
            Title = "Olematon kirjoittaja",
            Content = "sisalto",
            HeroImage = "/Uploads/x.jpg",
            CategoryId = category.Id,
            AuthorId = Guid.NewGuid()
        });

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task GetAllArticlesAsync_orders_newest_first()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var (categoryId, authorId) = await SeedDependenciesAsync(context);

        context.Articles.AddRange(
            TestDataFactory.NewArticle("Vanhin", categoryId: categoryId, authorId: authorId, createdDate: Utc(2026, 1, 1)),
            TestDataFactory.NewArticle("Uusin", categoryId: categoryId, authorId: authorId, createdDate: Utc(2026, 3, 1)),
            TestDataFactory.NewArticle("Keskimmainen", categoryId: categoryId, authorId: authorId, createdDate: Utc(2026, 2, 1)));
        await context.SaveChangesAsync();

        var articles = await CreateRepository(context).GetAllArticlesAsync();

        articles.Select(a => a.Title).Should().ContainInOrder("Uusin", "Keskimmainen", "Vanhin");
    }

    [Fact]
    public async Task GetAllArticlesAsync_returns_empty_when_there_are_no_articles()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        (await CreateRepository(context).GetAllArticlesAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllArticlesAsync_joins_CategoryName()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var (categoryId, authorId) = await SeedDependenciesAsync(context);

        context.Articles.Add(TestDataFactory.NewArticle(categoryId: categoryId, authorId: authorId));
        await context.SaveChangesAsync();

        var article = (await CreateRepository(context).GetAllArticlesAsync()).Single();
        article.CategoryId.Should().Be(categoryId);
        article.CategoryName.Should().Be("Style");
    }

    [Fact]
    public async Task GetAllArticlesAsync_joins_AuthorName_as_First_and_Last()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var (categoryId, authorId) = await SeedDependenciesAsync(context);

        context.Articles.Add(TestDataFactory.NewArticle(categoryId: categoryId, authorId: authorId));
        await context.SaveChangesAsync();

        var article = (await CreateRepository(context).GetAllArticlesAsync()).Single();
        article.AuthorId.Should().Be(authorId);
        article.AuthorName.Should().Be("Alvari Rantapelkonen");
    }

    [Fact]
    public async Task GetBySlugAsync_returns_article_when_slug_matches()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var (categoryId, authorId) = await SeedDependenciesAsync(context);

        context.Articles.Add(TestDataFactory.NewArticle("Syksyn hajuvesi", categoryId: categoryId, authorId: authorId));
        await context.SaveChangesAsync();

        var found = await CreateRepository(context).GetBySlugAsync("syksyn-hajuvesi");

        found.Should().NotBeNull();
        found!.Title.Should().Be("Syksyn hajuvesi");
        found.AuthorName.Should().Be("Alvari Rantapelkonen");
    }

    [Fact]
    public async Task GetBySlugAsync_returns_null_for_unknown_slug()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        (await CreateRepository(context).GetBySlugAsync("tuntematon")).Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_for_unknown_id()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        (await CreateRepository(context).GetByIdAsync(Guid.NewGuid())).Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_persists_changes_and_regenerates_slug()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var (categoryId, authorId) = await SeedDependenciesAsync(context);

        var article = TestDataFactory.NewArticle("Vanha otsikko", categoryId: categoryId, authorId: authorId);
        context.Articles.Add(article);
        await context.SaveChangesAsync();

        var before = DateTime.UtcNow;
        var updated = await CreateRepository(context).UpdateAsync(article.Id, new EditArticleDto
        {
            Title = "Uusi otsikko",
            Slug = "ohitettu-arvo",
            Content = "uusi sisalto",
            HeroImage = "/Uploads/uusi.jpg",
            CategoryId = categoryId,
            AuthorId = authorId
        });

        updated!.Title.Should().Be("Uusi otsikko");
        // The slug is regenerated from the title; the slug supplied in the DTO is ignored.
        updated.Slug.Should().Be("uusi-otsikko");
        updated.Content.Should().Be("uusi sisalto");
        updated.HeroImage.Should().Be("/Uploads/uusi.jpg");
        updated.LastEditDate.Should().NotBeNull();
        updated.LastEditDate.Should().BeOnOrAfter(before);

        var stored = await context.Articles.SingleAsync(a => a.Id == article.Id);
        stored.Title.Should().Be("Uusi otsikko");
        stored.Slug.Should().Be("uusi-otsikko");
    }

    [Fact]
    public async Task UpdateAsync_returns_null_for_unknown_id()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var result = await CreateRepository(context).UpdateAsync(Guid.NewGuid(), new EditArticleDto
        {
            Title = "Ei löydy",
            Slug = "ei-loydy",
            Content = "sisalto",
            HeroImage = "/Uploads/x.jpg"
        });

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_fails_when_the_dto_omits_AuthorId()
    {
        // EditArticleDto marks Title/Slug/Content/HeroImage as required but leaves AuthorId and
        // CategoryId as plain Guids, so they default to Guid.Empty. ArticleRepository.UpdateAsync
        // copies them onto the tracked entity unconditionally, so a client that updates an
        // article's body without resending both ids gets a foreign key violation instead of a
        // partial update. This makes a PUT either all-or-nothing in a way the API does not document.
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var (categoryId, authorId) = await SeedDependenciesAsync(context);

        var article = TestDataFactory.NewArticle(categoryId: categoryId, authorId: authorId);
        context.Articles.Add(article);
        await context.SaveChangesAsync();

        var act = () => CreateRepository(context).UpdateAsync(article.Id, new EditArticleDto
        {
            Title = "Uusi otsikko",
            Slug = "uusi-otsikko",
            Content = "uusi sisalto",
            HeroImage = "/Uploads/uusi.jpg"
        });

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task DeleteAsync_removes_the_article()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var (categoryId, authorId) = await SeedDependenciesAsync(context);

        var article = TestDataFactory.NewArticle(categoryId: categoryId, authorId: authorId);
        context.Articles.Add(article);
        await context.SaveChangesAsync();

        (await CreateRepository(context).DeleteAsync(article.Id)).Should().BeTrue();

        (await context.Articles.AnyAsync(a => a.Id == article.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_returns_false_for_unknown_id()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        (await CreateRepository(context).DeleteAsync(Guid.NewGuid())).Should().BeFalse();
    }
}
