using Evently.Common.Application.Messaging;
using Evently.Modules.Events.Application.Categories.GetCategory;
using Waseet.CQRS.Caching;

namespace Evently.Modules.Events.Application.Categories.GetCategories;

[Cache(Key = "events:categories", Duration = 600)]
public sealed record GetCategoriesQuery : IQuery<IReadOnlyList<CategoryResponse>>;
