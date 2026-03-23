using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Evently.Modules.Events.Domain.Categories;

namespace Evently.Modules.Events.Infrastructure.Database.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(300).IsRequired();

        builder.Property(c => c.IsArchived).IsRequired();
    }
}
