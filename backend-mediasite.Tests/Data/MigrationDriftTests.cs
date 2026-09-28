using MediaSite_backend.Data;
using MediaSite_backend.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MediaSite_backend.Tests.Data;

/// <summary>
/// These tests exercise the checked-in migration chain rather than the EF model. They are kept
/// apart from the repository suites because a failure here means a fresh deployment cannot be
/// created, not that a query is wrong.
/// </summary>
[Collection(DatabaseCollection.Name)]
[Trait("Category", "Integration")]
public class MigrationDriftTests
{
    private const string ScratchDatabase = "mediasite_migration_probe";

    private readonly PostgresFixture _fixture;

    public MigrationDriftTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Bug_AllMigrations_apply_against_an_empty_database()
    {
        // The migration chain cannot build a working schema. 20260911075229_initialcreate creates
        // NewsletterSubscribers with a Status column, then 20260922165129_testing tries to drop a
        // column named IsActive that initialcreate never created and re-add Status, which
        // initialcreate already created. Npgsql raises 42703 and `dotnet ef database update`
        // aborts, so nobody can provision a new environment from migrations.
        //
        // Fix by deleting 20260922165129_testing: the snapshot already describes a Status column,
        // so initialcreate alone already matches the entity model.
        await CreateScratchDatabaseAsync();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_fixture.ConnectionStringFor(ScratchDatabase))
            .Options;

        await using var context = new ApplicationDbContext(options);

        await context.Database.MigrateAsync();

        var columns = await DescribeColumnsAsync(context, "NewsletterSubscribers");
        columns.Should().Contain("Status");
        columns.Should().NotContain("IsActive");
    }

    private static async Task<List<string>> DescribeColumnsAsync(ApplicationDbContext context, string table)
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT column_name FROM information_schema.columns WHERE table_name = @table";
        var tableParameter = command.CreateParameter();
        tableParameter.ParameterName = "table";
        tableParameter.Value = table;
        command.Parameters.Add(tableParameter);

        var columns = new List<string>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            columns.Add(reader.GetString(0));
        }

        return columns;
    }

    private async Task CreateScratchDatabaseAsync()
    {
        var admin = new NpgsqlConnectionStringBuilder(_fixture.ConnectionString) { Database = "postgres" };
        await using var connection = new NpgsqlConnection(admin.ConnectionString);
        await connection.OpenAsync();

        await using var exists = new NpgsqlCommand(
            "SELECT 1 FROM pg_database WHERE datname = @name", connection);
        exists.Parameters.AddWithValue("name", ScratchDatabase);

        if (await exists.ExecuteScalarAsync() is null)
        {
            await using var create = new NpgsqlCommand($"CREATE DATABASE \"{ScratchDatabase}\"", connection);
            await create.ExecuteNonQueryAsync();
        }
    }
}
