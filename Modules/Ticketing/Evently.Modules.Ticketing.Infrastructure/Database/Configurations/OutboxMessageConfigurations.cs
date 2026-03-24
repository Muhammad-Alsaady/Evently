using Evently.Common.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evently.Modules.Ticketing.Infrastructure.Database.Configurations;

internal class OutboxMessageConfigurations : IEntityTypeConfiguration<OutboxMessage>
{
	public void Configure(EntityTypeBuilder<OutboxMessage> builder)
	{
		builder.ToTable("outbox_messages");
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Type).IsRequired();
		builder.Property(x => x.Content).IsRequired();

		builder.Property(x => x.OccurredOnUtc).IsRequired();

	}
}
