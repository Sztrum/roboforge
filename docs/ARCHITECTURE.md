# Architecture

## Initial state (2026-10-08)

The project directory contained only the agent instructions (`CLAUDE.md`,
`AGENTS_*.md`) and `docs/`. It was not a Git repository. The .NET SDK was not
installed locally; `gh` was available.

## Solution skeleton (stage 1, 2026-10-08)

The solution was generated from Jason Taylor's Clean Architecture template
(`dotnet new ca-sln -cf None -db postgresql`, template targeting .NET 10 and
Aspire 13) and trimmed to what the MVP needs. Target architecture and stage
plan: `docs/PROJECT.md`.

Projects:

- `src/Domain` — no package references. `Common/` holds `AggregateRoot<TId>`,
  `IDomainEvent`, `DomainException` (carries the violated rule ID) and
  `ValueObject`. Bounded-context folders (`Catalog/`, `Sales/`,
  `Production/`) appear with their first types in stages 2–5.
- `src/Application` — Mediator pipeline (logging, unhandled exceptions,
  FluentValidation, slow-request warning), `IApplicationDbContext`.
- `src/Infrastructure` — `ApplicationDbContext` (EF Core + Npgsql); the
  schema is managed by EF Core migrations applied on start-up in Development.
- `src/Web` — Minimal API endpoint groups (`IEndpointGroup`), OpenAPI +
  Scalar, `ProblemDetails` mapping.
- `src/AppHost` — Aspire: PostgreSQL container (persistent) + Web API.
- `src/ServiceDefaults`, `src/Shared` — Aspire defaults (OpenTelemetry,
  health checks at `/health` and `/alive` in Development) and resource names.
- `tests/Domain.UnitTests`, `tests/Application.UnitTests`,
  `tests/Application.FunctionalTests` (+ `tests/TestAppHost`).

### Decisions

- **Removed from the template:** Azure hosting (Container Apps, Azure
  PostgreSQL, Key Vault), ASP.NET Core Identity with bearer tokens and the
  `Users` endpoints, authorization behaviour, auditable entities, AutoMapper
  and the Todo/WeatherForecast samples. The MVP has no login (a customer ID
  header comes later) and no cloud deployment.
- **Mediator instead of MediatR** — MediatR 13+ and AutoMapper 15+ moved to a
  commercial license. `Mediator` (martinothamar, MIT) has an almost identical
  API, generates the dispatch code at compile time and has no reflection at
  runtime. Queries are mapped by hand instead of AutoMapper.
- **Domain has no dependencies.** The template's `BaseEvent : INotification`
  tied the domain to the mediator library; `IDomainEvent` is a plain marker
  interface. The template's `DispatchDomainEventsInterceptor` was removed: in
  stage 4 events go to the outbox instead of being published in-process
  inside `SaveChanges`.
- **`DomainException` → HTTP 422** `ProblemDetails` with `detail`
  `"<rule>: <message>"` and a `rules` extension listing the rule IDs.
- **EF Core migrations instead of `EnsureDeleted`/`EnsureCreated`.** The
  template recreated the database on every start; the first migration is
  added in stage 2 with the first table.
- **xUnit v3 instead of NUnit**, running on Microsoft.Testing.Platform
  (`global.json` → `test.runner`), which .NET 10 `dotnet test` requires for
  xUnit v3. Coverage uses `coverlet.MTP`.
- **Functional tests use `Aspire.Hosting.Testing`** (as in the template)
  instead of Testcontainers: the test app host starts a real PostgreSQL
  container, with no second container library to maintain.
- **`ASPIRE010` is suppressed** in the app hosts: DCP and the dashboard come
  from NuGet packages, so `dotnet run` works without the Aspire CLI.

## References

- Solution template: https://github.com/jasontaylordev/CleanArchitecture
- DDD patterns reference: https://github.com/kgrzybek/modular-monolith-with-ddd
