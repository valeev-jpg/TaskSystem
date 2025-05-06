using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskSystem.DataAccessLayer.Utils;
using TaskSystem.Domain.Models;

namespace TaskSystem.DataAccessLayer.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(ConfigurationUtils.RemoveSuffix(Constants.EntityPrefix, nameof(User)));
        builder.HasKey(user => user.Id);
        builder.HasMany(user => user.RefreshTokens)
            .WithOne(rt => rt.User).HasForeignKey(rt => rt.UserId);

    }
}