using ErrorOr;
using Mediator;
using RoomBooking.WebAPI.Features.Payment.Services;

namespace RoomBooking.WebAPI.Features.Payment.Pay;

public sealed class PayHandler(IPaymentService thisPaymentService) : ICommandHandler<PayCommand, ErrorOr<Unit>>
{
    #region ICommandHandler<PayCommand, ErrorOr<Unit>>
    public async ValueTask<ErrorOr<Unit>> Handle(PayCommand command, CancellationToken cancellationToken)
    {
        return await thisPaymentService.PayAsync(command.Price, command.Currency, cancellationToken);
    }
    #endregion
}