using MediaSite_backend.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace MediaSite_backend.Tests.Common;

/// <summary>
/// A single throwaway Postgres per test collection. Repositories are tested against a real
/// database on purpose: every query they issue (AnyAsync, Select projections, FindAsync) is
/// translated to SQL by the provider, so an in-memory provider would execute them client-side
/// and pass while the real Npgsql SQL fails.
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17")
        .WithDatabase("mediasite_test")
        .WithUsername("mediasite_test")
        .WithPassword("mediasite_test_password")
        .Build();

    private DbContextOptions<ApplicationDbContext>? _options;

    public string ConnectionString => _container.GetConnectionString();

    /// <summary>Connection string for a scratch database on the same server, used by the migration tests.</summary>
    public string ConnectionStringFor(string database) =>
        new Npgsql.NpgsqlConnectionStringBuilder(ConnectionString) { Database = database }.ConnectionString;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        // EnsureCreated, not MigrateAsync. The checked-in migration chain cannot build a working
        // schema: 20260922165129_testing drops a column "IsActive" that initialcreate never created
        // and re-adds "Status", which initialcreate already created. MigrateAsync therefore throws
        // on an empty database, so the repository tests build the schema from the current model
        // instead. MigrationDriftTests is the red test that keeps the migration bug visible.
        await using var context = new ApplicationDbContext(_options);
        await context.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public ApplicationDbContext CreateContext() => new(_options ?? throw new InvalidOperationException("Fixture not initialised."));

    /// <summary>Empties every table so each test starts from a known-empty database.</summary>
    public async Task ResetAsync()
    {
        await using var context = CreateContext();

        await context.Database.ExecuteSqlRawAsync(
            """
            TRUNCATE TABLE
                "Articles",
                "Categories",
                "NewsletterSubscribers",
                "AspNetUsers",
                "AspNetRoles",
                "AspNetUserRoles",
                "AspNetUserClaims",
                "AspNetUserLogins",
                "AspNetUserTokens",
                "AspNetRoleClaims"
            RESTART IDENTITY CASCADE;
            """);
    }
}

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class DatabaseCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "Database";
}
