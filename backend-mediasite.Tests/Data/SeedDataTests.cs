using MediaSite_backend.Data;
using MediaSite_backend.Models.Entities;
using MediaSite_backend.Repositories.ArticleRepository;
using MediaSite_backend.Repositories.CategoryRepository;
using MediaSite_backend.Repositories.NewsletterSubscriberRepository;
using MediaSite_backend.Tests.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Slugify;

namespace MediaSite_backend.Tests.Data;

[Collection(DatabaseCollection.Name)]
[Trait("Category", "Integration")]
public class SeedDataTests
{
    private readonly PostgresFixture _fixture;

    public SeedDataTests(PostgresFixture fixture) => _fixture = fixture;

    private static ServiceProvider BuildServices(PostgresFixture fixture)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(fixture.ConnectionString));
        services
            .AddIdentityApiEndpoints<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddSingleton<SlugHelper>();
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<INewsLetterSubscriberRepository, NewsLetterSubscriberRepository>();
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Runs SeedData and tolerates the failure of its final step, the seeded article insert.
    /// SeedData.cs hardcodes a CategoryId and an AuthorId that do not match the rows it creates, so
    /// that insert violates a foreign key and the exception escapes InitializeAsync. Everything
    /// before it is already committed, which is what the other four tests in this class assert.
    /// Bug_SeededArticle_references_a_category_that_does_not_exist is the test that reports the bug.
    /// </summary>
    private static async Task SeedToleratingArticleFailureAsync(IServiceProvider provider)
    {
        try
        {
            await SeedData.InitializeAsync(provider);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation })
        {
        }
    }

    [Fact]
    public async Task InitializeAsync_creates_the_four_categories()
    {
        await _fixture.ResetAsync();
        await using var provider = BuildServices(_fixture);

        await SeedToleratingArticleFailureAsync(provider);

        await using var context = _fixture.CreateContext();
        var names = await context.Categories.Select(c => c.Name).ToListAsync();
        names.Should().BeEquivalentTo(["Style", "Shopping", "Culture", "Sports"]);
    }

    [Fact]
    public async Task InitializeAsync_creates_the_four_roles()
    {
        await _fixture.ResetAsync();
        await using var provider = BuildServices(_fixture);

        await SeedToleratingArticleFailureAsync(provider);

        await using var context = _fixture.CreateContext();
        var roles = await context.Roles.Select(r => r.Name!).ToListAsync();
        roles.Should().BeEquivalentTo(
            [ApplicationUserRoles.Admin, ApplicationUserRoles.Editor, ApplicationUserRoles.Author, ApplicationUserRoles.Member]);
    }

    [Fact]
    public async Task InitializeAsync_creates_the_two_seed_users_with_the_expected_roles()
    {
        await _fixture.ResetAsync();
        await using var provider = BuildServices(_fixture);

        await SeedToleratingArticleFailureAsync(provider);

        await using var context = _fixture.CreateContext();
        (await context.Users.CountAsync()).Should().Be(2);

        var admin = await context.UserRoles
            .Where(ur => ur.UserId == context.Users.Single(u => u.Email == "test@gmail.com").Id)
            .Select(ur => ur.RoleId)
            .ToListAsync();
        var adminRoleId = await context.Roles.Where(r => r.Name == ApplicationUserRoles.Admin).Select(r => r.Id).SingleAsync();
        admin.Should().ContainSingle().Which.Should().Be(adminRoleId);

        var author = await context.UserRoles
            .Where(ur => ur.UserId == context.Users.Single(u => u.Email == "alvari.rantapelkonen@gmail.com").Id)
            .Select(ur => ur.RoleId)
            .ToListAsync();
        var authorRoleId = await context.Roles.Where(r => r.Name == ApplicationUserRoles.Author).Select(r => r.Id).SingleAsync();
        author.Should().ContainSingle().Which.Should().Be(authorRoleId);
    }

    [Fact]
    public async Task InitializeAsync_is_idempotent()
    {
        // SeedData runs on every application start, so running it twice must not duplicate rows
        // or fail. This is the single most valuable test in the suite: a regression here breaks
        // production boot rather than one request.
        await _fixture.ResetAsync();
        await using var provider = BuildServices(_fixture);

        await SeedToleratingArticleFailureAsync(provider);
        await SeedToleratingArticleFailureAsync(provider);

        await using var context = _fixture.CreateContext();
        (await context.Categories.CountAsync()).Should().Be(4);
        (await context.Roles.CountAsync()).Should().Be(4);
        (await context.Users.CountAsync()).Should().Be(2);
    }

    [Fact]
    public async Task Bug_SeededArticle_references_a_category_that_does_not_exist()
    {
        // SeedData.cs:116 hardcodes CategoryId 6425fd8a-... but the four categories it just created
        // got fresh Guid.NewGuid() values from Category's property initialiser, and SeedData.cs:12
        // hardcodes _testAuthorGuid for an author that Identity also generated. The article insert
        // therefore violates both foreign keys and the whole seeding routine aborts, which takes
        // application startup with it. Intentionally red until the hardcoded ids are replaced with
        // the ids of the rows the routine just created.
        await _fixture.ResetAsync();
        await using var provider = BuildServices(_fixture);

        var act = () => SeedData.InitializeAsync(provider);

        await act.Should().NotThrowAsync();

        await using var context = _fixture.CreateContext();
        (await context.Articles.CountAsync()).Should().Be(1);
    }
}
