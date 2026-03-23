using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.Domain.TicketTypes;

namespace Evently.Modules.Events.Infrastructure.Database.Configurations;

internal sealed class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        builder.ToTable("ticket_types");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(300).IsRequired();

        builder.Property(t => t.Price).HasColumnType("numeric(10,2)").IsRequired();

        builder.Property(t => t.Quantity).IsRequired();

        // FK constraint to events — no navigation property (owned by event aggregate)
        builder.HasOne<Event>()
            .WithMany()
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
