using Respawn;
using Projects;
using TUnit.Aspire;
using RoomBooking.Data;
using Microsoft.Data.SqlClient;
using RoomBooking.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

namespace RoomBooking.IntegrationTests;

public sealed class AppFixture : AspireFixture<RoomBooking_AppHost>
{
    #region Instance
    private string ConnectionString { get; set; } = null!; //Init after InitializedAsync
    private DbContextOptions<AppDbContext> DbOptions { get; set; } = null!; //Init after InitializedAsync
    private Respawner Respawner { get; set; } = null!; //Init after InitializedAsync
    private Uri? WebBaseAddress { get; set; }
    public HttpClient HttpClient { get; private set; } = null!; //Init after InitializedAsync

    public async ValueTask ExecuteDbContextAsync(Func<AppDbContext, ValueTask> func)
    {
        await using AppDbContext context = new(DbOptions);
        await func(context);
    }
    public async ValueTask<T> ExecuteDbContextAsync<T>(Func<AppDbContext, ValueTask<T>> func)
    {
        await using AppDbContext context = new(DbOptions);
        return await func(context);
    }
    public async ValueTask ResetAsync(CancellationToken cancellationToken)
    {
        HttpClient?.Dispose();
        HttpClient = new HttpClient()
        {
            BaseAddress = WebBaseAddress
        };

        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await Respawner.ResetAsync(connection);
    }
    #endregion

    #region Base
    protected override TimeSpan ResourceTimeout
    {
        get => TimeSpan.FromMinutes(10);
    }
    protected override AspireFixtureOptions Options
    {
        get => field ??= new AspireFixtureOptions()
        {
            ForwardResourceLogs = true
        };
    }

    public override async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("DOTNET_LAUNCH_PROFILE", Environments.Testing);
        await base.InitializeAsync();

        WebBaseAddress = base.CreateHttpClient(AppHostResources.WebAPI).BaseAddress;
        HttpClient = new HttpClient()
        {
            BaseAddress = WebBaseAddress
        };

        ConnectionString = await base.GetConnectionStringAsync(AppHostResources.AppDatabase) ?? throw new NullReferenceException("ConnectionString is null");
        DbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(ConnectionString).Options;
        await ExecuteDbContextAsync(async db =>
        {
            await db.Database.MigrateAsync(base.RunCancellationToken);
        });

        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync(base.RunCancellationToken);
        Respawner = await Respawner.CreateAsync(connection, new RespawnerOptions()
        {
            DbAdapter = DbAdapter.SqlServer,
            TablesToIgnore = [
                "__EFMigrationsHistory",
                nameof(AppDbContext.Rooms)
            ]
        });
    }
    public override async ValueTask DisposeAsync()
    {
        await ResetAsync(base.RunCancellationToken);
        await base.DisposeAsync();
    }
    #endregion
}