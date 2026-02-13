
IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> databaseProvider = builder
    .AddPostgres("postgres", port: 26991)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(false)
    .WithDataVolume("geonrage-pgsql")
    .WithPgWeb(pgWeb => pgWeb
        .WithHostPort(5050)
        .WithLifetime(ContainerLifetime.Persistent)
        .WithEndpointProxySupport(false)
    );

IResourceBuilder<PostgresDatabaseResource> database = databaseProvider
    .AddDatabase("geonrage-db-pg", "GeoNRage");

builder.AddProject<Projects.GeoNRage_Server>("app")
    .WithReference(database)
    .WaitFor(database)
    .WithUrlForEndpoint("https", url => url.DisplayText = "Application")
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar";
        url.Url += "/scalar/v1";
    });

await builder.Build().RunAsync();
