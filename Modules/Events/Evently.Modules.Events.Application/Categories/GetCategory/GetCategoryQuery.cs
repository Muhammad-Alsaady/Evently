using Evently.Common.Application.Messaging;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Events.Application.Categories.GetCategory;

[Cache(Key = "events:category:{CategoryId}", Duration = 600)]
public sealed record GetCategoryQuery(Guid CategoryId) : IQuery<CategoryResponse>;
