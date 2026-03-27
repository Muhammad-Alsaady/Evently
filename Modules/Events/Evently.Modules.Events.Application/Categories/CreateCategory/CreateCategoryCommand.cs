using Evently.Common.Application.Messaging;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Events.Application.Categories.CreateCategory;

[InvalidateCache("events:categories")]
public sealed record CreateCategoryCommand(string Name) : ICommand<Guid>;
