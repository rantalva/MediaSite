using Cloudflare.NET.R2;
using MediaSite_backend.Tests.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MediaSite_backend.Tests.Integration;

/// <summary>
/// Boots the real application against the Testcontainers Postgres.
///
/// Program.cs reads its connection string while the host is being constructed, which is before
/// WebApplicationFactory's ConfigureAppConfiguration hook can contribute anything, so the values
/// have to be in the process environment before CreateClient triggers the build. Setting them here
/// also happens to sidestep the key mismatch at Program.cs:32, which asks for
/// "PostgreSqlConnection" while appsettings.json defines "ApplicationDbContext".
///
/// These variables are process-global, which is why the whole suite runs with
/// parallelizeTestCollections disabled in xunit.runner.json.
/// </summary>
public sealed class AuthApiFactory : WebApplicationFactory<Program>
{
    private readonly PostgresFixture _fixture;

    public AuthApiFactory(PostgresFixture fixture)
    {
        _fixture = fixture;

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("ConnectionStrings__PostgreSqlConnection", fixture.ConnectionString);

        // AddCloudflareR2Client validates two options sections at startup: "Cloudflare" for the
        // account and "R2" for the API token, while StorageService reads the bucket from
        // "R2:BucketName". All three have to be populated for the host to build, even though
        // nothing in this suite calls out to Cloudflare.
        Environment.SetEnvironmentVariable("Cloudflare__AccountId", "test-account");
        Environment.SetEnvironmentVariable("Cloudflare__ApiUrl", "https://api.cloudflare.com/client/v4");
        Environment.SetEnvironmentVariable("R2__AccessKeyId", "test-access-key");
        Environment.SetEnvironmentVariable("R2__SecretAccessKey", "test-secret-key");
        Environment.SetEnvironmentVariable("R2__ApiUrl", "https://api.cloudflare.com/client/v4/r2");
        Environment.SetEnvironmentVariable("R2__BucketName", "test-bucket");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // The upload endpoint resolves IR2Client at construction time. Nothing in this suite calls
        // Cloudflare, so a substitute keeps the tests offline while still satisfying DI.
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IR2Client>();
            services.AddSingleton(Substitute.For<IR2Client>());
        });
    }
}
