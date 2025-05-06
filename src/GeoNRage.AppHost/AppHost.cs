IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

#pragma warning disable ASPIREPROXYENDPOINTS001
IResourceBuilder<MySqlServerResource> mysql = builder
    .AddMySql("geonrage-mysql", port: 26990)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(false)
    .WithDataVolume();
#pragma warning restore ASPIREPROXYENDPOINTS001

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
