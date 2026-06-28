# Boxmaker Server

ASP.NET Core server implementation for **Boxmaker**, built for private hosting, compatible client testing, preservation work, and continued development after the original online service stopped operating.

Chinese documentation is available at [README.zh-CN.md](./README.zh-CN.md).

## Overview

This project implements the Boxmaker game HTTP protocol, local account persistence, server-side map storage, mission flow, replay/ranking data, and a browser-based operations console.

The server is intended for compatible Boxmaker clients and controlled deployments. It is not affiliated with the original publisher.

## Features

- ASP.NET Core server targeting `.NET 9.0`.
- Protobuf-based client packet parsing and response serialization.
- Compatible DES payload decoding for legacy client traffic.
- Account login, guest creation, registration, account switching, profile editing, and password updates.
- Local file-based account, map, mission, recent-play, favorite, and player-state persistence.
- Server map upload, download, search, listing, likes, comments, completion, replay, and ranking support.
- Editor map lifecycle: create, save, rename, upload, delete, download, and inspect.
- Mission and challenge APIs for start, continue, replay, success, failure, finish, and drop flows.
- Replay/video storage tied to map ranking entries.
- Startup player-state rebuild for profile and web views.
- IO queue support for non-critical persistence work so request handlers avoid unnecessary blocking.
- Modernized console output for clearer runtime diagnostics.
- Razor web console with login, profile editing, country/region presets, avatar presets, password update, and public map search.
- Bundled country/region configuration under `Resources/config/t_guojia.txt`.
- Static web assets under `wwwroot`.

## Current Security Notes

A focused file traversal review was performed on the current code paths. No HTTP-controlled path traversal exposure was found in the active account, map, mission, profile, or web-console routes.

Important boundaries:

- Account directories are created from validated `openid` values. Registration rejects path separators and Windows-invalid filename characters.
- Login and web login look up cached account paths instead of combining request strings into filesystem paths.
- Server map files are addressed by integer map ids.
- User map lists, mission data, player state, and profile files use fixed filenames below resolved account directories.
- `BOXMAKER_COUNTRY_CONFIG` is an optional deployment-time environment variable. It is not read from HTTP input.

This does not replace a full security review, authentication hardening pass, or malicious payload fuzzing. It only covers file traversal risk in the current implementation.

## Pros

- Runs with standard .NET tooling and is easy to host privately.
- Keeps protocol DTOs separate from most application logic.
- Groups account logic and HTTP proxy routes into domain-focused partial classes.
- Uses local files, making backups, migration, and manual inspection straightforward.
- Provides both game-facing APIs and a browser-facing management console.
- Includes practical player statistics, recent/favorite maps, and readable map search output.
- Bundles required web-console country data instead of relying only on a developer-machine path.

## Cons and Limitations

- Persistence is file-based rather than database-backed.
- There is no automated test suite yet.
- Some protocol and legacy helper code still follows the original naming/style and produces nullable warnings.
- Several operations still depend on synchronous file IO in older code paths.
- Data migrations must be handled carefully because existing account and map files are live storage.
- The project is designed for compatible Boxmaker clients, not as a general public API.

## Requirements

- .NET SDK `9.0` or newer.
- A compatible Boxmaker client build.
- Windows, Linux, or another ASP.NET Core capable runtime environment.
- Read/write access to the server data directories.

## Quick Start

Restore and build:

```powershell
dotnet restore
dotnet build
```

Run:

```powershell
dotnet run
```

The checked-in Kestrel configuration listens on:

```text
http://0.0.0.0:13500
```

The browser console is available at:

```text
http://localhost:13500/
```

## Configuration

Country/region presets are loaded in this order:

1. `BOXMAKER_COUNTRY_CONFIG`
2. `Resources/config/t_guojia.txt` beside the published application
3. `Resources/config/t_guojia.txt` below the current working directory
4. The legacy development path used by the Unity export

The repository includes `Resources/config/t_guojia.txt` and the project file copies it to build/publish output.

## Project Layout

```text
AccountManager/      Account files, map cache, missions, player state, edit maps, and IO queue logic
BoxmakerProxy/       HTTP route handlers grouped by auth, maps, editor, and missions
Infrastructure/      Console output, file helpers, and DES helpers
Models/              Server-side map, mission, and map-data models
Networking/          Packet helpers, opcodes, and network message containers
Pages/               Razor web console
protocol.game/       Game protocol DTOs
protocol.map/        Map protocol DTOs
Resources/           Bundled configuration files
Services/            Runtime service helpers
Utilities/           Shared utilities and version helpers
wwwroot/             Static web assets
```

## Development Notes

- Avoid changing `protocol.game` and `protocol.map` unless protocol compatibility requires it.
- Add new game HTTP handlers under `BoxmakerProxy/`.
- Add account, map, mission, and player-state behavior under `AccountManager/`.
- Prefer cached map helpers for hot paths instead of scanning map files during requests.
- Keep stored-data compatibility in mind whenever models change.

## Companion Client Resources

- [Boxmaker client source](https://github.com/Caritusy/Boxmaker)
- [Boxmaker client releases](https://github.com/Caritusy/Boxmaker-Release/releases)

## License

See the license files included in this repository.
