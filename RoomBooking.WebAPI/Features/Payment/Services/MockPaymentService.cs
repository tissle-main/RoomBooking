using ErrorOr;
using Mediator;

namespace RoomBooking.WebAPI.Features.Payment.Services;

public sealed class MockPaymentService : IPaymentService
{
    #region IPaymentService
    public ValueTask<ErrorOr<Unit>> PayAsync(decimal price, string currency, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Unit.Value.ToErrorOr());
    }
    #endregion
}