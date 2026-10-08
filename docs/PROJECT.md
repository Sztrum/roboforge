# RoboForge project

## Goal

A customer builds their own small robot from ready-made parts (3D-printed
housing, drive, electronics, sensors) and the workshop takes the order
through production stages up to shipping. The project demonstrates:

- **Real DDD:** business logic lives in aggregates, not controllers or
  handlers.
- **Bounded contexts** (Sales and Production) communicating only through
  events.
- **Production quality:** tests on every level, CI, architecture tests and
  .NET Aspire with an observability dashboard.

## MVP scope

Intentionally small: 4 part categories, 5 production stages, one customer
type, no online payments.

**In:**

- Part catalog: 4 categories (Housing, Drive, Electronics, Sensor); each part
  has a price, lead time and technical attributes.
- Configurator: create a robot configuration, validate compatibility,
  compute price and lead time.
- Order: place from a configuration, confirm, cancel.
- Production: a job goes through Printing → Assembly → QA tests → Packing →
  Shipped.
- Production statuses flow back to the order so the customer sees progress.
- REST API + OpenAPI/Scalar (optionally a simple frontend at the end).

**Out:** payments, login with roles (a simple customer ID header is enough),
part inventory, invoices, e-mail notifications (an event log entry instead).

## Bounded contexts

- **Catalog** — what can be bought.
- **Sales** — what the customer configured and ordered.
- **Production** — what happens in the workshop.

Each has its own model: the same robot is an `OrderAggregate` in Sales and a
`ProductionJobAggregate` in Production. Sales and Production do not know each other's
classes; they are connected only by events.

## Domain model

| Context | Element | Kind | Key methods / fields |
| --- | --- | --- | --- |
| Catalog | `PartAggregate` | Aggregate | `Create(...)`, `ChangePrice(Money)`, `Discontinue()` |
| Catalog | `PartCategoryEnum` | Enum | Housing, Drive, Electronics, Sensor |
| Catalog | `TechSpec` | Value object | `MountType`, `Voltage`, `MaxLoadGrams`, `WeightGrams` |
| Sales | `RobotConfigurationAggregate` | Aggregate | `AddPart(PartSnapshot)`, `RemovePart(PartId)`, `Validate()`, `Finalize()` |
| Sales | `PartSnapshot` | Value object | copy of price and spec at the time the part was added |
| Sales | `OrderAggregate` | Aggregate | `Place(configuration)`, `Confirm()`, `Cancel(reason)`, `MarkStageReached(stage)` |
| Sales | `OrderStatusEnum` | Enum | Draft, Placed, Confirmed, InProduction, Shipped, Cancelled |
| Production | `ProductionJobAggregate` | Aggregate | `Start()`, `CompleteStage(stage, by)`, `FailQa(reason)`, `Ship(trackingNo)` |
| Production | `ProductionStageEnum` | Enum | Printing, Assembly, QaTests, Packing, Shipped |
| Shared | `Money` | Value object | `Amount`, `Currency`, operators `+` and `*` |
| Shared | `LeadTime` | Value object | working days, `Combine(...)` |

Names follow the naming rules (descriptive names with role suffixes, as in
the MDD configurator project). Business-rule violations are separate
exceptions named after the situation, derived from `DomainException`, e.g.
`OrderCannotBeCancelledInProductionException` (R8).

Key decision: `OrderAggregate` stores a **snapshot** of parts, not a catalog
reference. A catalog price change does not change a placed order's price.

## Business rules

Each rule lives in an aggregate or domain service and has its own unit test.
Numbers are examples and may be changed (record the change here).

