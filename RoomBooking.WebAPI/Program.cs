using RoomBooking.Data;
using RoomBooking.WebAPI;
using RoomBooking.ServiceDefaults;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddSqlServerDbContext<AppDbContext>(AppHostResources.AppDatabase);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = static void (ProblemDetailsContext ctx) =>
    {
        ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
    };
});
builder.Services.AddCQRS();
builder.AddFeatureServices();

WebApplication app = builder.Build();
await app.MigrateDatabaseAsync();
await app.SeedDatabaseAsync();
app.UseHttpsRedirection();
app.UseFeatureMiddlewares();
app.MapDefaultEndpoints();
app.MapFeatureEndpoints();
await app.RunAsync();