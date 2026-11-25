using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RoomBooking.Data.Entities.RefreshTokens;

public sealed class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    #region IEntityTypeConfiguration<RefreshTokenEntity>
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Value).IsRequired().HasMaxLength(RefreshTokenEntityConstraints.ValueMaxLength);
        builder.Property(rt => rt.ExpiresAt).IsRequired();
        builder.HasOne(rt => rt.User).WithMany(u => u.RefreshTokens).HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
    }
    #endregion
}