using InfisicalConfiguration;

namespace MediaSite_backend.Extensions
{
    public static class InfisicalExtensions
    {
        public static WebApplicationBuilder AddInfisicalConfiguration(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddInfisical(
                new InfisicalConfigBuilder()
                    .SetProjectId("1928bcb6-ef07-48ae-b23c-133e78d7af82")
                    .SetEnvironment("dev")
                    .SetSecretPath("/")
                    .SetAuth(
                        new InfisicalAuthBuilder()
                                .SetUniversalAuth(
                                    Environment.GetEnvironmentVariable("INFISICAL_CLIENT_ID")!,
                                    Environment.GetEnvironmentVariable("INFISICAL_CLIENT_SECRET")!
                                    )
                            .Build()
                    )
                    .Build()
        );
            return builder;
        }
    }
}
