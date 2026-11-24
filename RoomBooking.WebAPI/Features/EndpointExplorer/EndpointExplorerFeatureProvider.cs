using Scalar.AspNetCore;

namespace RoomBooking.WebAPI.Features.EndpointExplorer;

public sealed class EndpointExplorerFeatureProvider : FeatureProvider
{
    #region Base
    public override void AddServices(WebApplicationBuilder builder)
    {
        if(builder.Environment.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi();
        }
    }
    public override void UseMiddleware(WebApplication app)
    {
        if(app.Environment.IsDevelopment())
        {         
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", typeof(EndpointExplorerFeatureProvider).Assembly.FullName);
            });
        }
    }
    public override void MapEndpoints(WebApplication app)
    {
        if(app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }
    }
    #endregion
}