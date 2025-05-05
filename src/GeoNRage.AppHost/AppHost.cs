IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<MySqlServerResource> mysql = builder
    .AddMySql("geonrage-mysql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

IResourceBuilder<MySqlDatabaseResource> mysqldb = mysql.AddDatabase("geonrage-db", "GeoNRage");

builder.AddProject<Projects.GeoNRage_Server>("app")
    .WithReference(mysqldb)
    .WaitFor(mysqldb)
    .WithUrlForEndpoint("https", url => url.DisplayText = "Application")
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar";
        url.Url += "/scalar/v1";
    });

await builder.Build().RunAsync();
