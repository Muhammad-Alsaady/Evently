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

## CreateCategory Command

**Q: Why does `CreateCategoryCommand` return `Guid` instead of a full `CategoryResponse` DTO?**
A: Commands should return the minimum needed for the caller to proceed. The caller (endpoint) only needs the ID to return a 201 Created with a Location header. Returning a full DTO from a command would require the handler to either re-query the database after saving (extra round trip) or map domain objects to DTOs — neither belongs in the write path. If the caller needs the full object, they issue a separate query using the returned ID.

**Q: Why does `ICommandHandler` live in `Common.Application` and not be defined per-module?**
A: It's a shared contract over Waseet's `IRequestHandler`. Every module's command handlers implement the same pattern — defining it once in Common avoids duplication and ensures all handlers are consistent across modules.

---

## Events Application Layer

**Q: Why does each module define its own `IUnitOfWork` instead of sharing one from `Common`?**
A: All modules use the same PostgreSQL instance but each has its own DbContext (EventsDbContext, TicketingDbContext, etc.). If IUnitOfWork lived in Common and was shared, it would imply one DbContext for the whole application — saving an Event could accidentally flush pending Ticketing changes in the same transaction. Each module's IUnitOfWork maps to its own DbContext only. Rule: one IUnitOfWork per module = one DbContext per module = one schema per module = true isolation.

**Q: Why does `SaveChangesAsync` return `Task<int>` and not `Task`?**
A: EF Core's `SaveChangesAsync` returns the number of state entries written to the database. This is useful for detecting unexpected behavior — if you expected to save 1 entity and got 0 back, something silently failed. It also mirrors EF Core's own API, making the implementation straightforward. Returning `Task` would discard this diagnostic information.

---

## Events Domain

**Q: Why do aggregates reference other aggregates by ID (Guid) instead of object reference?**
A: Core DDD principle — aggregates reference each other by ID only. If Event held a Category object reference, you could call event.Category.Archive() from inside Event — one aggregate mutating another, which DDD forbids. Also prevents accidental lazy loading (EF Core loading Category whenever Event is loaded). DB relations are NOT broken — EF Core creates the FK constraint via EntityTypeConfiguration using the Guid property. Rule: within an aggregate use object references; across aggregates use IDs.

**Q: Why is `IRepository.Insert()` synchronous while `GetAsync()` is async?**
A: Insert only calls DbSet.Add(entity) — registers the entity in EF Core's change tracker in memory. No DB call happens yet. The actual INSERT SQL runs later when SaveChangesAsync() is called on the Unit of Work. GetAsync hits the database to fetch a record, so it must be async.

**Q: Why does `Reschedule()` only work when the event is Published, not Draft?**
A: Rescheduling implies attendees already know about the event. You can't reschedule something that hasn't been announced — a Draft event has no audience to notify. Rescheduling a Draft makes no semantic sense.

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

---

## Events Infrastructure Layer

**Q: Why does `EventsDbContext` call `modelBuilder.HasDefaultSchema("events")` instead of prefixing each table name?**
A: `HasDefaultSchema` sets the PostgreSQL schema for all tables in this DbContext. Instead of writing `"events.categories"`, `"events.events"`, etc. on every table configuration, you set it once. This is the schema isolation mechanism — each module owns its own PostgreSQL schema so their tables never collide, even if names overlap across modules.

**Q: Why does `EventConfiguration` use `builder.HasOne<Category>()` (no lambda) instead of `builder.HasOne(e => e.Category)`?**
A: `Event` doesn't have a `Category` navigation property — it only holds `CategoryId` (a Guid). The `HasOne<Category>()` overload with no lambda tells EF Core: "there's a FK relationship to the `categories` table, but don't create a navigation property for it". This enforces the DB constraint without coupling the `Event` aggregate to the `Category` aggregate in C#. Cross-aggregate references are always by ID only — no navigation properties across aggregate boundaries.

**Q: Why is `UnitOfWork` just a thin wrapper over `DbContext.SaveChangesAsync`?**
A: The Application layer defines `IUnitOfWork` — application code depends on an abstraction, not on EF Core. The Infrastructure layer implements it. Command handlers call `IUnitOfWork.SaveChangesAsync()` without knowing they're talking to EF Core. If you ever swap EF Core out, no command handler changes. The thin wrapper is intentional — the real logic lives in EF Core's change tracker, not here.

**Q: Why is `PublishDomainEventsInterceptor` changed from `internal` to `public`?**
A: The interceptor lives in `Common.Infrastructure` but each module's `DbContext` needs to use it. If it's `internal`, only code inside `Common.Infrastructure` can reference the type by name — the Events module can't resolve it from DI by type. Making it `public` lets each module register it as a Singleton and inject it into their DbContext options via `provider.GetRequiredService<PublishDomainEventsInterceptor>()`.

**Q: Why register `EventsDbContext` using the `(provider, options)` factory overload instead of just `options =>`?**
A: Because we need to resolve `PublishDomainEventsInterceptor` from the DI container. The `provider` parameter gives access to already-registered services. With just `options =>` you have no way to get other services. This is the same pattern the Users module uses for its `AuditableInterceptor`.

**Q: Why register the interceptor as `Singleton` and not `Scoped`?**
A: `SaveChangesInterceptor` is stateless — it holds no per-request data. EF Core also requires interceptors to outlive the `DbContext`. If the interceptor were `Scoped`, EF Core could receive a disposed interceptor when it tries to use it. Additionally, the interceptor uses `IServiceScopeFactory` internally (a Singleton) to create its own scope on demand — so it safely handles Scoped services without being Scoped itself.

**Q: Why do repositories need to be `Scoped` and not `Singleton`?**
A: Repositories hold a reference to `EventsDbContext`, which is `Scoped` (one per HTTP request). If a repository were a Singleton, it would capture the first `DbContext` instance at startup and hold it forever — sharing one DbContext across all requests. This would cause shared change tracker state, incorrect behavior, and concurrency bugs. Rule: if a service depends on a Scoped service, it must also be Scoped or shorter-lived.

