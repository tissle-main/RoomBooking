using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RoomBooking.Data.Entities.Rooms;

public sealed class RoomEntityConfiguration : IEntityTypeConfiguration<RoomEntity>
{
    #region IEntityTypeConfiguration<RoomEntity>
    public void Configure(EntityTypeBuilder<RoomEntity> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(RoomEntityConstaints.NameMaxLength);
        builder.Property(r => r.IsFree).IsRequired();
    }
    #endregion
}