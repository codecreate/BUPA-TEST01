# Full Stack Interview Exercise - Time Zone Coverage API

A small .NET 8 Web API exercise for a **live 30-minute in-session practical interview**.

## Scenario
A customer can purchase insurance while in one country and nominate a future policy start date.
The business expectation is:

> Coverage should begin at **midnight in the intended activation time zone** on the selected date.

The current implementation is incorrect. It assumes the selected date is already midnight UTC.
That causes real-world issues when customers travel between time zones.

### Example
A customer buys cover while in India and selects **2026-05-01** as the start date.
The intended activation time zone is **Australia/Sydney**.

The current implementation stores activation as:
- `2026-05-01T00:00:00Z`

But the correct activation should be:
- local midnight in Sydney on `2026-05-01`
- converted to UTC
- which is `2026-04-30T14:00:00Z` in standard time

A second issue also exists:
- the solution must remain correct during **daylight saving time**

---

## Candidate Task
Fix the scheduling logic so that:

1. Coverage starts at **local midnight in the target activation time zone**.
2. The UTC activation time is stored correctly.
3. Daylight saving is handled correctly.
4. The API still returns a clear response.

You do **not** need to redesign the whole codebase.
Keep the solution simple and pragmatic.

---

## Existing Endpoints

### `POST /api/coverage/schedule`
Schedules the coverage activation date.

Example request:
```json
{
  "customerId": "cust-001",
  "selectedStartDate": "2026-05-01",
  "purchaseTimeZoneId": "Asia/Kolkata",
  "activationTimeZoneId": "Australia/Sydney"
}
```

### `POST /api/coverage/status`
Checks if coverage is active based on a UTC activation timestamp.

---

## What is intentionally incomplete or wrong
- The conversion helper assumes midnight UTC.
- The activation time zone is not used correctly.
- Daylight saving behavior is not validated by the implementation.
- Tests currently describe the expected behavior and should guide your fix.

---

## Notes
- AI tools are allowed.
- We care more about reasoning than speed.
- Explain your assumptions as you work.
- A capable solution should fit comfortably in about 30 minutes.

---

## Suggested Local Commands
```bash
dotnet restore
dotnet test
dotnet run --project src/TimeZoneCoverage.Api/TimeZoneCoverage.Api.csproj
```

Swagger is enabled in Development mode.
