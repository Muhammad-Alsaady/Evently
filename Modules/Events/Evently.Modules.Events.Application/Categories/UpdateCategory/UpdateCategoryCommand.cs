using Evently.Common.Application.Messaging;

namespace Evently.Modules.Events.Application.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid Id, string Name) : ICommand;
