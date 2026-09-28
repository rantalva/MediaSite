using MediaSite_backend.Data;
using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Dtos.Category;
using MediaSite_backend.Models.Entities;
using MediaSite_backend.Repositories.CategoryRepository;
using MediaSite_backend.Tests.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaSite_backend.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly ICategoryRepository _repo = Substitute.For<ICategoryRepository>();

    // CategoriesController takes ApplicationDbContext but never reads it, so a context built with
    // provider-less options is enough. NSubstitute cannot proxy it: ApplicationDbContext has no
    // parameterless constructor. This is the same latent coupling that should be removed from
    // the controller signature.
    private readonly ApplicationDbContext _context =
        new(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

    private CategoriesController CreateController() =>
        new CategoriesController(_context, _repo).WithStubbedUrlHelper();

    private static GetCategoryWithPostsDto NewCategoryWithPosts(string name) => new()
    {
        Name = name,
        Articles = new List<CategoryArticleDto>
        {
            new() { Id = Guid.NewGuid(), Title = "Artikkeli", Slug = "artikkeli", HeroImage = "/Uploads/x.jpg" }
        }
    };

    [Fact]
    public async Task GetCategory_returns_Ok_with_categories()
    {
        var expected = new[] { NewCategoryWithPosts("Style"), NewCategoryWithPosts("Sports") };
        _repo.GetCategoriesWithArticlesAsync().Returns(Task.FromResult<IEnumerable<GetCategoryWithPostsDto>>(expected));

        var result = await CreateController().GetCategory();

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetCategory_returns_NotFound_when_repository_returns_null()
    {
        _repo.GetCategoriesWithArticlesAsync()
            .Returns(Task.FromResult<IEnumerable<GetCategoryWithPostsDto>>(null!));

        var result = await CreateController().GetCategory();

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetCategoryById_returns_Ok_when_found()
    {
        var expected = NewCategoryWithPosts("Style");
        _repo.GetCategoryWithArticlesByIdAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult<GetCategoryWithPostsDto?>(expected));

        var result = await CreateController().GetCategory(Guid.NewGuid());

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task GetCategoryById_returns_NotFound_when_missing()
    {
        _repo.GetCategoryWithArticlesByIdAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult<GetCategoryWithPostsDto?>(null));

        var result = await CreateController().GetCategory(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetCategoryByName_returns_Ok_when_found()
    {
        var expected = NewCategoryWithPosts("Culture");
        _repo.GetCategoryWithArticlesByNameAsync("Culture")
            .Returns(Task.FromResult<GetCategoryWithPostsDto>(expected));

        var result = await CreateController().GetCategoryByName("Culture");

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task GetCategoryByName_returns_NotFound_when_missing()
    {
        _repo.GetCategoryWithArticlesByNameAsync(Arg.Any<string>())
            .Returns(Task.FromResult<GetCategoryWithPostsDto>(null!));

        var result = await CreateController().GetCategoryByName("Tuntematon");

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostCategory_returns_CreatedAtAction_on_success()
    {
        // The repository interface returns the Category entity while the action is declared
        // ActionResult<CategoryDto>; the mismatch is hidden by Ok() widening to ActionResult.
        var created = new Category { Id = Guid.NewGuid(), Name = "Style" };
        _repo.CreateCategoryAsync(Arg.Any<CategoryDto>()).Returns(Task.FromResult(created));

        var result = await CreateController().PostCategory(new CategoryDto { Name = "Style" });

        result.Result.Should().BeOfType<CreatedAtActionResult>()
            .Which.ActionName.Should().Be("GetCategory");
    }

    [Fact]
    public async Task PostCategory_returns_BadRequest_when_name_already_exists()
    {
        _repo.CreateCategoryAsync(Arg.Any<CategoryDto>()).Returns(Task.FromResult<Category>(null!));

        var result = await CreateController().PostCategory(new CategoryDto { Name = "Style" });

        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task EditCategory_returns_Ok_when_updated()
    {
        var updated = new Category { Id = Guid.NewGuid(), Name = "Uusi nimi" };
        _repo.EditCategoryAsync(Arg.Any<Guid>(), Arg.Any<CategoryDto>()).Returns(Task.FromResult(updated));

        var result = await CreateController().EditCategory(updated.Id, new CategoryDto { Name = "Uusi nimi" });

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeSameAs(updated);
    }

    [Fact]
    public async Task EditCategory_returns_BadRequest_when_category_missing()
    {
        // Deliberately BadRequest, not NotFound: an edit against an unknown id is a client error
        // here, which is inconsistent with ArticlesController.EditArticle returning 404.
        _repo.EditCategoryAsync(Arg.Any<Guid>(), Arg.Any<CategoryDto>()).Returns(Task.FromResult<Category>(null!));

        var result = await CreateController().EditCategory(Guid.NewGuid(), new CategoryDto { Name = "x" });

        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task DeleteCategory_returns_NoContent_when_existed()
    {
        _repo.DeleteCategoryAsync(Arg.Any<Guid>()).Returns(Task.FromResult(true));

        var result = await CreateController().DeleteCategory(Guid.NewGuid());

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteCategory_returns_NotFound_when_missing()
    {
        _repo.DeleteCategoryAsync(Arg.Any<Guid>()).Returns(Task.FromResult(false));

        var result = await CreateController().DeleteCategory(Guid.NewGuid());

        result.Should().BeOfType<NotFoundResult>();
    }
}
