namespace RoboForge.AppHost;

internal static class PassAspNetCoreEnvironmentResourceBuilderExtensions
{
    public static IResourceBuilder<T> WithAspNetCoreEnvironment<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEnvironment
    {
        builder.WithEnvironment(context =>
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            context.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = environment ?? "Development";
        });

        return builder;
    }
}
