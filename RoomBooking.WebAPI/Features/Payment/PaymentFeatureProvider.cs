using RoomBooking.WebAPI.Features.Payment.Services;

namespace RoomBooking.WebAPI.Features.Payment;

public sealed class PaymentFeatureProvider : FeatureProvider
{
    public override void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPaymentService, MockPaymentService>();
    }
}
