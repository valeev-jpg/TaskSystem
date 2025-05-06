using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskSystem.DataAccessLayer.Utils;
using TaskSystem.Domain.Models;

namespace TaskSystem.DataAccessLayer.EntityConfigurations;

public class TaskHistoryConfiguration : IEntityTypeConfiguration<TaskHistory>
{
    public void Configure(EntityTypeBuilder<TaskHistory> builder)
    {
        builder.ToTable(ConfigurationUtils.RemoveSuffix(Constants.EntityPrefix, nameof(TaskHistory)));
        builder.HasKey(o => o.Id);
    }
}