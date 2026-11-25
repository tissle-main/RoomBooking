using ErrorOr;
using Mediator;

namespace RoomBooking.WebAPI.Features.Payment.Services;

public interface IPaymentService
{
    public abstract ValueTask<ErrorOr<Unit>> PayAsync(decimal price, string currency, CancellationToken cancellationToken);
}