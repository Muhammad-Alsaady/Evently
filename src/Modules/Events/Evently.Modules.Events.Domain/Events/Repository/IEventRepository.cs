﻿using Evently.Modules.Events.Domain.Events.Models;
namespace Evently.Modules.Events.Domain.Events.Repository;

public interface IEventRepository
{
    Task<Event?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task Insert(Event @event);
}
