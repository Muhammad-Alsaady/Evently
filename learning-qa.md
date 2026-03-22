# Learning Q&A — Modular Monolith / Evently Build

---

## Step 1 — Entity, DomainEvent, IDomainEvent (Common.Domain)

**Q: Why does `Entity` hold `_domainEvents` as a `private` list but expose it as `IReadOnlyList`?**
A: Encapsulation — but the real motivation is domain integrity. Domain events are facts: they record something that already happened inside the aggregate. Only the aggregate itself decides when and what to raise. External code (handlers, repositories, interceptors) may only read and consume them, never inject or remove them. A public `List<IDomainEvent>` would allow external code to add fake events, remove events before dispatch, or reorder them — all of which would corrupt the domain model.

**Q: Why is `Raise()` protected but `ClearDomainEvents()` is public?**
A: `Raise()` is protected because only the aggregate itself should decide when something happened — this enforces DDD's rule that domain logic stays inside the aggregate. `ClearDomainEvents()` is public because the infrastructure (EF interceptor) or application layer needs to clear the list after dispatching events, to prevent double-publishing on subsequent SaveChanges calls.

---

## Step 2 — IQuery / IQueryHandler (Common.Application)

**Q: Why does `IQuery<TResponse>` wrap its response in `Result<TResponse>` instead of returning `TResponse` directly?**
A: Two reasons. First, to unify all responses under the Result type. Second — and more importantly — `Result<TResponse>` forces the caller to explicitly handle failure. Queries can fail for real reasons: record not found, access denied, invalid filter. Returning `TResponse` directly leads to `null` checks or exceptions for failure paths — both are worse. `Result` makes the error path visible in the type system and impossible to ignore.

---

## Step 4 — RequestLoggingPipelineBehavior (Common.Application)

**Q: Why is the behavior `internal sealed` instead of `public`?**
A: `internal` — it's an implementation detail of Common.Application. No module should reference or instantiate it directly; it gets registered into DI and Waseet picks it up automatically. Making it `public` would expose an internal mechanism as part of the public API surface. `sealed` — pipeline behaviors have one job and should never be inherited. If you need different logging logic, register a different behavior. `sealed` also prevents accidental inheritance and enables minor JIT optimizations via devirtualization.

**Q: Can the ExceptionHandlingPipelineBehavior be used to log Result failures?**
A: No. `ExceptionHandlingPipelineBehavior` catches thrown exceptions — unexpected crashes. `RequestLoggingPipelineBehavior` reports on `Result` errors — controlled, expected failures (not found, validation failed, etc.). These are two completely different failure modes and must not be mixed.

---

## Step 6 — PublishDomainEventsInterceptor (Common.Infrastructure)

**Q: Why does the interceptor use `IServiceScopeFactory` instead of injecting `IEventPublisher` directly?**
A: The interceptor is registered as a Singleton (EF Core requires it). `IEventPublisher` and its handlers are Scoped. Injecting a Scoped service into a Singleton is the **Captive Dependency** problem: the Scoped service gets resolved once at startup and reused for the entire application lifetime — far outliving its intended per-request scope. This causes stale state, shared mutable state across requests, and concurrency bugs. `IServiceScopeFactory` is itself a Singleton. You use it to create a fresh scope on demand, resolve `IEventPublisher` within that scope, publish, then dispose the scope. Each call is clean and isolated.

**Q: Why clear domain events before publishing, not after?**
A: To prevent double-publishing. If publishing fails halfway through and you retry, events that were already cleared won't fire again. Clearing after would risk re-publishing events if anything after `ClearDomainEvents()` threw an exception before the list was cleared.

**Q: Why is `(dynamic)` needed when calling `PublishAsync(domainEvent)`?**
A: `domainEvent` is typed as `IDomainEvent`. `EventPublisher.PublishAsync<TEvent>` is generic — it resolves `IEventHandler<TEvent>` using the exact runtime type of `TEvent`. Without `(dynamic)`, the compiler resolves `TEvent` as `IDomainEvent` and the publisher looks for `IEventHandler<IDomainEvent>` — which has no handlers. `(dynamic)` forces runtime dispatch on the concrete type (e.g., `EventCreatedDomainEvent`), so the correct `IEventHandler<EventCreatedDomainEvent>` is resolved.

**Q: Why create the scope outside the loop, not inside?**
A: Creating a scope inside the loop allocates and disposes a new DI scope per event. If an aggregate raises 5 events, you get 5 scopes. One scope per `SaveChanges` call is sufficient — all events in the same transaction belong to the same logical operation and can share the same scope.

---

## Events Module Scaffold

**Q: Why does `PublicApi` reference nothing?**
A: `PublicApi` is a contract project — it contains only interfaces and DTOs that other modules are allowed to use to call into this module. If it referenced `Application` or `Domain`, any consuming module would transitively pull in all the internals of the Events module, creating tight coupling. `PublicApi` must stay thin and dependency-free. The actual implementation lives in `Infrastructure` (e.g. `EventsApi : IEventsApi`). Consuming modules reference only `PublicApi` — they depend on the contract, not the implementation. This is Dependency Inversion applied at the module boundary level.

---

## Step 5 — DbConnectionFactory (Common.Infrastructure)

**Q: Why is `DbConnectionFactory` registered as `Singleton` and not `Scoped`?**
A: Because it's stateless — it holds only a `string connectionString`. Every call to `OpenConnectionAsync()` creates a brand new `NpgsqlConnection`, so there's no shared state and no concurrency risk. Registering it as `Scoped` would create a new identical factory instance per HTTP request — wasted allocations with zero benefit. Rule: if a service holds no mutable state, register it as Singleton.

**Q: Why use a factory delegate `_ => new DbConnectionFactory(connectionString)` instead of `AddSingleton<IDbConnectionFactory, DbConnectionFactory>()`?**
A: `DbConnectionFactory` takes a raw `string` in its constructor. The DI container cannot resolve a primitive type — it would fail trying to inject it. The factory delegate gives you full control: resolve the connection string from configuration yourself, then construct the object manually.

---

## Step 3 — IDbConnectionFactory (Common.Application)

**Q: Why does `IDbConnectionFactory` return `IDbConnection` (System.Data) instead of `NpgsqlConnection`?**
A: `NpgsqlConnection` is PostgreSQL-specific. Using it in the interface would couple the entire Application layer — and every query handler in every module — to PostgreSQL. Switching databases would require touching every query handler. `IDbConnection` is the abstraction: Application says "give me something I can run queries on" without caring whether it's Postgres, SQL Server, or SQLite. Only the Infrastructure layer knows it's Npgsql. This is the Dependency Inversion Principle — high-level modules should not depend on low-level modules; both should depend on abstractions.
