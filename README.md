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

dotnet run --project src/MiniAlarmBoard.Api

The API listens on http://localhost:5014. In development the OpenAPI document is
served at /openapi/v1.json.

Endpoints:

- GET /alarms returns every alarm raised so far
- /hubs/alarms is the SignalR hub and is push only

Live board: open http://127.0.0.1:5014/alarmboard.html in a browser.

Use 127.0.0.1 rather than localhost. On this WSL setup, Windows to WSL forwarding
for localhost was not reliable, and 127.0.0.1 forces IPv4.

Zone detection settings live in appsettings.json under "ZoneDetector". Changing
WindowSeconds or Threshold needs an app restart but no rebuild.

## Phases

- \[\*] Phase 0: environment and project setup
- \[\*] Phase 1: C# language basics
- \[\*] Phase 2: OOP
- \[\*] Phase 3: dependency injection and lifetimes
- \[\*] Phase 4: async and background work
- \[\*] Phase 5: SignalR
- \[\*] Phase 6: design patterns
- \[\*] Phase 7: zone detector
- \[ ] Phase 8: AI insight layer
- \[ ] Phase 9: testing
- \[ ] Phase 10: review

## Notes and decisions

- Store is a singleton because the generator and the controller must share the same state, and a singleton cannot hold a scoped service because that instance would be captured for the life of the app.
- Enums are serialised as strings rather than numbers, using
  [JsonConverter(typeof(JsonStringEnumConverter))]. Reordering the enum later cannot
  silently change a value a client already stored.
- SignalR client method names go out exactly as the interface declares them,
  AlarmRaised and ZoneAlarmRaised, with no case change. The JavaScript handler has to
  match character for character and a mismatch fails silently in both consoles. During
  bring-up we registered both the PascalCase and camelCase names to see which one fired.
- The zone detector takes a TimeProvider instead of reading the clock inside, so a test
  can jump time forward rather than wait. A 30 second window becomes microseconds.
- Detection windows are measured from arrival time, not from the alarm's own timestamp,
  so the window reflects the rate alarms are arriving. In this project those two times
  coincide because the generator stamps at creation. A real device feed would need a
  separate received-at field.
- An area is marked when it crosses the threshold and released when the count falls back
  below it, so a flood raises one zone alarm rather than one per alarm. The flag must be
  cleared on recovery, otherwise that area is a dead alarm path for the rest of the process.
- A plain lock was chosen over ConcurrentDictionary because the invariant spans trim,
  check and mark, and those three have to be atomic together.
- The detector lock is not load bearing while only the background generator calls it.
  It becomes necessary the moment a request thread reads that state, which is exactly
  what the insight layer does next.
- Known smell: the generator is turning into an orchestrator, doing generation, push,
  detection and soon insight. Phase 10 splits this out.
