using MediaSite_backend.Data;
using MediaSite_backend.Models.Dtos.NewsletterSubscriber;
using MediaSite_backend.Models.Entities;
using MediaSite_backend.Repositories.NewsletterSubscriberRepository;
using MediaSite_backend.Tests.Common;
using Microsoft.EntityFrameworkCore;

namespace MediaSite_backend.Tests.Repositories;

[Collection(DatabaseCollection.Name)]
[Trait("Category", "Integration")]
public class NewsLetterSubscriberRepositoryTests
{
    private readonly PostgresFixture _fixture;

    public NewsLetterSubscriberRepositoryTests(PostgresFixture fixture) => _fixture = fixture;

    private static NewsLetterSubscriberRepository CreateRepository(ApplicationDbContext context) => new(context);

    [Fact]
    public async Task AddNewsletterSubscriberAsync_persists_the_email_as_Active()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var created = await CreateRepository(context)
            .AddNewsletterSubscriberAsync(new NewsletterSubscriberDto { Email = "lukija@example.com" });

        created.Email.Should().Be("lukija@example.com");
        created.Status.Should().Be(NewsletterSubscriberStatus.Active);

        var stored = await context.NewsletterSubscribers.SingleAsync();
        stored.Email.Should().Be("lukija@example.com");
        stored.Status.Should().Be(NewsletterSubscriberStatus.Active);
    }

    [Fact]
    public async Task AddNewsletterSubscriberAsync_returns_null_for_duplicate_email()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var repository = CreateRepository(context);

        var dto = new NewsletterSubscriberDto { Email = "duplikaatti@example.com" };
        await repository.AddNewsletterSubscriberAsync(dto);
        var duplicate = await repository.AddNewsletterSubscriberAsync(dto);

        duplicate.Should().BeNull();
        (await context.NewsletterSubscribers.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task GetNewsletterSubscribersAsync_returns_every_subscriber()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        context.NewsletterSubscribers.AddRange(
            new NewsletterSubscriber { Email = "a@example.com" },
            new NewsletterSubscriber { Email = "b@example.com", Status = NewsletterSubscriberStatus.InActive });
        await context.SaveChangesAsync();

        var subscribers = await CreateRepository(context).GetNewsletterSubscribersAsync();

        subscribers.Select(s => s.Email).Should().BeEquivalentTo(["a@example.com", "b@example.com"]);
    }

    [Fact]
    public async Task GetNewsletterSubscriberIdAsync_returns_null_for_unknown_id()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        (await CreateRepository(context).GetNewsletterSubscriberIdAsync(Guid.NewGuid())).Should().BeNull();
    }

    [Fact]
    public async Task EditNewsletterSubscriberStatusAsync_changes_and_persists_the_status()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var subscriber = new NewsletterSubscriber { Email = "lukija@example.com" };
        context.NewsletterSubscribers.Add(subscriber);
        await context.SaveChangesAsync();

        var updated = await CreateRepository(context).EditNewsletterSubscriberStatusAsync(
            subscriber.Id,
            new NewsletterSubscriberStatusDto { Status = NewsletterSubscriberStatus.RemovalRequested });

        updated!.Status.Should().Be(NewsletterSubscriberStatus.RemovalRequested);
        (await context.NewsletterSubscribers.SingleAsync()).Status
            .Should().Be(NewsletterSubscriberStatus.RemovalRequested);
    }

    [Fact]
    public async Task EditNewsletterSubscriberStatusAsync_returns_null_for_unknown_id()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var result = await CreateRepository(context).EditNewsletterSubscriberStatusAsync(
            Guid.NewGuid(),
            new NewsletterSubscriberStatusDto { Status = NewsletterSubscriberStatus.InActive });

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteNewsletterSubscriberAsync_removes_the_subscriber()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var subscriber = new NewsletterSubscriber { Email = "lukija@example.com" };
        context.NewsletterSubscribers.Add(subscriber);
        await context.SaveChangesAsync();

        (await CreateRepository(context).DeleteNewsletterSubscriberAsync(subscriber.Id)).Should().BeTrue();

        (await context.NewsletterSubscribers.AnyAsync(s => s.Id == subscriber.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteNewsletterSubscriberAsync_returns_false_for_unknown_id()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        (await CreateRepository(context).DeleteNewsletterSubscriberAsync(Guid.NewGuid())).Should().BeFalse();
    }

    [Fact]
    public async Task EditNewsletterSubscriberEmailAsync_returns_null_for_unknown_id()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var result = await CreateRepository(context)
            .EditNewsletterSubscriberEmailAsync(Guid.NewGuid(), new NewsletterSubscriberDto { Email = "x@example.com" });

        result.Should().BeNull();
    }

    [Fact]
    public async Task Bug_EditEmail_does_not_persist_the_new_address()
    {
        // NewsLetterSubscriberRepository.cs:49-61 mutates subscriber.Email and returns without
        // calling SaveChangesAsync, so PUT api/NewsletterSubscribers/{id}/email answers 204 while
        // the database keeps the old address. This test is intentionally red until the missing
        // SaveChangesAsync is added; it is the executable form of that bug report.
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var subscriber = new NewsletterSubscriber { Email = "vanha@example.com" };
        context.NewsletterSubscribers.Add(subscriber);
        await context.SaveChangesAsync();

        var returned = await CreateRepository(context)
            .EditNewsletterSubscriberEmailAsync(subscriber.Id, new NewsletterSubscriberDto { Email = "uusi@example.com" });

        // The method reports success and hands back the mutated entity...
        returned!.Email.Should().Be("uusi@example.com");

        // ...but the row still holds the original address.
        await using var verification = _fixture.CreateContext();
        var stored = await verification.NewsletterSubscribers.SingleAsync(s => s.Id == subscriber.Id);
        stored.Email.Should().Be("uusi@example.com");
    }
}
