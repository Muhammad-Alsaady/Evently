using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Evently.Modules.Events.Domain.Categories;
using Evently.Modules.Events.Domain.Events;

namespace Evently.Modules.Events.Infrastructure.Database.Configurations;

internal sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).HasMaxLength(300).IsRequired();

        builder.Property(e => e.Description).HasMaxLength(2000).IsRequired();

        builder.Property(e => e.Location).HasMaxLength(500);

        builder.Property(e => e.StartsAtUtc).IsRequired();

        builder.Property(e => e.EndsAtUtc);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .IsRequired();

        // FK constraint to categories — no navigation property (cross-aggregate reference by ID)
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
