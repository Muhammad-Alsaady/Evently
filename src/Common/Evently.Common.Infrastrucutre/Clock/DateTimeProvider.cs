using Evently.Common.Application.Clock;

namespace Evently.Common.Infrastrucutre.Clock;
internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; } = DateTime.Now;
}
