# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a C# (.NET 8) ASP.NET Core Web API — a timezone-aware insurance coverage scheduling API. It is structured as an interview exercise with an intentional bug to find and fix.

## Commands

All commands should be run from within `timezone-coverage-api/`.

```bash
# Restore dependencies
dotnet restore

# Run the API (Swagger UI available at /swagger in Development)
dotnet run --project src/TimeZoneCoverage.Api/TimeZoneCoverage.Api.csproj

# Run all tests
dotnet test

# Run a single test by name filter
dotnet test --filter "FullyQualifiedName~TestMethodName"

# Run tests with verbose output
dotnet test --verbosity detailed
```

The solution file for IDE use is `timezone-coverage-api/TimeZoneCoverageInterview.sln`.

A Postman collection is available at the repo root as `timezone-coverage-postman (1).zip`.

## Architecture

Layered architecture: **Controller → Service → Models**, with all services programmed against interfaces for testability.

- `Program.cs` — DI registration and middleware pipeline. Registers `TimeZoneResolver`, `CoverageActivationService`, `SystemClock`. Swagger enabled only in Development. Also exposes `GET /health`.
- `Controllers/CoverageController.cs` — Two endpoints: `POST /api/coverage/schedule` and `POST /api/coverage/status`.
- `Services/CoverageActivationService.cs` — Core business logic. Contains `ConvertStartDateToActivationUtc_Buggy`, an intentionally incorrect method that treats a selected date as UTC midnight instead of midnight in the activation timezone.
- `Services/TimeZoneResolver.cs` — Resolves IANA/Windows timezone ID strings to `TimeZoneInfo`.
- `Services/SystemClock.cs` / `IClock.cs` — Injectable time provider. `IClock` is registered in DI but not yet injected into `CoverageActivationService`; it exists as a testability hook for future use.
- `tests/TimeZoneCoverage.Api.Tests/CoverageActivationServiceTests.cs` — xUnit tests that document the correct behavior. Tests construct `CoverageActivationService` directly with a real `TimeZoneResolver` — no mocks needed.

## Historical bug note

Earlier versions of the scheduling logic treated the selected activation date as UTC midnight, which produced incorrect activation timestamps for the requested timezone. The correct behavior is to construct midnight in the activation timezone and then convert that local time to UTC, respecting DST offsets.

This is now handled by creating a `DateTime` with `DateTimeKind.Unspecified` for local midnight and passing it to `TimeZoneInfo.ConvertTimeToUtc(localMidnight, activationTimeZone)`, which applies the correct timezone and DST rules automatically.

Expected results from the tests:

- May 1 midnight in Sydney (UTC+10, standard time) = `2026-04-30T14:00:00Z`
- Jan 15 midnight in Sydney (UTC+11, DST) = `2026-01-14T13:00:00Z`
