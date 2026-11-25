using RoomBooking.Data.Entities.Bookings;

namespace RoomBooking.Data.Entities.Rooms;

public sealed class RoomEntity
{
    //Value properties
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = "";
    public bool IsFree { get; set; }

    //Navigation properties
    public List<BookingEntity> Bookings { get; set; } = [];
}