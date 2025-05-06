using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskSystem.DataAccessLayer.Utils;
using TaskSystem.Domain.Models;

namespace TaskSystem.DataAccessLayer.EntityConfigurations;

public class TaskTicketConfiguration : IEntityTypeConfiguration<TaskTicket>
{
    public void Configure(EntityTypeBuilder<TaskTicket> builder)
    {
        builder.ToTable(ConfigurationUtils.RemoveSuffix(Constants.EntityPrefix, nameof(TaskTicket)));
        builder.HasKey(o => o.Id);
        builder.HasMany(tt => tt.TaskHistory)
            .WithOne(th => th.TaskTicket).HasForeignKey(th => th.TaskTicketId);
    }
}