**Q: Why register pipeline behaviors with `typeof(IPipelineBehavior<,>)` as an open generic?**
A: Pipeline behaviors are generic — `ValidationPipelineBehavior<TRequest, TResponse>` works for any request type. Registering with an open generic tells DI: "whenever someone needs `IPipelineBehavior<X, Y>`, close this generic type with X and Y". You can't pre-register every concrete combination (there would be hundreds). Open generic registration lets the container do the closing at resolution time.

**Q: What's the registration order of pipeline behaviors and why does it matter?**
A: Order matters — they execute like nested middleware, first registered = outermost wrap. Correct order: `ExceptionHandling` (outermost — catches anything thrown inside) → `RequestLogging` (logs the full round-trip including validation failures) → `Validation` (innermost before the handler — blocks bad requests before any domain logic runs). If Validation were outermost, ExceptionHandling wouldn't catch validation exceptions. If Logging were innermost, you'd miss logging validation failures.

---

## Events Presentation Layer

**Q: Why does Presentation reference Application directly instead of going through Infrastructure?**
A: Presentation only needs the command/query/response types to construct requests and map responses — those live in Application. It doesn't need DB access, repositories, or EF Core. Referencing Infrastructure would pull in all persistence dependencies into a layer that has no business touching them. Rule: depend only on what you actually use.

**Q: Why use `IMediator.Send()` in endpoints instead of injecting the handler directly (like the Users module does)?**
A: The Users module injects specific handler interfaces (`IGetUserByIdHandler`) which is a valid pattern for simple cases but couples the endpoint type to a specific handler. `IMediator` decouples the endpoint from any specific handler type — the endpoint just sends a message and doesn't know who handles it. More importantly, `IMediator` is the entry point for the entire pipeline (validation, logging, exception handling). Bypassing it means those pipeline behaviors don't run.

**Q: Why do commands that don't return a value (Publish, Cancel, Archive) return `Results.Ok()` with no body, while commands that create resources return `Results.Created()`?**
A: REST conventions. `201 Created` indicates a resource was created and includes a `Location` header pointing to the new resource — the client needs the ID to find it. `200 OK` indicates an operation succeeded with no new resource to point to. Using `201` for publish/cancel would be semantically wrong — you're not creating anything new, you're changing the state of an existing resource.

**Q: Why are request DTOs (`CreateCategoryRequest`, `CreateEventRequest`, etc.) defined as `internal sealed record` in the same file as the endpoint?**
A: They're presentation-layer contracts — only the endpoint file uses them to bind the HTTP request body. There's no reason to expose them to other assemblies or even other files. Keeping them co-located with the endpoint that uses them is the vertical slice principle: everything related to one feature lives together. If the request shape changes, you change one file.

---

## Events PublicApi

**Q: Why does `PublicApi` have no project references at all (no Domain, no Application)?**
A: `PublicApi` is a pure contract library — it contains only interfaces and DTOs. Any module that wants to call into Events depends on `PublicApi`, not on `Application` or `Infrastructure`. If `PublicApi` referenced `Application`, every consuming module would transitively pull in all of Events' internal dependencies (EF Core, FluentValidation, domain logic). The contract must be dependency-free so it stays thin and stable. The implementation (`EventsApi`) lives in Infrastructure and is the only place that knows about the internals.

**Q: Why does `IEventsApi` use simple DTOs (`EventResponse`, `TicketTypeResponse`) instead of returning the same DTOs from the Application layer?**
A: The Application layer's DTOs (`Application.Events.GetEvent.EventResponse`) are internal to that module — they can contain fields that are irrelevant or even sensitive for other modules. `PublicApi` DTOs are deliberately minimal: they expose only what other modules actually need. This is the principle of least privilege applied to data contracts. If Ticketing only needs `Id`, `Title`, and `Status`, those are the only fields in the PublicApi DTO.

**Q: Why is `IEventsApi` implemented in Infrastructure, not Application?**
A: The implementation needs to query the database (Dapper + `IDbConnectionFactory`). Application has no knowledge of databases — it only defines business logic. Infrastructure is the correct layer for anything that touches external systems. The interface (`IEventsApi`) is in PublicApi (dependency-free), the implementation (`EventsApi`) is in Infrastructure (knows about Dapper/Postgres), and consumers only ever depend on the interface. This is Dependency Inversion across module boundaries.

**Q: Why register `IEventsApi` as `Scoped` and not `Singleton`?**
A: `EventsApi` takes `IDbConnectionFactory` in its constructor. `IDbConnectionFactory` is a Singleton (stateless factory). However, `EventsApi` creates a new DB connection per call (`OpenConnectionAsync()`), making each method call independent. Registering as Scoped is the safe, conventional choice — it aligns with the lifetime of the request scope and avoids any potential issues with shared state if the implementation ever grows.

---

## EF Core Migrations

**Q: Why does `dotnet ef migrations` fail without `IDesignTimeDbContextFactory`?**
A: At design time, EF Core tools try to instantiate your `DbContext` by starting the application and resolving it from DI — but this requires a real connection string which doesn't exist at design time. `IDesignTimeDbContextFactory<T>` is EF Core's escape hatch: if found in the assembly, it uses it instead of starting the full app. The factory provides a hardcoded local connection string just for migration purposes. It has no effect at runtime.

**Q: Why use `--project`, `--startup-project`, and `--context` flags?**
A: `--project` tells EF where the `DbContext` and migration files live (Infrastructure). `--startup-project` tells EF which project to build to find the factory (the API, which has all references). `--context` is required when multiple `DbContext` types exist in scope — in a modular monolith each module has its own, so EF needs to know which one to migrate.

**Q: Why is the migration history table named `_migrations` scoped to the `events` schema?**
A: By default EF stores history in `public.__EFMigrationsHistory`. In a modular monolith with multiple modules sharing one database, all modules would write to the same table — creating conflicts. Scoping it to each module's schema (`events._migrations`, `ticketing._migrations`) gives each module its own isolated migration history and lets each module manage its schema independently.

---

