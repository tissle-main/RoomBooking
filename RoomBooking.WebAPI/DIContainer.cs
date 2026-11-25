using FluentValidation;
using RoomBooking.Data;
using System.Collections.Frozen;
using RoomBooking.WebAPI.Features;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Rooms;
using RoomBooking.WebAPI.Features.Auth;
using RoomBooking.WebAPI.Features.Rooms;
using RoomBooking.WebAPI.Features.Serilog;
using RoomBooking.WebAPI.Features.Bookings;
using RoomBooking.WebAPI.Behaviors.Validation;
using RoomBooking.WebAPI.Features.EndpointExplorer;

namespace RoomBooking.WebAPI;

public static class DIContainer
{
    public static FrozenSet<FeatureProvider> Features { get; } = [
        new SerilogFeatureProvider(),
        new AuthFeatureProvider(),
        new EndpointExplorerFeatureProvider(),
        new RoomFeatureProvider(),
        new BookingFeatureProvider()
    ];

    public static void AddCQRS(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
        });
        services.AddValidatorsFromAssemblyContaining(typeof(DIContainer), ServiceLifetime.Singleton);
        ValidatorOptions.Global.LanguageManager.Enabled = false;
    }
    public static void AddFeatureServices(this WebApplicationBuilder builder)
    {
        foreach(FeatureProvider feature in Features)
        {
            feature.AddServices(builder);
        }
    }
    public static void UseFeatureMiddlewares(this WebApplication app)
    {
        foreach(FeatureProvider feature in Features)
        {
            feature.UseMiddleware(app);
        }
    }
    public static void MapFeatureEndpoints(this WebApplication app)
    {
        foreach(FeatureProvider feature in Features)
        {
            feature.MapEndpoints(app);
        }
    }
    public static async ValueTask MigrateDatabaseAsync(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
    public static async ValueTask SeedDatabaseAsync(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        if(!await dbContext.Rooms.AnyAsync())
        {
            await dbContext.Rooms.AddRangeAsync(
                new RoomEntity()
                {
                    Name = "Room 1"
                },
                new RoomEntity()
                {
                    Name = "Room 2"
                },
                new RoomEntity()
                {
                    Name = "Room 3"
                }
            );
        }
        await dbContext.SaveChangesAsync();
    }
}