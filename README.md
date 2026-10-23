# Mini Alarm Board

A small ASP.NET Core Web API built as a learning project. It generates fake industrial alarms, pushes them to connected clients over SignalR, and raises a zone alarm when too many alarms arrive in one area within a time window.

## Why this exists

A deliberate practice project for C#, OOP, dependency injection, async background
work and SignalR. It mimics the shape of industrial alarm management software
without being connected to anything real.

## Stack

- .NET 10, ASP.NET Core Web API
- SignalR for real-time push
- xUnit for tests

## How to run it

dotnet run --project

only endpoint so far is GET /weatherforecast

## Phases

- [*] Phase 0: environment and project setup
- [*] Phase 1: C# language basics
- [*] Phase 2: OOP
- [ ] Phase 3: dependency injection and lifetimes
- [ ] Phase 4: async and background work
- [ ] Phase 5: SignalR
- [ ] Phase 6: design patterns
- [ ] Phase 7: zone detector
- [ ] Phase 8: AI insight layer
- [ ] Phase 9: testing
- [ ] Phase 10: review

## Notes and decisions

Nothing yet.