| # | Rule | Lives in |
| --- | --- | --- |
| R1 | A configuration has exactly 1 housing, 1 drive, 1 electronics and 0–3 sensors. | `RobotConfigurationAggregate.Validate()` |
| R2 | The drive must match the housing's `MountType`. | `RobotPartsCompatibilityPolicy` |
| R3 | Electronics voltage must match the drive voltage. | `RobotPartsCompatibilityPolicy` |
| R4 | Total part weight must not exceed the drive's `MaxLoadGrams`. | `RobotPartsCompatibilityPolicy` |
| R5 | Price = sum of part prices + 15% assembly. Orders from 1000 PLN get 5% off. | `RobotConfigurationPricingPolicy` |
| R6 | Lead time = longest part lead time + 2 days assembly + 1 day QA. | `LeadTime.Combine(...)` |
| R7 | A discontinued part cannot be added to a new configuration. | `RobotConfigurationAggregate.AddPart()` |
| R8 | An order can be cancelled only before production starts. | `OrderAggregate.Cancel()` |
| R9 | Production stages run strictly in order, no skipping. | `ProductionJobAggregate.CompleteStage()` |
| R10 | Failed QA sends the job back to Assembly; after 2 failures the job is on hold pending a decision. | `ProductionJobAggregate.FailQa()` |

R2–R4 span several parts, so they live in the `RobotPartsCompatibilityPolicy` domain
service, which returns a list of violations instead of throwing on the first
one — the customer sees all problems at once.

## Domain events and integration

Contexts never call each other directly. Events are written to an outbox in
the same transaction as the state change.

| Event | Published by | Handled by | Effect |
| --- | --- | --- | --- |
| `OrderConfirmedEvent` | Sales | Production | creates a `ProductionJobAggregate` with the part list |
| `OrderCancelledEvent` | Sales | (log) | log entry (e-mail in the future) |
| `ProductionStageCompletedEvent` | Production | Sales | `OrderAggregate.MarkStageReached(stage)`, status `InProduction` |
| `QaFailedEvent` | Production | Sales | order note, new lead time |
| `RobotShippedEvent` | Production | Sales | status `Shipped`, tracking number |
| `PartDiscontinuedEvent` | Catalog | Sales | marks draft configurations with this part as invalid |

Mechanics:

1. The aggregate adds an event via `AddDomainEvent(...)`.
2. An EF Core interceptor writes events to `OutboxMessages` during
   `SaveChanges`, in the same transaction.
3. A hosted background service polls the outbox and publishes events.
4. Initially in-process publishing (mediator); later replaceable with
   RabbitMQ + MassTransit without touching the domain.

## References

- Solution template: https://github.com/jasontaylordev/CleanArchitecture
- DDD reference for patterns (aggregates, domain events, outbox, module
  boundaries, architecture tests): https://github.com/kgrzybek/modular-monolith-with-ddd

## Technology

| Area | Choice |
| --- | --- |
| Platform | .NET 10, C# 14 |
| Database | PostgreSQL + EF Core |
| CQRS | `Mediator` (martinothamar, MIT, source generator) — see `docs/ARCHITECTURE.md` |
| Validation | FluentValidation |
| Orchestration | .NET Aspire (API + PostgreSQL + dashboard) |
| API docs | OpenAPI + Scalar |
| Quality | `Directory.Build.props` with `Nullable`, `TreatWarningsAsErrors`, Roslynator + Sonar analyzers |
| Tests | xUnit v3, Shouldly, NetArchTest, `Aspire.Hosting.Testing` (real PostgreSQL container), `WebApplicationFactory`, Coverlet |
| CI | GitHub Actions on every push and PR: build, test, coverage, badge |

## API

