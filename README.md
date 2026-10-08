# RoboForge

[![CI](https://github.com/Sztrum/roboforge/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Sztrum/roboforge/actions/workflows/ci.yml)

A robot configurator and production workflow built with .NET 10, Clean
Architecture and DDD. Customers assemble a small robot from catalog parts; the
workshop takes the order through production stages up to shipping.

Project scope, domain model and business rules: [`docs/PROJECT.md`](docs/PROJECT.md).
Architecture decisions: [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).

## Requirements

- .NET 10 SDK
- Docker (PostgreSQL runs in a container started by Aspire)

## Run

```bash
dotnet run --project src/AppHost --launch-profile http
```

This starts PostgreSQL, the Web API and the Aspire dashboard
(http://roboforge.dev.localhost:15090). The API reference (Scalar) is at
http://localhost:5040/scalar.

## Build and test

```bash
dotnet build
dotnet test
```

Functional tests start their own PostgreSQL container through Aspire, so Docker
must be running.

## Based on

- [Clean Architecture Solution Template](https://github.com/jasontaylordev/CleanArchitecture) by Jason Taylor