## Why create `AddApplicationBehaviors()` extension in `Common.Application` instead of registering the behaviors directly from the module's DI?
A: Pipeline behaviors are `internal sealed` in `Common.Application`. Internal types are only visible within their own assembly — the Events Infrastructure project is a different assembly and cannot reference them by name. The pattern is: behaviors stay `internal` (implementation detail, not public API), but `AddApplicationBehaviors()` is `public` and lives in the same assembly, so it can reference them. Modules call the extension without ever knowing the concrete behavior types. This is the same reason we put `AddDbConnectionFactory` inside `Common.Infrastructure` rather than letting callers construct `DbConnectionFactory` directly.

---

## Cross-Module Integration (Events → Ticketing)

**Q: What is the difference between a Domain Event and an Integration Event?**
A: A domain event is private to one module — `EventPublishedDomainEvent` is raised inside Events.Domain and consumed only by handlers inside the Events module. An integration event is a public contract between modules — `EventPublishedIntegrationEvent` lives in Events.PublicApi so any module can consume it. Think of it as: domain event = internal memo; integration event = official announcement.

**Q: Why does `EventPublishedIntegrationEvent` live in `Events.PublicApi` and not `Events.Domain`?**
A: Because other modules (Ticketing, Attendance) need to reference it to subscribe. If it lived in Events.Domain, those modules would have to reference Events.Domain — which would leak the entire Events domain model to them. PublicApi is a thin contract layer: it only exposes what's needed for inter-module communication, with no business logic inside.

**Q: Why does `EventPublishedIntegrationEvent` implement `IEvent`?**
A: Our `IEventPublisher.PublishAsync<TEvent>` has the constraint `where TEvent : IEvent`. Since we dispatch integration events through the same `IEventPublisher` mechanism (resolving `IEventHandler<T>` from DI), the integration event must satisfy that constraint. The `IEvent` marker is in `Common.Domain`, so `Events.PublicApi` now references `Common.Domain` — that's its only dependency.

**Q: Why does the `EventPublishedDomainEventHandler` live in `Events.Application` rather than `Events.Infrastructure`?**
A: The domain event handler's job is pure orchestration: "When an event is published, create and dispatch the integration event." That's use-case logic, which belongs in Application. Infrastructure is for I/O (database, HTTP, file system). The handler doesn't do I/O — it reads from the repository (already injected) and calls `IEventPublisher`. Both are abstractions. Infrastructure would be appropriate only if we needed something like writing to an outbox table or calling an HTTP webhook directly.

**Q: The integration event handler (`EventPublishedIntegrationEventHandler`) is in Ticketing.Infrastructure, not Ticketing.Application. Why?**
A: Because it has an infrastructure concern: it receives an external event (something that arrived from another module via the in-process bus). The Application layer only knows about commands and domain concepts — it shouldn't reference `Events.PublicApi` directly. Infrastructure is the wiring layer that bridges the external world to Application. It translates `EventPublishedIntegrationEvent` → `CreateEventCommand` and dispatches it to the Application via `IMediator`. The Application never knows a cross-module event existed.

**Q: Why does Ticketing create its own `Event` entity (a local read model) instead of querying `events.events` directly?**
A: Module isolation. Each module must be fully autonomous and own its data. If Ticketing queried `events.events`, it would:
1. Be coupled to Events module's schema — any rename or column change in Events breaks Ticketing.
2. Have to join across schemas at runtime, coupling deployment concerns.
3. Violate the "share nothing" rule of modular design.
Instead, Ticketing mirrors the data it needs (title, dates, location) in `ticketing.events`. The `EventPublishedIntegrationEvent` is the source of truth for what gets copied.

**Q: How does the full integration event chain work from HTTP call to Ticketing database?**
A:
1. HTTP POST `events/{id}/publish` → `PublishEventCommandHandler` calls `event.Publish()` → raises `EventPublishedDomainEvent` → `SaveChangesAsync()`
2. `PublishDomainEventsInterceptor` intercepts `SaveChanges`, reads domain events from the entity
3. Calls `IEventPublisher.PublishAsync<EventPublishedDomainEvent>()` → resolves `EventPublishedDomainEventHandler` from DI
4. Handler queries the event from `events.events`, creates `EventPublishedIntegrationEvent`, calls `IEventPublisher.PublishAsync<EventPublishedIntegrationEvent>()`
5. `EventPublisher` resolves `IEventHandler<EventPublishedIntegrationEvent>` from DI → finds `EventPublishedIntegrationEventHandler` in Ticketing.Infrastructure
6. Handler dispatches `CreateEventCommand` via `IMediator`
7. `CreateEventCommandHandler` in Ticketing.Application creates `Ticketing.Domain.Event` and saves to `ticketing.events`

**Q: Why doesn't Ticketing have a `PublishDomainEventsInterceptor` registered? Won't that cause issues?**
A: The interceptor IS registered in Ticketing — it's `AddSingleton<PublishDomainEventsInterceptor>()` in `TicketingModuleExtensions.AddDatabase()`. But the Ticketing `Event` entity has no domain events (it's a read model, not an aggregate that does business logic). So the interceptor will fire on `TicketingDbContext.SaveChanges` but find zero domain events and do nothing. The interceptor is registered defensively — if we later add domain events to Ticketing aggregates (e.g., `CustomerCreatedDomainEvent`), it'll work automatically.

---

## Why Ticketing Mirrors the Event Entity (Local Read Model, Decoupling, DDD)

**Q: Ticketing already knows the EventId. Why does it need to store Title, Location, StartsAtUtc locally? Can't it just call `IEventsApi.GetAsync(eventId)` whenever it needs those fields?**
A: It *can*, but it shouldn't rely on it for every read. If Ticketing calls `IEventsApi.GetAsync()` for every order confirmation and receipt, it creates **runtime coupling** — Ticketing's availability depends on Events module working correctly. If Events is slow, buggy, or later moved to a separate service, Ticketing breaks too. The mirror says: "I only need your data once — when you publish. After that, I own my copy."