| Method | Path | Action |
| --- | --- | --- |
| GET | `/api/parts?category=` | list catalog parts |
| POST | `/api/parts` | add a part |
| PUT | `/api/parts/{id}/price` | change price |
| POST | `/api/parts/{id}/discontinue` | discontinue a part |
| POST | `/api/configurations` | new empty configuration |
| POST | `/api/configurations/{id}/parts` | add a part to a configuration |
| DELETE | `/api/configurations/{id}/parts/{partId}` | remove a part |
| GET | `/api/configurations/{id}/quote` | validation + price + lead time |
| POST | `/api/orders` | place an order from a configuration |
| POST | `/api/orders/{id}/confirm` | confirm (starts production) |
| POST | `/api/orders/{id}/cancel` | cancel |
| GET | `/api/orders/{id}` | details and stage history |
| GET | `/api/production/jobs?stage=` | workshop queue |
| POST | `/api/production/jobs/{id}/complete-stage` | complete a stage |
| POST | `/api/production/jobs/{id}/fail-qa` | failed QA |

Domain errors: `ProblemDetails` (RFC 9457), status 422, list of violated
rules, e.g. `R2: Napęd nie pasuje do mocowania obudowy`.

## Stage plan

Each stage ends with a working project, green tests and one or more PRs
awaiting the user's review. Do not start the next stage without approval.

1. **Skeleton** — solution from the template, Aspire + PostgreSQL,
   `Directory.Build.props`, analyzers, GitHub Actions CI, first architecture
   test.
2. **Catalog** — `PartAggregate`, `Money` and `TechSpec` value objects,
   commands, seed with ~12 parts. Split into small tasks, one branch and PR
   each (the owner writes 1–12 against prepared failing tests):
   1. `Money` creation; a negative amount is rejected
   2. `Money` rounds to two decimal places
   3. `Money` addition; different currencies are rejected
   4. `Money` multiplication by a factor or a quantity
   5. `PartCategoryEnum`, `MountTypeEnum`
   6. `LeadTime` (working days, at least 1; `Combine` comes in stage 3)
   7. `TechSpec` (mount type, voltage, max load, weight; positive values)
   8. `PartId` (strongly typed ID)
   9. `PartAggregate.Create(...)`; the name must not be empty
   10. `PartAggregate.ChangePrice(...)`
   11. `PartAggregate.Discontinue()` raises `PartDiscontinuedEvent`
   12. a discontinued part cannot be repriced or discontinued again
   13. `CatalogDbContext` mapping and the first migration
   14. `IndexPartQuery` + `GET /api/parts?category=`
   15. seed with 12 parts
   16. `StorePartCommand` + validator + `POST /api/parts`
   17. `ChangePartPriceCommand` + `PUT /api/parts/{id}/price`
   18. `DiscontinuePartCommand` + `POST /api/parts/{id}/discontinue`
   19. `.http` scenario file

   Proposed defaults, awaiting the owner's confirmation: PLN only in the MVP, but `Money` carries its
   currency; discontinuing a part is final (no reactivation).
3. **Configurator** — `RobotConfigurationAggregate`, `RobotPartsCompatibilityPolicy`
   (R1–R4, R7), `RobotConfigurationPricingPolicy` (R5), `LeadTime` (R6), `/quote` endpoint, tests for each
   rule.
4. **Orders** — `OrderAggregate` with part snapshots, confirm and cancel (R8), domain
   events + outbox.
5. **Production** — `ProductionJobAggregate` created from `OrderConfirmedEvent`, stages
   (R9), failed QA (R10), events back to Sales.
6. **Demo polish** — README with diagram, demo seed data, `.http` scenario
   file, optional simple Blazor frontend.

## Demo scenario (~10 minutes)

1. `dotnet run` in AppHost: API, database and Aspire dashboard start.
2. In Scalar, build a robot with a drive that does not fit the housing → 422
   with R2 and R4.
3. Fix the configuration, get a quote with discount and lead time.
4. Place and confirm the order; show the trace in Aspire: request → outbox →
   event → new `ProductionJobAggregate`.
5. Take the job through the stages, once with failed QA; the order status
   updates itself.
6. Try to cancel an order in production → refused.
7. Show the green CI pipeline and the architecture test.

**Later extensions:** RabbitMQ + MassTransit, Blazor frontend with robot
preview, login with Customer/Workshop roles, part inventory with
reservations, Stripe test-mode payments.
