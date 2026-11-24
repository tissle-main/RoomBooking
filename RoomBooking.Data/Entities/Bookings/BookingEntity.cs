using RoomBooking.Data.Entities.Rooms;

namespace RoomBooking.Data.Entities.Bookings;

public sealed class BookingEntity
{
    //Value properties
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public bool Cancelled { get; set; }
    public Guid RoomId { get; set; }

    //Navigation properties
    public RoomEntity? Room { get; set; }
}