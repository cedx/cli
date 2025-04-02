# Changelog

## Version [2.2.0](https://github.com/cedx/cli/compare/v2.1.0...v2.2.0)
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
