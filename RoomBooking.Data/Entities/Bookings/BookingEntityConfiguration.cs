using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RoomBooking.Data.Entities.Bookings;

public sealed class BookingEntityConfiguration : IEntityTypeConfiguration<BookingEntity>
{
    #region IEntityTypeConfiguration<BookingEntity>
    public void Configure(EntityTypeBuilder<BookingEntity> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title).IsRequired().HasMaxLength(BookingEntityConstaints.TitleMaxLength);
        builder.Property(b => b.StartTime).IsRequired();
        builder.Property(b => b.EndTime).IsRequired();
        builder.Property(b => b.Cancelled).IsRequired();
        builder.HasOne(b => b.Room).WithMany(r => r.Bookings).HasForeignKey(b => b.RoomId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(b => b.User).WithMany(u => u.Bookings).HasForeignKey(b => b.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(b => b.RoomId);
    }
    #endregion
}