**Q: But if Events updates the event title later, Ticketing's copy becomes stale. Isn't that a problem?**
A: This is the core DDD trade-off called **eventual consistency** vs **strong consistency**. In DDD, each Bounded Context owns its data. Ticketing's `Event` represents *"the event as it was when tickets went on sale"* — a valid business fact. If the event is rescheduled, Events fires `EventRescheduledIntegrationEvent` and Ticketing updates its copy. The rule is: **stale data is acceptable if the business accepts it**. If staleness matters, handle it with another integration event — not by making Ticketing query Events at runtime.

**Q: What DDD concept does this mirror implement exactly?**
A: It's a **Local Read Model** inside a Bounded Context. `events.events` is the authoritative model — it has all the business rules and state transitions. `ticketing.events` is a **projection** — a simplified, denormalized snapshot of what Ticketing needs to do its job. Ticketing doesn't need `CategoryId`, `Status`, or any Events-domain concept. Its shape is driven by Ticketing's own needs.

**Q: The Ticketing `Event` class has no business logic right now. Is it really a DDD entity or just a database row?**
A: Right now it's closer to a Value Snapshot. But that's intentional — it starts minimal and gains behavior as Ticketing grows: `Cancel()` blocks new ticket sales, `Reschedule()` updates dates, `SoldOut()` tracks capacity. Each of those behaviors lives in Ticketing's entity, driven by Ticketing's business rules — not Events'. The model's shape and behavior are driven by the context it lives in, not by another module's model.

**Q: What is a Bounded Context, and how does the mirror pattern enforce the boundary?**
A: A Bounded Context is a boundary within which a model has a specific, consistent meaning. The word "Event" means two different things:
- Events module: a thing being organized — has status, categories, ticket types, lifecycle rules
- Ticketing module: a reference to what a ticket is for — has display info, dates, and availability state

Without the mirror, if Ticketing referenced `Events.Domain.Event` directly: a column rename in Events breaks Ticketing's queries, Events' business rules leak into Ticketing, and both modules must be deployed together. The mirror enforces the boundary: **Ticketing only knows what Events chose to announce publicly** via the integration event contract.

**Q: When should a module mirror data vs call another module's API?**
A: Mirror when the data is needed on every read (orders, receipts, ticket display) — calling an API for every read is too slow and creates runtime coupling. Call the API when data is needed only for rare one-time validation (e.g., "does this EventId exist before I create a ticket?"). Mirror + handle update events when the data can change after the initial sync. Never mirror when the data must always be real-time accurate.

---

## Guided Feature: EventCancelled Cross-Module Integration

### What we built and why

We implemented the full chain for propagating an event cancellation from the Events module into the Ticketing module. This is the same pattern as EventPublished but with an important difference: instead of *creating* data in Ticketing, we're *updating* existing data — which introduced new concerns.

**The chain:**
```
POST events/{id}/cancel
  → CancelEventCommandHandler (Events.Application)
  → event.Cancel() raises EventCancelledDomainEvent
  → SaveChanges() triggers PublishDomainEventsInterceptor
  → EventCancelledDomainEventHandler (Events.Application)
  → EventCancelledIntegrationEvent (Events.PublicApi)
  → EventCancelledIntegrationEventHandler (Ticketing.Infrastructure)
  → CancelEventCommand → CancelEventCommandHandler (Ticketing.Application)
  → ticketing.Event.Cancel() → SaveChanges()
```

**Q: Why does `EventCancelledDomainEventHandler` NOT query the repository, while `EventPublishedDomainEventHandler` does?**
A: Because the contracts are different. `EventPublishedIntegrationEvent` carries the full event details (Title, Location, StartsAtUtc etc.) — data that only exists in `events.events`. The domain event only carries `EventId`, so the handler must query to get the rest. `EventCancelledIntegrationEvent` only carries `EventId` — which is already on the domain event. No query needed. The rule: only do the extra work (repository call) when the integration event contract requires data you don't already have.

