using Evently.Common.Application.Messaging;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Events.Application.Categories.ArchiveCategory;

[InvalidateCache("events:category:{Id}")]
[InvalidateCache("events:categories")]
public sealed record ArchiveCategoryCommand(Guid Id) : ICommand;

