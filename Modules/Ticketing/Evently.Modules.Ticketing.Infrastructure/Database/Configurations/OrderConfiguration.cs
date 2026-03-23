using Evently.Modules.Ticketing.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evently.Modules.Ticketing.Infrastructure.Database.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
		builder.ToTable("orders");

		builder.HasKey(o => o.Id);

		builder.Property(o => o.TotalPrice).HasColumnType("numeric(10,2)");
		builder.Property(o => o.OrderStatus).HasConversion<string>();

		builder.HasMany<OrderItem>("OrderItems")
			.WithOne()
			.HasForeignKey(i => i.OrderId);
	}
}
