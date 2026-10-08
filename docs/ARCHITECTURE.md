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

The initial layer-first layout (one `Domain`, `Application`, `Infrastructure`
and `Web` project) was replaced by a module-first layout the same day; see
"Module-first layout" below.

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

## Quality gates (stage 1, 2026-10-08)

- **Analyzers in every project:** Roslynator and SonarAnalyzer as
  `GlobalPackageReference` (`Directory.Packages.props`), plus the .NET
  analyzers at `AnalysisLevel` `latest-recommended` and code style enforced
  in the build. With `TreatWarningsAsErrors` every finding fails the build.
- **Suppressed rules** (`.editorconfig`, each with its reason):
  `RCS1194` (domain exceptions require a rule ID), `CA1716` (`Shared` is
  only a VB keyword), `CA1707` in `tests/` (test names use underscores).
- **Fixes the analyzers forced on template code:** source-generated
  `LoggerMessage` logging; `UnhandledExceptionBehaviour` removed (it logged
  and rethrew, so ASP.NET Core logged every exception twice and business
  rule violations showed up as errors); the open CORS policy removed (no
  browser client yet; add one with explicit origins when needed); the
  template `ValueObject` base class removed (value objects are `record`s).
- **Architecture tests** (NetArchTest, `tests/Architecture.Tests`): see
  "Module-first layout" for the current rules.

## Module-first layout (2026-10-08)

Decision: each bounded context is a module with its own layer projects, as in
the MDD configurator (`app/V1/Modules/{Module}/{Domain,Application,Infrastructure,UI}`)
and the DDD reference (kgrzybek/modular-monolith-with-ddd). All code of one
module lives in one folder, and the compiler enforces the layers inside it.

```
src/
├── AppHost/                    Aspire: PostgreSQL + API + dashboard
├── ServiceDefaults/            OpenTelemetry, health checks, resilience
├── Web/                        API host: composes the modules, Mediator pipeline,
│                               exception → ProblemDetails, OpenAPI, migrations on start-up
├── Shared/
│   ├── Shared.Domain/          AggregateRoot<TId>, IDomainEvent, DomainException (later Money, LeadTime)
│   ├── Shared.Application/     pipeline behaviours, RequestValidationFailedException
│   ├── Shared.UI/              IEndpointGroup and endpoint-group mapping
│   └── Shared.Hosting/         Aspire resource names
└── Modules/
    └── Catalog/                (Sales and Production follow in stages 3–5)
        ├── Catalog.Domain/
        ├── Catalog.Application/
        ├── Catalog.Infrastructure/   CatalogDbContext, module registration
        └── Catalog.UI/               endpoint groups (/api/parts)
tests/
├── Shared/Shared.{Domain,Application}.UnitTests/
├── Modules/{Module}/{Module}.Domain.UnitTests/   (from stage 2)
├── Architecture.Tests/
├── Web.FunctionalTests/
└── TestAppHost/
```

- **Project references inside a module:** `UI → Application`,
  `Infrastructure → Application → Domain`; `Domain → Shared.Domain`,
  `Application → Shared.Application`, `UI → Shared.UI`. No module references
  another module.
- **One `DbContext` per module, one schema per module** (`catalog`, later
  `sales`, `production`), each with its own migrations and migration history
  table in its schema. Modules therefore share no tables. `Web` applies each
  module's pending migrations on start-up in Development.
- **Each module registers itself:** `Add{Module}Module()` in
  `{Module}.Infrastructure` and `Map{Module}ModuleEndpoints()` in
  `{Module}.UI`; `Web/Program.cs` only calls them.
- **The Mediator source generator runs in `Web`**, the only project that
  references every module, so it finds all handlers. Modules reference only
  `Mediator.Abstractions`.
- **Architecture tests** check, for every module: Domain uses no framework and
  no outer layer; Application uses no Infrastructure, UI, Npgsql or ASP.NET
  Core; Infrastructure uses no UI; no layer uses another bounded context
  (Sales and Production are already named); shared code uses no module.
  A new module is added to `ProjectAssemblies.ExistingModules` and referenced
  in `Architecture.Tests.csproj`. Verified to fail on a deliberate EF Core
  reference in `Catalog.Domain`.
- **Integration events** between modules (stage 4) will need contracts both
  sides can reference; the reference repository uses a separate
  `{Module}.IntegrationEvents` project per module. Decided with the outbox.

## Continuous integration (stage 1, 2026-10-08)

`.github/workflows/ci.yml` runs on every pull request and every push to
`main`: restore, Release build (analyzers as errors), `dotnet format
--verify-no-changes`, all tests including the functional tests (the
`ubuntu-latest` runner has Docker for the Aspire-started PostgreSQL), and
code coverage with `coverlet.MTP`. ReportGenerator turns the Cobertura files
into a summary on the run page and an HTML report artifact
(`coverage-report`). The README shows the CI badge.

## Naming (2026-10-08)

File and type names follow the MDD configurator project (`app/V1`): a name
describes what the file does and ends with its role suffix
(`ValidateRequestPipelineBehavior`, `MapExceptionsToProblemDetailsExceptionHandler`,
`OrderCannotBeCancelledInProductionException`); aggregates end with
`Aggregate`, enums with `Enum`, events with `Event`, commands/queries/handlers
with `Command`/`Query`/`Handler`. The full table lives in the agent rules
(`AGENTS_DOTNET_RULES.md`, local). Template types were renamed accordingly
(`DependencyInjection` → service-registration classes, `Services` →
`AspireResourceNames`, which also removes its clash with
`WebApplicationFactory.Services`).

- `CA1711` and `S2344` are suppressed so `…Enum` and `…EventHandler` names
  are allowed.
- Acronyms keep C# casing (`Dto`, not `DTO`) to keep the PascalCase rule
  `S101` active.
- `DomainException` is abstract: every business rule gets its own exception
  type named after the situation.

## References

- Solution template: https://github.com/jasontaylordev/CleanArchitecture
- DDD patterns reference: https://github.com/kgrzybek/modular-monolith-with-ddd
