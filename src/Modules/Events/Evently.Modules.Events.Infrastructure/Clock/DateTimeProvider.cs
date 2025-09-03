using Evently.Modules.Events.Application.Abstractions.Clock;

namespace Evently.Modules.Events.Infrastructure.Clock;
internal class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; } = DateTime.Now;
}
