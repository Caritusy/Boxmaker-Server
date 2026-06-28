# Boxmaker Server

An ASP.NET Core server implementation for **Boxmaker**, designed for local hosting, private deployments, preservation work, and custom server experiments.

This repository contains the game API handlers, account persistence logic, map storage logic, mission flow, replay/ranking support, and a small Razor-based web console for basic account and map operations.

本 readme 文档的中文版本, 查阅 [README.zh-CN.md](./README.zh-CN.md).

## Project Status

Boxmaker was originally an online game published by Yinyue Network. The official service is no longer operating, so this project adapts and extends the server-side behavior for community use and compatible client builds.

This project is not affiliated with the original publisher.

## Features

- ASP.NET Core server targeting `.NET 9.0`.
- Protobuf-based client packet parsing and responses.
- DES-compatible client payload decoding.
- Account login, registration, token verification, profile updates, and password changes.
- Local account data storage under per-player folders.
- Server map storage, lookup, upload, download, search, likes, comments, and ranking data.
- Edit-map lifecycle support: create, save, rename, upload, delete, and inspect.
- Mission and challenge route handlers for play, replay, failure, success, continuation, and drop flows.
- Replay/video storage and map ranking integration.
- Player state rebuild on startup for profile and web views.
- Recent-play and favorite-map tracking.
- Small Razor web console with login, profile editing, password update, and public map search.
- Static web assets under `wwwroot`.

## Pros

- Runs as a normal ASP.NET Core application and can be hosted with standard .NET tooling.
- Keeps protocol classes separate from application logic.
- Uses partial classes to group account-management and proxy-route behavior by domain.
- Stores data locally, which makes private testing and migration easier.
- Includes both game-facing HTTP routes and a simple browser-facing operations page.
- The current structure separates models, networking helpers, services, infrastructure helpers, proxy routes, account logic, pages, and generated-style protocol files.

## Cons and Known Limitations

- The protocol classes are legacy/generated-style code and still contain many nullable warnings.
- Persistence is file-based rather than database-backed.
- Some helper class names follow the original code style instead of modern C# naming conventions.
- There is no automated test suite yet.
- Some runtime data directories, such as account and map data, are expected to exist or be created by the deployment workflow.
- The server is intended for compatible Boxmaker clients and is not a generic public API.

## Requirements

- .NET SDK `9.0` or newer.
- A compatible Boxmaker client build.
- Windows, Linux, or any environment that can run ASP.NET Core and access the configured data directories.

## Quick Start

Restore and build:

```powershell
dotnet restore
dotnet build
```

Run in the default project profile:

```powershell
dotnet run
```

The checked-in Kestrel configuration listens on:

```text
http://0.0.0.0:13500
```

The development launch profile uses:

```text
http://localhost:5226
```

## Client Repositories

Recommended companion client resources:

- [Boxmaker client source](https://github.com/Caritusy/Boxmaker)
- [Boxmaker client releases](https://github.com/Caritusy/Boxmaker-Release/releases)

## Project Layout

```text
AccountManager/      Account files, map cache, missions, player state, edit maps, and IO queue logic
BoxmakerProxy/       HTTP route handlers grouped by auth, maps, editor, and missions
Infrastructure/      Console output, file locking helpers, and DES helpers
Models/              Server-side map, mission, and map-data models
Networking/          Packet helpers, opcodes, and network message containers
Pages/               Razor web console pages
protocol.game/       Game protocol DTOs
protocol.map/        Map protocol DTOs
Services/            Runtime service helpers
Utilities/           Shared utility and version helpers
wwwroot/             Static web assets
```

## Development Notes

- Avoid changing `protocol.game` and `protocol.map` unless protocol compatibility work requires it.
- Prefer adding new game route handlers under `BoxmakerProxy/`.
- Prefer adding account, map, mission, or player-state behavior under `AccountManager/`.
- Keep data model changes explicit because existing stored files may need migration.

## License

See the license files included in this repository.
