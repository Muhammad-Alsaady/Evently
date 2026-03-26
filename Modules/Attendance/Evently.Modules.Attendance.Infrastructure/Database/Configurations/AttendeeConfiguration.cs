using Evently.Modules.Attendance.Domain.Attendees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evently.Modules.Attendance.Infrastructure.Database.Configurations;

internal sealed class AttendeeConfiguration : IEntityTypeConfiguration<Attendee>
{
    public void Configure(EntityTypeBuilder<Attendee> builder)
    {
        builder.ToTable("attendees");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.OrderId).IsRequired();
        builder.Property(a => a.CustomerId).IsRequired();
        builder.Property(a => a.EventId).IsRequired();
        builder.Property(a => a.CreatedAtUtc).IsRequired();
    }
}
