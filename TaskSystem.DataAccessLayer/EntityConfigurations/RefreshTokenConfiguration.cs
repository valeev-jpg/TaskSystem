using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskSystem.DataAccessLayer.Utils;
using TaskSystem.Domain.Models;

namespace TaskSystem.DataAccessLayer.EntityConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable(ConfigurationUtils.RemoveSuffix(Constants.EntityPrefix, nameof(RefreshToken)));
        builder.HasKey(rt => rt.Id);
    }
}