**Q: Why does `CancelEventCommand` need to be `public` when `CreateEventCommandHandler` is `internal`?**
A: The command is constructed in Ticketing.Infrastructure (inside `EventCancelledIntegrationEventHandler`) and consumed by a handler in Ticketing.Application — two different assemblies. `internal` types are invisible outside their assembly. The handler can be `internal sealed` because IMediator resolves it via reflection at runtime (it doesn't need to reference the type by name). But the command is constructed by name with `new CancelEventCommand(...)`, so it must be `public`.

**Q: Why does `CancelEventCommandHandler` check the `Cancel()` result before saving?**
A: Because `Cancel()` can return an error — specifically `EventErrors.AlreadyCancelled`. If we called `SaveChanges()` without checking, EF would try to persist a no-op (the entity didn't change) and the caller would get `200 OK` on a double-cancel instead of a `409 Conflict`. The pattern is always: call domain method → check result → only persist if success.

**Q: Why does `Ticketing.Domain.Event` have `IsCancelled` (a bool flag) rather than a `Status` enum like Events.Domain.Event?**
A: Because Ticketing's model is shaped by Ticketing's needs, not Events'. Ticketing only cares about one question: "can I sell tickets for this event?" A bool is the simplest representation of that. Events.Domain needs a full status enum (`Draft`, `Published`, `Cancelled`) because it manages the event lifecycle. Ticketing doesn't manage the lifecycle — it just reacts to it. Simpler model = less code to maintain.

**Q: What does `sealed` on a class give us, and why does it matter for handlers?**
A: `sealed` tells the compiler and JIT that no subclass exists. For handlers, this means:
1. The class is never meant to be extended — it's a single-purpose implementation.
2. The JIT can devirtualize method calls on sealed types, giving a minor performance benefit.
3. It signals intent to the reader: "this is a leaf class, don't inherit from it."
All command handlers and event handlers in this codebase should be `sealed` by convention.

---

## Phase 3 — Infrastructure Concerns

---

## Serilog + Seq

**Q: What is structured logging, and why is it better than plain text logging?**
A: Plain text logging writes human-readable strings: `"User 123 placed order 456"`. Structured logging writes events as data: `{ "UserId": 123, "OrderId": 456, "Event": "OrderPlaced" }`. The difference matters when you need to query logs. With plain text you can only search by string pattern — fragile and slow. With structured logs you can run real queries: "show me all orders placed by UserId 123 in the last hour where TotalPrice > 100". Seq is a log server that accepts structured events and lets you run exactly these queries. In a modular monolith with multiple modules and background workers all writing logs, the ability to filter by `Module`, `RequestId`, or `OrderId` is the difference between debugging in 5 minutes vs 5 hours.

**Q: What is Serilog and how does it work in this project?**
A: Serilog is a .NET structured logging library. Instead of `ILogger.LogInformation("msg")` writing a raw string, Serilog captures the message template and property values separately: `Log.Information("Order {OrderId} created for {CustomerId}", orderId, customerId)` stores `OrderId` and `CustomerId` as searchable properties on the log event. In this project, `appsettings.json` configures two sinks (output targets): Console (for local development) and Seq (for structured query). The `Enrich` section adds automatic properties to every event: `MachineName` (which server), `ThreadId` (useful for async debugging), and `FromLogContext` (lets you push extra properties with `LogContext.PushProperty`). The `AddCoreHostLogging()` call in `Program.cs` reads this config and wires Serilog as the default `ILogger` provider.

**Q: What are Serilog enrichers and why do we use them?**
A: Enrichers automatically attach extra properties to every log event without you having to pass them manually. `WithMachineName` adds the server hostname — essential when running multiple instances. `WithThreadId` adds the OS thread ID — helps trace async operations across thread switches. `FromLogContext` enables scoped enrichment: when a request comes in, you push `RequestId`, `UserId`, and `Module` onto the context once, and every log written during that request automatically carries those properties. Without enrichers, you'd have to manually pass these fields to every single log call.

**Q: Why does Seq run separately as a Docker container instead of writing logs to a file?**
A: Log files have three problems: they're hard to query, they don't survive container restarts unless mounted to a volume, and in a multi-instance deployment each instance writes its own file. Seq solves all three: it's a centralized server that receives structured events over HTTP, stores them in a queryable database, and serves a web UI. In production you'd use a hosted service or a more scalable solution, but for local development and small deployments, Seq in Docker gives you everything: persistence (via `/data` volume mount), real-time streaming, and query power — all on port 8081 in the browser.

**Q: What is the `MinimumLevel` config and why is `Microsoft` overridden to `Information`?**
A: `MinimumLevel.Default: Debug` means our application code logs everything from Debug upward. But ASP.NET Core and EF Core (both under the `Microsoft` namespace) emit enormous amounts of Debug noise — every route matched, every SQL query parameterized, every DI scope opened. Overriding `Microsoft` to `Information` silences that noise while keeping our own Debug logs. This is a standard pattern: keep your own code verbose, silence framework internals.

**Q: How does Serilog improve performance, scalability, and observability in this project and in general?**
A:
- **Performance**: Serilog uses asynchronous, buffered sinks. The `Seq` sink batches log events and flushes them in the background — the calling thread is not blocked waiting for the log server to respond. Compared to synchronous file-writing loggers (like `log4net` in blocking mode), this means logging has near-zero impact on request latency. Destructuring (capturing structured properties) is also faster than string interpolation because no string is built unless the event actually passes the minimum level filter.
- **Scalability**: When you run multiple API instances (horizontal scaling), all instances send their structured logs to the single Seq container. Instead of SSH-ing into 5 servers to read 5 separate log files, you have one centralized query interface. Adding a new instance requires zero logging configuration — it just starts sending to Seq.
- **Observability**: By enriching every log event with `MachineName`, `RequestId`, `Module`, and `UserId`, you can trace a single user's order flow across all modules and background workers in one Seq query — even when that request touched the Events module, the Ticketing module, and the Outbox processor. Without structured enrichment, correlating logs across modules at scale is practically impossible.
- **In other projects**: Any system with more than one service or more than one instance benefits the same way. Microservices especially need centralized structured logging — a distributed trace that spans 5 services is only reconstructible if every service enriches its logs with the same correlation ID.

---

## Redis Caching

**Q: What is the difference between in-memory cache and distributed cache (Redis)?**
A: In-memory cache (`IMemoryCache`) stores data in the process's RAM. It's fast (no network hop) but has two critical limitations: it's per-process (if you run two API instances, each has its own cache — a cache miss on one instance isn't a hit on the other), and it's lost on restart (a deploy wipes the cache). Redis is a distributed cache: it runs as a separate server that all API instances connect to. Any instance that caches a value makes it available to all others. Redis persists to disk, so a restart doesn't flush it. In this project, `AddMemoryCache()` was a placeholder. For a real deployment, Redis behind `IDistributedCache` is the correct choice.

**Q: How does `ICacheService` / `CacheService` work in this project?**
A: `ICacheService` is defined in `Common.Application` — a thin abstraction with `GetAsync<T>`, `SetAsync<T>`, and `RemoveAsync`. The `CacheService` implementation in `Common.Infrastructure` wraps `IDistributedCache` (the ASP.NET Core abstraction). `IDistributedCache` is backed by Redis via `AddStackExchangeRedisCache`. The cache stores values as `byte[]` — `CacheService` handles serialization to/from JSON using `System.Text.Json`. The key design: Application layer (query handlers) depends only on `ICacheService` — it never knows whether the backend is Redis, in-memory, or a file. Swapping backends means changing one DI registration in Infrastructure.

**Q: What should be cached and what shouldn't?**
A: Cache read-heavy, rarely-changing data: event details, category lists, ticket type prices. Don't cache: user-specific data without a per-user key, data that changes on every write (cart totals), or data where staleness is unacceptable (inventory counts). The pattern in query handlers is cache-aside: check cache first → if miss, query DB → store in cache → return result. On write (command handler), invalidate the relevant cache key so the next read fetches fresh data.

**Q: What is a cache key strategy and why does it matter?**
A: A cache key uniquely identifies a cached value. Bad keys cause bugs: if `GetEventAsync(eventId1)` and `GetEventAsync(eventId2)` both cache under key `"event"`, they overwrite each other. Good keys embed all parameters: `$"events:{eventId}"`, `$"tickettypes:event:{eventId}"`. In a modular monolith with multiple modules, prefix with the module name to avoid collisions: `"ticketing:event:123"` vs `"events:event:123"` are different cached values for the same entity ID but from different modules (different read models). The `CacheOptions` class sets expiration — short for volatile data (1 minute), longer for stable data (1 hour).

**Q: Why is the class named `CashService` in the codebase?**
A: It's a typo — it should be `CacheService`. This is a naming bug carried over from the template. It doesn't affect functionality since DI resolves by interface (`ICacheService`), but it should be renamed to `CacheService` for clarity. We'll fix it when we wire up Redis properly in P3-2.

**Q: How does Redis improve performance and scalability in this project and in general?**
A:
- **Performance — this project**: Every `GET /events/{id}` query currently hits PostgreSQL. With caching, the first request queries the DB and stores the result in Redis (e.g., for 5 minutes). Every subsequent request for the same event in that window returns the cached result in ~1ms (Redis is an in-memory key-value store) instead of ~20–50ms for a DB round-trip. Under load — say 500 users all viewing the same popular event — that's 499 DB queries eliminated per cache window. Read-heavy endpoints (event listings, ticket type prices) benefit most.
- **Performance — DB protection**: Redis acts as a shield in front of PostgreSQL. Without caching, a traffic spike (e.g., a popular event goes on sale) sends thousands of identical queries to the DB simultaneously — the DB saturates, latency spikes, requests time out. With Redis, those identical queries are absorbed at the cache layer. The DB only sees the initial miss. This pattern is called **cache-aside** and it's the most direct way to reduce DB load without scaling the DB itself.
- **Scalability**: Because Redis is external and shared, it scales horizontally with the application. When you add a second API instance, it reads from the same Redis cache — cache hits from instance A are immediately usable by instance B. In-memory cache provides zero benefit when you add instances (each instance has its own empty cache on startup). Redis also supports clustering and replication natively, so the cache layer itself can scale independently of the API.
- **In other projects**: Redis is used for session storage (stateless APIs sharing user sessions), rate limiting (distributed counters for API throttle), leaderboard/ranking (sorted sets), and pub/sub messaging. Any read-heavy system where the same data is read frequently by many users benefits from Redis in front of the primary data store.

---

## Outbox Pattern

**Q: What problem does the Outbox pattern solve?**
A: The current `PublishDomainEventsInterceptor` dispatches domain events immediately after `SaveChanges`. This creates a window for data loss: SaveChanges succeeds (DB write committed) → crash → domain event never dispatched. The order is placed in the DB but `OrderCreatedDomainEvent` never fires, so `OrderCreatedIntegrationEvent` never publishes, so no other module knows the order exists. The Outbox closes this window by making event publishing atomic with the DB write: instead of dispatching events immediately, the interceptor writes them as rows in an `outbox_messages` table in the same transaction. Even if the app crashes, the events are safely stored. A background worker reads and dispatches them later.

**Q: How does the Outbox work step by step?**
A:
```
[Request — same DB transaction]
  1. SaveChanges() commits business data (e.g., new Order row)
  2. Interceptor writes domain events to outbox_messages table
     → { Id, Type, Content (JSON), OccurredAt, ProcessedAt = null }
  Both steps are in the same transaction — they succeed or fail together.

[Background worker — separate process loop]
  3. Worker polls outbox_messages WHERE processed_at IS NULL
  4. For each pending event: deserialize → dispatch to IEventPublisher
  5. Mark as processed: SET processed_at = NOW()
  6. If dispatch fails: leave processed_at as null → retried next poll
```
The guarantee: if the event row exists in the DB, it will eventually be dispatched. At-least-once delivery.

**Q: What is "at-least-once delivery" and what does it mean for handlers?**
A: At-least-once means an event may be dispatched more than once — if the worker dispatches successfully but crashes before marking `processed_at`, it will re-dispatch on the next poll. This means handlers must be **idempotent**: calling them multiple times with the same event produces the same result. In this codebase, `CreateCustomerCommandHandler` doing a duplicate `INSERT` would fail with a unique key violation. The fix: check if the customer already exists before inserting, and treat "already exists" as success rather than an error. Rule: every integration event handler must handle duplicate delivery gracefully.

**Q: Why is the Outbox table per-module (e.g., `ticketing.outbox_messages`) instead of shared?**
A: Module isolation. Each module has its own schema, its own DbContext, and its own transaction boundary. A shared outbox table would require cross-schema writes — coupling modules at the DB level. Each module's outbox is written in the same transaction as that module's business data, using that module's DbContext. The background worker for Ticketing only processes Ticketing's outbox. This mirrors the same "shared nothing" principle as the read models.

**Q: Why does the background worker need to be careful about polling interval and locking?**
A: If you run two API instances, both workers might poll and pick up the same unprocessed event simultaneously, dispatching it twice. The fix is database-level locking: use `SELECT FOR UPDATE SKIP LOCKED` when fetching rows — each worker locks the rows it's processing, and other workers skip them. This is the standard pattern for multi-instance outbox processing. The polling interval is a trade-off: too short wastes DB resources on empty polls, too long adds latency before events are dispatched. A common choice is 5–15 seconds.

**Q: How does the Outbox change `PublishDomainEventsInterceptor`?**
A: Instead of calling `IEventPublisher.PublishAsync(domainEvent)` directly, the interceptor serializes each domain event to JSON and writes an `OutboxMessage` row to the DB using the same `DbContext`. The immediate dispatch is completely removed from the interceptor — it becomes a pure "write to outbox" step. The `OutboxProcessor` background service then reads those rows and calls `IEventPublisher.PublishAsync`. The interface contract stays the same; only the timing changes from synchronous (within the request) to asynchronous (after the request, within seconds).

**Q: How does the Outbox pattern improve performance, reliability, and scalability in this project and in general?**
A:
- **Performance — request latency**: Without the Outbox, the current interceptor dispatches domain events synchronously inside the request pipeline. If `OrderCreatedDomainEventHandler` triggers `CreateCustomerInAttendanceCommand` which writes to the DB, all of that happens before the HTTP response returns. The user waits for every downstream side-effect to complete. With the Outbox, the request writes its own data + outbox rows and returns immediately. Side-effects (integration events, downstream module updates) happen asynchronously in the background. The HTTP response is faster because it does less work per request.
- **Reliability — no silent data loss**: Without the Outbox, a crash between `SaveChanges` and `PublishAsync` silently drops events. At scale this is not hypothetical — deploys, OOM kills, and network timeouts happen constantly. The Outbox makes event publishing durable: the event is committed to the DB in the same transaction as the business data. Even if the process crashes immediately after, the worker will pick it up on restart. This is the difference between "eventually consistent" and "randomly inconsistent".
- **Scalability — background processing**: Moving event dispatch to a background worker means the request thread is freed faster. Under high load, shorter request durations mean fewer concurrent threads needed to serve the same throughput. The worker processes events at its own pace independently — if it falls behind, the outbox queue grows but the API continues accepting requests without degradation. This is the **load leveling** pattern.
- **In other projects**: The Outbox is foundational in any system that writes to a database and publishes events. E-commerce (order placed → payment charged → inventory reserved), banking (transfer initiated → debit account → credit account → notify), and logistics (shipment created → notify carrier → update tracking) all require this guarantee. Without it, every one of these systems has a silent failure window. The Outbox eliminates that window with minimal complexity.

---

## RabbitMQ

**Q: What is RabbitMQ and what problem does it solve?**
A: RabbitMQ is a message broker — a server that accepts messages from publishers and routes them to consumers via queues. Right now, `IEventPublisher.PublishAsync(integrationEvent)` dispatches to `IEventHandler<T>` handlers in-process: the same process, same thread, same request scope. This means: (1) if the consuming module's handler throws, the publishing module's request fails, (2) all modules must be in the same process, (3) if a module is slow, it slows the publisher. RabbitMQ decouples them completely: the publisher puts a message on a queue and moves on. The consumer reads from its queue independently, at its own pace, in its own process if needed.

**Q: What are exchanges, queues, and bindings in RabbitMQ?**
A: Three concepts:
- **Exchange**: receives messages from publishers and routes them. A `fanout` exchange sends every message to all bound queues. A `topic` exchange routes by pattern (e.g., `ticketing.*`). A `direct` exchange routes by exact key.
- **Queue**: a buffer that stores messages until a consumer reads them. Each subscribing module has its own queue so it gets its own copy of every relevant message.
- **Binding**: the rule that connects an exchange to a queue. "Route messages from the `events` exchange to the `ticketing.events` queue."

In this project: when Events publishes `EventPublishedIntegrationEvent`, it sends to an exchange. Ticketing has a queue bound to that exchange — it receives its own copy. If we later add an Attendance module, it gets its own queue and its own copy, with zero changes to the Events module.

**Q: How does RabbitMQ change the `IEventPublisher` implementation?**
A: Currently `IEventPublisher` is implemented in-process — it calls `IEventHandler<T>` handlers registered in the same DI container. With RabbitMQ:
- **Publishing side**: `IEventPublisher.PublishAsync(integrationEvent)` serializes the event to JSON and calls `IModel.BasicPublish` on the RabbitMQ channel. Done — no waiting for handlers.
- **Consuming side**: Each module registers a `IHostedService` consumer that opens a RabbitMQ channel, binds to its queue, and calls `IEventHandler<T>` via `IMediator` when a message arrives.

The key insight: `IEventPublisher` and `IEventHandler<T>` contracts don't change. Only the implementation of `IEventPublisher` changes (writes to broker instead of calling handlers directly), and a new consumer background service is added per module.

**Q: What happens if a module's consumer is down when a message is published?**
A: Messages sit in the queue until the consumer comes back up and reads them. This is the durability guarantee: as long as the queue is configured as `durable: true` and messages are published with `persistent: true`, RabbitMQ stores them to disk. When the consumer restarts, it processes all pending messages in order. This is fundamentally different from the current in-process model where a module crash means any events fired during that time are lost.

**Q: What is a Dead Letter Queue (DLQ) and why do we need it?**
A: When a consumer fails to process a message (handler throws, deserialization fails, etc.), RabbitMQ can move the message to a Dead Letter Queue instead of dropping it or blocking the main queue. The DLQ is a holding area for messages that couldn't be processed. An operator can inspect them, fix the bug, and re-publish them. Without a DLQ, a bad message can block the entire queue (if you keep retrying) or silently disappear (if you drop it). Both are worse than having a quarantine area.

**Q: How does the Outbox interact with RabbitMQ?**
A: The Outbox and RabbitMQ work together — they solve different problems at different points in the chain:
```
[DB transaction]
  Business data + outbox_messages written atomically
[Outbox processor — background service]
  Reads outbox_messages → calls IEventPublisher.PublishAsync(integrationEvent)
[RabbitMQ IEventPublisher]
  Publishes to exchange → message sits in queue
[Module consumer — background service]
  Reads from queue → dispatches to IEventHandler<T> → marks message acknowledged
```
The Outbox guarantees the message reaches RabbitMQ. RabbitMQ guarantees the message reaches the consumer. Together they give you end-to-end reliability: no event is ever lost between DB write and handler execution, even across crashes and restarts.

**Q: Why use RabbitMQ instead of just keeping the in-process publisher?**
A: For a single-server modular monolith that never scales, the in-process publisher is actually fine — simpler and faster. RabbitMQ makes sense when: (1) you need modules to process events at different rates independently, (2) you want to extract a module into a separate service later (the consumer just connects to RabbitMQ from anywhere), (3) you need durability guarantees the in-process publisher can't give, (4) you want to replay events or inspect the message stream. In this project we add it now so the architecture is ready for those scenarios without a rewrite later.

**Q: How does RabbitMQ improve performance, scalability, and resilience in this project and in general?**
A:
- **Performance — publisher throughput**: With the in-process publisher, publishing `OrderCreatedIntegrationEvent` blocks until every handler in every subscribing module completes. If the Attendance module's handler takes 200ms, every order placement takes at least 200ms extra. With RabbitMQ, publishing is fire-and-forget: serialize to JSON, write to RabbitMQ channel (~1ms network call), done. The publisher's throughput is limited only by the broker's ingestion rate, not by the slowest downstream consumer.
- **Scalability — independent consumer scaling**: In-process consumers scale with the API — you can't scale the Ticketing consumer independently from the Events consumer. With RabbitMQ, each module's consumer is an independent `IHostedService`. If the Ticketing queue is falling behind during a ticket sale surge, you can add more Ticketing consumer instances without touching Events. Each consumer instance reads from the same queue, distributing the load. This is **horizontal consumer scaling** — impossible with in-process dispatch.
- **Resilience — isolation of failure**: With in-process dispatch, if `EventCancelledIntegrationEventHandler` in Ticketing throws an unhandled exception, it propagates up through the publisher and can fail the original request in the Events module. Modules that aren't even the source of the failure take down the request. With RabbitMQ, a Ticketing consumer crash affects only Ticketing's queue processing — the Events module keeps running, keeps publishing, and Ticketing's queue accumulates until Ticketing recovers. Failures are **isolated by module boundary**.
- **Back-pressure handling**: If a downstream module is slow, its queue grows. RabbitMQ provides visibility into queue depth (via management UI at `:15672`) so you can see which module is falling behind and respond — add consumers, optimize the handler, or throttle the publisher. In-process dispatch has no such signal; you only see it as high request latency.
- **In other projects**: Any system with asymmetric processing speeds benefits from a broker. Video processing platforms (upload fast, encode slow), notification systems (API fast, email/SMS slow), financial systems (transaction fast, reconciliation slow) all use RabbitMQ or equivalent brokers to decouple the fast producer from the slow consumer. The broker is the buffer that absorbs the speed mismatch.
- **Evently specifically**: When a popular event goes on sale and 10,000 orders are placed in 60 seconds, without RabbitMQ the Attendance module's handler runs synchronously inside each order request — 10,000 DB writes to `attendance.customers` in 60 seconds adds directly to order placement latency. With RabbitMQ, those 10,000 `OrderCreatedIntegrationEvent` messages queue up and Attendance processes them at whatever rate it can handle, independently, without slowing down order placement at all.

---

## Outbox vs RabbitMQ — Do you need both?

**Q: Is the Outbox Pattern alone enough? Why add RabbitMQ at all?**
A: It depends on what you're solving. They solve completely different problems and operate at different points in the chain.

The Outbox solves **reliable event persistence** — it answers: "how do I guarantee the event is not lost if the app crashes after SaveChanges but before publishing?" Answer: write it to the DB in the same transaction. The event survives any crash because it's in the DB.

RabbitMQ solves **cross-process, asynchronous, durable message delivery** — it answers: "how do I get an event from one process (or module) to another process reliably and at scale?"

They are not alternatives. They are complements:
- Outbox guarantees the event reaches the broker.
- RabbitMQ guarantees the event reaches the consumer.

**Q: For a single-instance modular monolith, is Outbox + in-process IEventPublisher sufficient?**
A: Yes — completely. If you run one instance of Evently, never extract a module, and don't need independent consumer scaling, the Outbox + in-process `IEventPublisher` gives you:
- Atomicity: event stored with business data ✓
- Reliability: event survives app crash ✓
- Delivery: OutboxProcessor dispatches to in-process handlers ✓

What you don't get: cross-process delivery, independent scaling per module, queue depth visibility, temporal decoupling. For a single-instance monolith those don't matter.

**Q: When does adding RabbitMQ actually become necessary?**
A: Four scenarios:

1. **Horizontal scaling** — you run 2+ instances of Evently behind a load balancer. An in-process handler only runs inside the process that received the request. If Instance A processes the order, Instance B's Ticketing handler never sees the event. With RabbitMQ, both instances connect to the same queue — whichever instance picks up the message processes it. This is the most common real-world trigger.

2. **Module extraction** — you decide to split the Ticketing module into its own service. In-process dispatch breaks immediately — there's no shared memory. With RabbitMQ already in place, Ticketing just runs its consumer against the same broker from a different process. Zero changes to the Events module.

3. **Consumer speed mismatch** — the Events module fires 1,000 `EventPublishedIntegrationEvent`s per minute, but the Ticketing handler takes 200ms each (= max 300/min). In-process: every Events publish blocks waiting for Ticketing. With RabbitMQ: Events publishes fire instantly, Ticketing's queue grows and drains at its own rate, and you add more Ticketing consumer instances if needed.

4. **Operational visibility** — you want to see queue depth, message rates, consumer lag, DLQs in the RabbitMQ management UI. In-process dispatch is invisible — you have no idea how many events are backed up or whether handlers are failing.

**Q: In Evently specifically, which events need RabbitMQ and which don't?**
A:
- **Domain events** (e.g., `OrderCreatedDomainEvent`) — always in-process. They are raised inside one module's aggregate and consumed by handlers in the same module. RabbitMQ adds zero value here.
- **Integration events** (e.g., `OrderCreatedIntegrationEvent`, `EventPublishedIntegrationEvent`) — these cross module boundaries. They are the candidates for RabbitMQ, because they need to reach a different module which may be in a different process, scaling independently.

So the rule is: **domain events = in-process, integration events = RabbitMQ**.

**Q: Summary — Outbox vs RabbitMQ responsibility table**
A:
| Concern | Outbox | RabbitMQ |
|---|---|---|
| Survive app crash before publish | ✓ | ✗ |
| Atomic write with business data | ✓ | ✗ |
| Cross-process delivery | ✗ | ✓ |
| Independent consumer scaling | ✗ | ✓ |
| Message durability across restarts | ✗ (DB) | ✓ (durable queue) |
| Dead letter / retry | ✗ | ✓ |
| Queue depth visibility | ✗ | ✓ |
| Required for single-instance monolith | ✓ | ✗ (optional) |
| Required for multi-instance / extraction | ✓ | ✓ |
