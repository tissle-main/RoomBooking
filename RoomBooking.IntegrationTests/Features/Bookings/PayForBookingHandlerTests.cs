using Bogus;
using System.Net;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.Data.Entities.Users;
using RoomBooking.IntegrationTests.Features.Auth;
using RoomBooking.IntegrationTests.Features.Rooms;
using RoomBooking.IntegrationTests.Seeders;
using RoomBooking.WebAPI.Features.Bookings.Dtos;
using RoomBooking.WebAPI.Features.Bookings.PayForBooking;

namespace RoomBooking.IntegrationTests.Features.Bookings;

[DependsOn<RoomSeedingTests>]
[DependsOn<LoginUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class PayForBookingHandlerTests(AppFixture thisApp)
{
    private static Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_ShouldMarkBookingPaid(CancellationToken cancellationToken)
    {
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db => await db.Rooms.Select(r => r.Id).FirstAsync(cancellationToken));
        List<BookingEntity> bookings = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<BookingEntity>().Valid(roomId).WithUserId(user.Id)
                .WithPaymentState(BookingPaymentState.PendingPayment).SeedDatabaseAsync(db, cancellationToken);
        });
        PayForBookingCommand command = new Faker<PayForBookingCommand>().Valid().WithBookingId(bookings[0].Id).Generate();

        using HttpResponseMessage message = await thisApp.HttpClient.SendPayForBooking(command, cancellationToken);

        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            BookingEntity? booking = await db.Bookings.SingleOrDefaultAsync(b => b.Id == command.BookingId, cancellationToken);
            await Assert.That(booking).IsNotNull();
            await Assert.That(booking!.PaymentState).IsEqualTo(BookingPaymentState.PaymentSucceded);
        });
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUserIsUnauthorized(CancellationToken cancellationToken)
    {
        await thisApp.ResetAsync(cancellationToken);
        PayForBookingCommand command = new Faker<PayForBookingCommand>().Valid().Generate();

        using HttpResponseMessage message = await thisApp.HttpClient.SendPayForBooking(command, cancellationToken);

        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenBookingNotFound(CancellationToken cancellationToken)
    {
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        PayForBookingCommand command = new Faker<PayForBookingCommand>().Valid().Generate();

        using HttpResponseMessage message = await thisApp.HttpClient.SendPayForBooking(command, cancellationToken);

        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenBookingBelongsToAnotherUser(CancellationToken cancellationToken)
    {
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity User, string Password)[] users = (await thisApp.AddUsers2Async(cancellationToken, min: 2, max: 2)).ToArray();
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db => await db.Rooms.Select(r => r.Id).FirstAsync(cancellationToken));
        List<BookingEntity> bookings = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<BookingEntity>().Valid(roomId).WithUserId(users[0].User.Id)
                .WithPaymentState(BookingPaymentState.PendingPayment).SeedDatabaseAsync(db, cancellationToken, min: 1, max: 1);
        });
        await thisApp.AuthenticateAsAsync(users[1].User, users[1].Password, cancellationToken);
        PayForBookingCommand command = new Faker<PayForBookingCommand>().Valid().WithBookingId(bookings[0].Id).Generate();

        using HttpResponseMessage message = await thisApp.HttpClient.SendPayForBooking(command, cancellationToken);

        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
    }

    [Test]
    [Arguments(BookingPaymentState.NotRequired)]
    [Arguments(BookingPaymentState.PaymentSucceded)]
    public async ValueTask Handler_ShouldFail_WhenBookingCannotBePaid(BookingPaymentState paymentState, CancellationToken cancellationToken)
    {
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db => await db.Rooms.Select(r => r.Id).FirstAsync(cancellationToken));
        List<BookingEntity> bookings = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<BookingEntity>().Valid(roomId).WithUserId(user.Id)
                .WithPaymentState(paymentState).SeedDatabaseAsync(db, cancellationToken, min: 1, max: 1);
        });
        PayForBookingCommand command = new Faker<PayForBookingCommand>().Valid().WithBookingId(bookings[0].Id).Generate();

        using HttpResponseMessage message = await thisApp.HttpClient.SendPayForBooking(command, cancellationToken);

        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenBookingIsCancelled(CancellationToken cancellationToken)
    {
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db => await db.Rooms.Select(r => r.Id).FirstAsync(cancellationToken));
        List<BookingEntity> bookings = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<BookingEntity>().Valid(roomId).WithUserId(user.Id)
                .WithPaymentState(BookingPaymentState.PendingPayment).Cancelled().SeedDatabaseAsync(db, cancellationToken, min: 1, max: 1);
        });
        PayForBookingCommand command = new Faker<PayForBookingCommand>().Valid().WithBookingId(bookings[0].Id).Generate();

        using HttpResponseMessage message = await thisApp.HttpClient.SendPayForBooking(command, cancellationToken);

        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPriceIsNegative(CancellationToken cancellationToken)
    {
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        PayForBookingCommand command = new Faker<PayForBookingCommand>().Valid().WithNegativePrice().Generate();

        using HttpResponseMessage message = await thisApp.HttpClient.SendPayForBooking(command, cancellationToken);

        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }
}
