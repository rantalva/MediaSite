using MediaSite_backend.Data;
using MediaSite_backend.Models.Dtos.Category;
using MediaSite_backend.Repositories.CategoryRepository;
using MediaSite_backend.Tests.Common;
using Microsoft.EntityFrameworkCore;

namespace MediaSite_backend.Tests.Repositories;

[Collection(DatabaseCollection.Name)]
[Trait("Category", "Integration")]
public class CategoryRepositoryTests
{
    private readonly PostgresFixture _fixture;

    public CategoryRepositoryTests(PostgresFixture fixture) => _fixture = fixture;

    private static CategoryRepository CreateRepository(ApplicationDbContext context) => new(context);

    [Fact]
    public async Task CreateCategoryAsync_persists_the_name()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var created = await CreateRepository(context).CreateCategoryAsync(new CategoryDto { Name = "Style" });

        created.Id.Should().NotBeEmpty();
        (await context.Categories.SingleAsync()).Name.Should().Be("Style");
    }

    [Fact]
    public async Task CreateCategoryAsync_returns_null_for_duplicate_name()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var repository = CreateRepository(context);

        await repository.CreateCategoryAsync(new CategoryDto { Name = "Style" });
        var duplicate = await repository.CreateCategoryAsync(new CategoryDto { Name = "Style" });

        duplicate.Should().BeNull();
        (await context.Categories.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task GetCategoriesWithArticlesAsync_returns_categories_ordered_by_name()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        context.Categories.AddRange(
            TestDataFactory.NewCategory("Style"),
            TestDataFactory.NewCategory("Culture"),
            TestDataFactory.NewCategory("Sports"));
        await context.SaveChangesAsync();

        var categories = await CreateRepository(context).GetCategoriesWithArticlesAsync();

        categories.Select(c => c.Name).Should().ContainInOrder("Culture", "Sports", "Style");
    }

    [Fact]
    public async Task GetCategoriesWithArticlesAsync_projects_articles_onto_each_category()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var style = TestDataFactory.NewCategory("Style");
        var culture = TestDataFactory.NewCategory("Culture");
        var author = TestDataFactory.NewUser();
        context.Categories.AddRange(style, culture);
        context.Users.Add(author);
        await context.SaveChangesAsync();

        context.Articles.AddRange(
            TestDataFactory.NewArticle("Hajuvesi", categoryId: style.Id, authorId: author.Id),
            TestDataFactory.NewArticle("Kulta-aika", categoryId: style.Id, authorId: author.Id));
        await context.SaveChangesAsync();

        var categories = await CreateRepository(context).GetCategoriesWithArticlesAsync();

        categories.Single(c => c.Name == "Style").Articles.Select(a => a.Title)
            .Should().BeEquivalentTo(["Hajuvesi", "Kulta-aika"]);
        categories.Single(c => c.Name == "Culture").Articles.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCategoriesWithArticlesAsync_returns_empty_when_no_categories_exist()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        (await CreateRepository(context).GetCategoriesWithArticlesAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetCategoryWithArticlesByIdAsync_returns_the_matching_category()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var style = TestDataFactory.NewCategory("Style");
        context.Categories.Add(style);
        await context.SaveChangesAsync();

        var found = await CreateRepository(context).GetCategoryWithArticlesByIdAsync(style.Id);

        found.Should().NotBeNull();
        found!.Name.Should().Be("Style");
        found.Articles.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCategoryWithArticlesByIdAsync_returns_null_for_unknown_id()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        (await CreateRepository(context).GetCategoryWithArticlesByIdAsync(Guid.NewGuid())).Should().BeNull();
    }

    [Fact]
    public async Task GetCategoryWithArticlesByNameAsync_returns_the_matching_category()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        context.Categories.Add(TestDataFactory.NewCategory("Style"));
        await context.SaveChangesAsync();

        var found = await CreateRepository(context).GetCategoryWithArticlesByNameAsync("Style");

        found.Should().NotBeNull();
        found!.Name.Should().Be("Style");
    }

    [Fact]
    public async Task GetCategoryWithArticlesByNameAsync_returns_null_for_unknown_name()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        (await CreateRepository(context).GetCategoryWithArticlesByNameAsync("Tuntematon")).Should().BeNull();
    }

    [Fact]
    public async Task EditCategoryAsync_renames_and_persists_the_category()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var category = TestDataFactory.NewCategory("Style");
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var edited = await CreateRepository(context).EditCategoryAsync(category.Id, new CategoryDto { Name = "Muoti" });

        edited!.Name.Should().Be("Muoti");
        (await context.Categories.SingleAsync()).Name.Should().Be("Muoti");
    }

    [Fact]
    public async Task EditCategoryAsync_returns_null_for_unknown_id()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        (await CreateRepository(context).EditCategoryAsync(Guid.NewGuid(), new CategoryDto { Name = "x" })).Should().BeNull();
    }

    [Fact]
    public async Task DeleteCategoryAsync_removes_the_category()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var category = TestDataFactory.NewCategory("Style");
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        (await CreateRepository(context).DeleteCategoryAsync(category.Id)).Should().BeTrue();

        (await context.Categories.AnyAsync(c => c.Id == category.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteCategoryAsync_returns_false_for_unknown_id()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        (await CreateRepository(context).DeleteCategoryAsync(Guid.NewGuid())).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteCategoryAsync_fails_when_the_category_still_has_articles()
    {
        // ApplicationDbContext maps Article.Category with OnDelete(DeleteBehavior.SetNull), but
        // Article.CategoryId is a non-nullable Guid, so there is no null to set. EF detects the
        // impossible sever in the change tracker and throws before the statement reaches Postgres.
        // Any category referenced by an article is effectively undeletable, and ArticlesController
        // surfaces this as a 500 rather than the 404/400 its sibling endpoints return.
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var category = TestDataFactory.NewCategory("Style");
        var author = TestDataFactory.NewUser();
        context.Categories.Add(category);
        context.Users.Add(author);
        await context.SaveChangesAsync();

        context.Articles.Add(TestDataFactory.NewArticle(categoryId: category.Id, authorId: author.Id));
        await context.SaveChangesAsync();

        var act = () => CreateRepository(context).DeleteCategoryAsync(category.Id);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
