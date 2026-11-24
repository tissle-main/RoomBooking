namespace RoomBooking.WebAPI.Features;

public abstract class FeatureProvider
{
    public virtual void AddServices(WebApplicationBuilder builder)
    {

    }
    public virtual void UseMiddleware(WebApplication app)
    {

    }
    public virtual void MapEndpoints(WebApplication app)
    {

    }
}