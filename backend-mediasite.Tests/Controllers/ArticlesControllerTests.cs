using MediaSite_backend.Controllers;
using MediaSite_backend.Models.Dtos.Article;
using MediaSite_backend.Models.Entities;
using MediaSite_backend.Repositories.ArticleRepository;
using MediaSite_backend.Tests.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaSite_backend.Tests.Controllers;

public class ArticlesControllerTests
{
    private readonly IArticleRepository _repo = Substitute.For<IArticleRepository>();

    private ArticlesController CreateController() => new ArticlesController(_repo).WithStubbedUrlHelper();

    private static GetArticleDto NewDto(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Title = "Syksyn hajuvesi",
        Slug = "syksyn-hajuvesi",
        Content = "Hugo Boss, Armani, YSL",
        HeroImage = "/Uploads/Menswear-closet.jpg",
        CategoryName = "Style",
        AuthorName = "Alvari Rantapelkonen"
    };

    [Fact]
    public async Task GetArticles_returns_Ok_with_articles_from_repository()
    {
        var expected = new[] { NewDto(), NewDto() };
        _repo.GetAllArticlesAsync().Returns(Task.FromResult<IEnumerable<GetArticleDto>>(expected));

        var result = await CreateController().GetArticles();

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeEquivalentTo(expected);
        await _repo.Received(1).GetAllArticlesAsync();
    }

    [Fact]
    public async Task GetArticleByGuid_returns_Ok_when_article_exists()
    {
        var article = new Article { Id = Guid.NewGuid(), Title = "Olemassa" };
        _repo.GetByIdAsync(article.Id).Returns(Task.FromResult<Article?>(article));

        var result = await CreateController().GetArticleByGuid(article.Id);

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeSameAs(article);
    }

    [Fact]
    public async Task GetArticleByGuid_returns_NotFound_when_article_missing()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<Article?>(null));

        var result = await CreateController().GetArticleByGuid(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetArticleByGuid_does_not_short_circuit_on_Guid_Empty()
    {
        // ArticlesController.cs:26 tests `id != null` on a non-nullable Guid, so the compiler
        // emits CS8073 and the BadRequest branch below it is unreachable. This locks in the
        // current behaviour: an empty Guid still hits the repository rather than returning 400.
        _repo.GetByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<Article?>(null));

        var result = await CreateController().GetArticleByGuid(Guid.Empty);

        result.Result.Should().BeOfType<NotFoundResult>();
        await _repo.Received(1).GetByIdAsync(Guid.Empty);
    }

    [Fact]
    public async Task GetArticleBySlug_returns_Ok_with_dto_when_found()
    {
        var dto = NewDto();
        _repo.GetBySlugAsync("syksyn-hajuvesi").Returns(Task.FromResult<GetArticleDto?>(dto));

        var result = await CreateController().GetArticleBySlug("syksyn-hajuvesi");

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeSameAs(dto);
    }

    [Fact]
    public async Task GetArticleBySlug_returns_NotFound_when_slug_unknown()
    {
        _repo.GetBySlugAsync(Arg.Any<string>()).Returns(Task.FromResult<GetArticleDto?>(null));

        var result = await CreateController().GetArticleBySlug("does-not-exist");

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetArticleBySlug_returns_BadRequest_when_slug_is_null()
    {
        var result = await CreateController().GetArticleBySlug(null!);

        result.Result.Should().BeOfType<BadRequestResult>();
        await _repo.DidNotReceive().GetBySlugAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task CreateArticle_returns_CreatedAtRoute_on_success()
    {
        var created = new Article { Id = Guid.NewGuid(), Title = "Uusi" };
        _repo.CreateAsync(Arg.Any<CreateArticleDto>()).Returns(Task.FromResult(created));

        var result = await CreateController().CreateArticle(new CreateArticleDto
        {
            Title = "Uusi",
            Content = "sisalto",
            HeroImage = "/Uploads/x.jpg"
        });

        result.Result.Should().BeOfType<CreatedAtRouteResult>()
            .Which.RouteName.Should().Be("GetArticles");
    }

    [Fact]
    public async Task CreateArticle_returns_BadRequest_when_repository_rejects_duplicate_title()
    {
        _repo.CreateAsync(Arg.Any<CreateArticleDto>()).Returns(Task.FromResult<Article>(null!));

        var result = await CreateController().CreateArticle(new CreateArticleDto
        {
            Title = "duplikaatti",
            Content = "sisalto",
            HeroImage = "/Uploads/x.jpg"
        });

        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task EditArticle_returns_Ok_when_article_updated()
    {
        var updated = new Article { Id = Guid.NewGuid(), Title = "Paivitetty" };
        _repo.UpdateAsync(Arg.Any<Guid>(), Arg.Any<EditArticleDto>()).Returns(Task.FromResult(updated));

        var result = await CreateController().EditArticle(updated.Id, NewEditDto());

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeSameAs(updated);
    }

    [Fact]
    public async Task EditArticle_returns_NotFound_when_article_missing()
    {
        _repo.UpdateAsync(Arg.Any<Guid>(), Arg.Any<EditArticleDto>()).Returns(Task.FromResult<Article>(null!));

        var result = await CreateController().EditArticle(Guid.NewGuid(), NewEditDto());

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteArticle_returns_NoContent_when_article_existed()
    {
        _repo.DeleteAsync(Arg.Any<Guid>()).Returns(Task.FromResult(true));

        var result = await CreateController().DeleteArticle(Guid.NewGuid());

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteArticle_returns_NotFound_when_article_missing()
    {
        _repo.DeleteAsync(Arg.Any<Guid>()).Returns(Task.FromResult(false));

        var result = await CreateController().DeleteArticle(Guid.NewGuid());

        result.Should().BeOfType<NotFoundResult>();
    }

    /*  [Fact]
      public void GetArticleBySlug_is_restricted_to_Admin_role()
      {
          // A public read endpoint guarded by [Authorize(Roles = Admin)] at
          // ArticlesController.cs:38 is almost certainly a copy-paste error. This test makes the
          // current state explicit so correcting it shows up as a deliberate, reviewed diff.
          var authorize = typeof(ArticlesController)
              .GetMethod(nameof(ArticlesController.GetArticleBySlug))!
              .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
              .Cast<AuthorizeAttribute>()
              .Single();

          authorize.Roles.Should().Be(ApplicationUserRoles.Admin);

      }*/

    [Fact]
    public void CreateAndEditAndDelete_endpoints_are_not_authorized()
    {
        // Documents that every article mutation is currently reachable without authentication,
        // unlike CategoriesController.PostCategory where [Authorize] is commented out at line 76.
        foreach (var method in new[]
                 {
                     nameof(ArticlesController.CreateArticle),
                     nameof(ArticlesController.EditArticle),
                     nameof(ArticlesController.DeleteArticle)
                 })
        {
            typeof(ArticlesController)
                .GetMethod(method)!
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Should().BeEmpty($"{method} is currently unauthenticated");
        }
    }

    private static EditArticleDto NewEditDto() => new()
    {
        Title = "Paivitetty",
        Slug = "paivitetty",
        Content = "uusi sisalto",
        HeroImage = "/Uploads/x.jpg"
    };
}
