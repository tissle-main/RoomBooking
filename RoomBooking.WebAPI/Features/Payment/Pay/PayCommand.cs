using ErrorOr;
using Mediator;

namespace RoomBooking.WebAPI.Features.Payment.Pay;

public sealed record class PayCommand(decimal Price, string Currency) : ICommand<ErrorOr<Unit>>;