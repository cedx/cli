# Changelog

## Version [2.6.0](https://github.com/cedx/cli/compare/v2.5.0...v2.6.0)
- Updated the installer to allow the program to be installed in non administrative mode.
- Updated the installer to add the program to the `PATH` environment variable.

## Version [2.5.0](https://github.com/cedx/cli/compare/v2.4.0...v2.5.0)
- Updated the `iconv` command to exclude specific folders (`.git`, `node_modules` and `vendor`).

## Version [2.4.0](https://github.com/cedx/cli/compare/v2.3.0...v2.4.0)
- Removed the generic host.
- Updated the package dependencies.

## Version [2.3.0](https://github.com/cedx/cli/compare/v2.2.0...v2.3.0)
- Use a generic host and dependency injection to handle commands and services.

## Version [2.2.0](https://github.com/cedx/cli/compare/v2.1.0...v2.2.0)
- Ported the [NSSM](https://nssm.cc) configuration file to [XML](https://www.w3.org/XML) format.
- Renamed the `Belin.Cli.Nssm.Application` class to `WebApplication`.
- Use [Dapper](https://github.com/DapperLib/Dapper) to handle object mapping.

## Version [2.1.0](https://github.com/cedx/cli/compare/v2.0.0...v2.1.0)
- Added support for [C#](https://learn.microsoft.com/en-us/dotnet/csharp) applications to the `nssm install` command.

## Version [2.0.0](https://github.com/cedx/cli/compare/v1.2.3...v2.0.0)
- Breaking change: dropped support for **Linux** and **macOS** platforms.
- Breaking change: ported the source code to [C#](https://learn.microsoft.com/en-us/dotnet/csharp).
- Ported the build system to [Cake](https://cakebuild.net).

## Version [1.2.3](https://github.com/cedx/cli/compare/v1.2.2...v1.2.3)
- Fixed the `node` command: the [NSSM](https://nssm.cc) configuration file is now in [JSON](https://www.json.org) format.
- Ported the build system to [Cake](https://coffeescript.org/#cake).

## Version [1.2.2](https://github.com/cedx/cli/compare/v1.2.1...v1.2.2)
- Fixed the `php` command.

## Version [1.2.1](https://github.com/cedx/cli/compare/v1.2.0...v1.2.1)
- Fixed the the `jdk` and `node` commands.

## Version [1.2.0](https://github.com/cedx/cli/compare/v1.1.0...v1.2.0)
- Provide the `db-backup` and `db-restore` commands.

## Version [1.1.0](https://github.com/cedx/cli/compare/v1.0.1...v1.1.0)
- Provide the `db-charset`, `db-engine`, `db-optimize` and `iconv` commands.
- Fixed the log file path for `node` command.

## Version [1.0.1](https://github.com/cedx/cli/compare/v1.0.0...v1.0.1)
- Fixed the packaging.

## Version 1.0.0
- Provide the `jdk`, `node`, `nssm` and `php` commands.
