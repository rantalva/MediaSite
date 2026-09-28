using MediaSite_backend.Models.Dtos.NewsletterSubscriber;
using MediaSite_backend.Models.Entities;
using MediaSite_backend.Repositories.NewsletterSubscriberRepository;
using MediaSite_backend.Tests.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaSite_backend.Tests.Controllers;

public class NewsletterSubscribersControllerTests
{
    private readonly INewsLetterSubscriberRepository _repo = Substitute.For<INewsLetterSubscriberRepository>();

    private NewsletterSubscribersController CreateController() =>
        new NewsletterSubscribersController(_repo).WithStubbedUrlHelper();

    [Fact]
    public void TestAuth_returns_Ok_for_an_admin_and_is_guarded_by_the_Admin_role()
    {
        var result = CreateController().TestAuth();

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be("You are authenticated.");

        var authorize = typeof(NewsletterSubscribersController)
            .GetMethod(nameof(NewsletterSubscribersController.TestAuth))!
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .Cast<AuthorizeAttribute>()
            .Single();

        authorize.Roles.Should().Be(ApplicationUserRoles.Admin);
    }

    private static NewsletterSubscriber NewSubscriber(string email = "lukija@example.com") => new()
    {
        Id = Guid.NewGuid(),
        Email = email,
        Status = NewsletterSubscriberStatus.Active
    };

    [Fact]
    public async Task GetNewsletterSubscriber_returns_Ok_with_all_subscribers()
    {
        var expected = new[] { NewSubscriber(), NewSubscriber("toinen@example.com") };
        _repo.GetNewsletterSubscribersAsync()
            .Returns(Task.FromResult<IEnumerable<NewsletterSubscriber>>(expected));

        var result = await CreateController().GetNewsletterSubscriber();

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetNewsletterSubscriber_returns_BadRequest_when_repository_returns_null()
    {
        _repo.GetNewsletterSubscribersAsync()
            .Returns(Task.FromResult<IEnumerable<NewsletterSubscriber>>(null!));

        var result = await CreateController().GetNewsletterSubscriber();

        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task GetNewsletterSubscriberById_returns_Ok_when_found()
    {
        var subscriber = NewSubscriber();
        _repo.GetNewsletterSubscriberIdAsync(subscriber.Id).Returns(Task.FromResult(subscriber));

        var result = await CreateController().GetNewsletterSubscriber(subscriber.Id);

        // This action returns the entity directly instead of wrapping in Ok(), so the implicit
        // ActionResult<T> conversion populates Value and leaves Result null. ASP.NET turns that
        // into a 200 ObjectResult at runtime. Asserting on Result would fail.
        result.Result.Should().BeNull();
        result.Value.Should().BeSameAs(subscriber);
    }

    [Fact]
    public async Task GetNewsletterSubscriberById_returns_NotFound_when_missing()
    {
        _repo.GetNewsletterSubscriberIdAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult<NewsletterSubscriber>(null!));

        var result = await CreateController().GetNewsletterSubscriber(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostNewsletterSubscriber_returns_CreatedAtAction_on_success()
    {
        var created = NewSubscriber();
        _repo.AddNewsletterSubscriberAsync(Arg.Any<NewsletterSubscriberDto>())
            .Returns(Task.FromResult(created));

        var result = await CreateController()
            .PostNewsletterSubscriber(new NewsletterSubscriberDto { Email = created.Email });

        result.Result.Should().BeOfType<CreatedAtActionResult>()
            .Which.ActionName.Should().Be("GetNewsletterSubscriber");
    }

    [Fact]
    public async Task PostNewsletterSubscriber_returns_BadRequest_when_email_already_subscribed()
    {
        _repo.AddNewsletterSubscriberAsync(Arg.Any<NewsletterSubscriberDto>())
            .Returns(Task.FromResult<NewsletterSubscriber>(null!));

        var result = await CreateController()
            .PostNewsletterSubscriber(new NewsletterSubscriberDto { Email = "duplikaatti@example.com" });

        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task DeleteNewsletterSubscriber_returns_NoContent_when_existed()
    {
        _repo.DeleteNewsletterSubscriberAsync(Arg.Any<Guid>()).Returns(Task.FromResult(true));

        var result = await CreateController().DeleteNewsletterSubscriber(Guid.NewGuid());

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteNewsletterSubscriber_returns_BadRequest_not_NotFound_when_missing()
    {
        // Inconsistent with ArticlesController.DeleteArticle and CategoriesController.DeleteCategory,
        // which both return 404 for a missing row. Locked in so changing it is a deliberate diff.
        _repo.DeleteNewsletterSubscriberAsync(Arg.Any<Guid>()).Returns(Task.FromResult(false));

        var result = await CreateController().DeleteNewsletterSubscriber(Guid.NewGuid());

        result.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [InlineData(NewsletterSubscriberStatus.Active)]
    [InlineData(NewsletterSubscriberStatus.InActive)]
    [InlineData(NewsletterSubscriberStatus.RemovalRequested)]
    public async Task UpdateStatus_returns_NoContent_for_each_valid_status(NewsletterSubscriberStatus status)
    {
        _repo.EditNewsletterSubscriberStatusAsync(Arg.Any<Guid>(), Arg.Any<NewsletterSubscriberStatusDto>())
            .Returns(Task.FromResult(NewSubscriber()));

        var result = await CreateController()
            .UpdateStatus(Guid.NewGuid(), new NewsletterSubscriberStatusDto { Status = status });

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateStatus_returns_NotFound_when_subscriber_missing()
    {
        _repo.EditNewsletterSubscriberStatusAsync(Arg.Any<Guid>(), Arg.Any<NewsletterSubscriberStatusDto>())
            .Returns(Task.FromResult<NewsletterSubscriber>(null!));

        var result = await CreateController()
            .UpdateStatus(Guid.NewGuid(), new NewsletterSubscriberStatusDto());

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateEmail_returns_NoContent_when_subscriber_found()
    {
        _repo.EditNewsletterSubscriberEmailAsync(Arg.Any<Guid>(), Arg.Any<NewsletterSubscriberDto>())
            .Returns(Task.FromResult(NewSubscriber()));

        var result = await CreateController()
            .UpdateEmail(Guid.NewGuid(), new NewsletterSubscriberDto { Email = "uusi@example.com" });

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateEmail_returns_NotFound_when_subscriber_missing()
    {
        _repo.EditNewsletterSubscriberEmailAsync(Arg.Any<Guid>(), Arg.Any<NewsletterSubscriberDto>())
            .Returns(Task.FromResult<NewsletterSubscriber>(null!));

        var result = await CreateController()
            .UpdateEmail(Guid.NewGuid(), new NewsletterSubscriberDto { Email = "uusi@example.com" });

        result.Should().BeOfType<NotFoundResult>();
    }
}
