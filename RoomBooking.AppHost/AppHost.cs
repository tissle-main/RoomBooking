using Aspire.Hosting.Dotnet;
using RoomBooking.ServiceDefaults;
using Arshid.Aspire.ApiDocs.Extensions;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);
//IResourceBuilder<SqlServerServerResource> sqlserver = builder.AddSqlServer(AppHostResources.SqlServer);
//IResourceBuilder<SqlServerDatabaseResource> appdb = sqlserver.AddDatabase(AppHostResources.AppDatabase);
IResourceBuilder<DotnetProjectResource> webapi = builder.AddDotnetProject(AppHostResources.WebAPI, "..\\RoomBooking.WebAPI");

webapi.WithExternalHttpEndpoints();
//webapi.WithReference(appdb).WaitFor(appdb);
webapi.WithScalar(true).WithSwagger(true).WithOpenApi(true);
await builder.Build().RunAsync();