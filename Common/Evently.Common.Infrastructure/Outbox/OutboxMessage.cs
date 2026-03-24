namespace Evently.Common.Infrastructure.Outbox;

public sealed class OutboxMessage
{
	public Guid Id { get; set; }
	public DateTime OccurredOnUtc { get; set; }
	public string Type { get; set; } = default!;
	public string Content { get; set; } = default!;
	public DateTime? ProcessedAt { get; set; }  
	public string? Error { get; set; }
}
