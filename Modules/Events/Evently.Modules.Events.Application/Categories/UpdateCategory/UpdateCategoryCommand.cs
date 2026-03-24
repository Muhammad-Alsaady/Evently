using Evently.Common.Application.Messaging;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Events.Application.Categories.UpdateCategory;

[InvalidateCache("events:category:{Id}")]
[InvalidateCache("events:categories")]
public sealed record UpdateCategoryCommand(Guid Id, string Name) : ICommand;
