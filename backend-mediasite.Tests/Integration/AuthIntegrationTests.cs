using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MediaSite_backend.Data;
using MediaSite_backend.Models.Entities;
using MediaSite_backend.Tests.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediaSite_backend.Tests.Integration;

[Collection(DatabaseCollection.Name)]
[Trait("Category", "Integration")]
public class AuthIntegrationTests : IClassFixture<AuthApiFactory>
{
    private const string Password = "TestPassword123!";

    private readonly AuthApiFactory _factory;

    public AuthIntegrationTests(AuthApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Register_then_login_returns_an_access_token()
    {
        await ResetAsync();

        using var client = _factory.CreateClient();
        var email = "uusi-rekisterointi@example.com";

        var register = await client.PostAsJsonAsync("/register", new { email, password = Password });
        register.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);

        var login = await client.PostAsJsonAsync("/login", new { email, password = Password });
        login.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await login.Content.ReadFromJsonAsync<JsonElement>();
        payload.GetProperty("accessToken").GetString().Should().NotBeNullOrEmpty();
        payload.GetProperty("tokenType").GetString().Should().Be("Bearer");
    }

    [Fact]
    public async Task Login_with_a_wrong_password_is_rejected()
    {
        await ResetAsync();
        await SeedUserAsync("vaarat-tunnarit@example.com", null);

        using var client = _factory.CreateClient();
        var login = await client.PostAsJsonAsync("/login", new { email = "vaarat-tunnarit@example.com", password = "vaara salasana" });

        // MapIdentityApi answers 401 for any failed sign-in, including a wrong password and an
        // unknown address, so the two cases are indistinguishable from outside.
        login.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_article_by_slug_without_a_token_returns_Unauthorized()
    {
        await ResetAsync();
        await SeedArticleAsync();

        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/articles/syksyn-hajuvesi");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_article_by_slug_with_a_non_admin_token_returns_Forbidden()
    {
        await ResetAsync();
        await SeedArticleAsync();
        var token = await TokenForAsync("lukija@example.com", null);

        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/articles/syksyn-hajuvesi");

        // The slug route carries [Authorize(Roles = Admin)], so an authenticated non-admin is 403.
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_article_by_slug_with_an_admin_token_returns_Ok()
    {
        await ResetAsync();
        await SeedArticleAsync();
        var token = await TokenForAsync("admin@example.com", ApplicationUserRoles.Admin);

        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/articles/syksyn-hajuvesi");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var article = await response.Content.ReadFromJsonAsync<JsonElement>();
        article.GetProperty("slug").GetString().Should().Be("syksyn-hajuvesi");
    }

    [Fact]
    public async Task Get_articles_needs_no_token_at_all()
    {
        await ResetAsync();
        await SeedArticleAsync();

        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/articles");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Helpers reach the fixture's database through the host's own service provider so these tests
    // share one container and one schema with the repository suites.
    private async Task ResetAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.ExecuteSqlRawAsync(
            """
            TRUNCATE TABLE
                "Articles", "Categories", "NewsletterSubscribers",
                "AspNetUsers", "AspNetRoles", "AspNetUserRoles", "AspNetUserClaims",
                "AspNetUserLogins", "AspNetUserTokens", "AspNetRoleClaims"
            RESTART IDENTITY CASCADE;
            """);
    }

    private async Task SeedUserAsync(string email, string? role)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // UserManager, not context.Users.AddAsync: the Identity login handler resolves the user
        // with FindByEmailAsync, which matches on NormalizedEmail. Adding the row directly leaves
        // that column null and every seeded login comes back 401.
        var user = TestDataFactory.NewUser(email);
        var created = await userManager.CreateAsync(user, Password);
        created.Succeeded.Should().BeTrue(string.Join("; ", created.Errors.Select(e => e.Description)));

        if (role is not null)
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            var roleId = context.Roles.Single(r => r.Name == role).Id;
            context.UserRoles.Add(new IdentityUserRole<Guid> { UserId = user.Id, RoleId = roleId });
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedArticleAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var category = TestDataFactory.NewCategory("Style");
        var author = TestDataFactory.NewUser("kirjoittaja@example.com");
        context.Categories.Add(category);
        context.Users.Add(author);
        await context.SaveChangesAsync();

        context.Articles.Add(TestDataFactory.NewArticle("Syksyn hajuvesi", categoryId: category.Id, authorId: author.Id));
        await context.SaveChangesAsync();
    }

    private async Task<string> TokenForAsync(string email, string? role)
    {
        await SeedUserAsync(email, role);

        using var client = _factory.CreateClient();
        var login = await client.PostAsJsonAsync("/login", new { email, password = Password });
        login.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await login.Content.ReadFromJsonAsync<JsonElement>();
        return payload.GetProperty("accessToken").GetString()!;
    }
}
