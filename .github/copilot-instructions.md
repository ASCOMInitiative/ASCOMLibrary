# ASCOMLibrary Copilot Instructions

## Build and test commands

```powershell
# Build the full solution
dotnet build .\ASCOMLibrary.sln -c Debug -v minimal

# Build one package project
dotnet build .\ASCOM.Alpaca\ASCOM.Alpaca.csproj -c Debug -v minimal

# Run the unit test suite for the net8.0 target
dotnet test .\UnitTests\UnitTests.csproj -c Debug -f net8.0 --no-build -v minimal

# Run one xUnit test by fully qualified name
dotnet test .\UnitTests\UnitTests.csproj -c Debug -f net8.0 --no-build --filter "FullyQualifiedName~TraceLoggerMultiThreadedTests.ConcurrentWritesFromMultipleThreads" -v minimal
```

There is no dedicated lint command in the repository. Use `.editorconfig` as the style source of truth and rely on `dotnet build` for compiler/analyzer feedback.

## Unit tests and the `Tester` project
- Unit tests are the primary form of automated verification in the repository.
- The `Tester` project is a manual harness for ad hoc experimentation and should not be confused with the test suite.
- The full unit test suite takes about 8 minutes to run, so prefer targeted test runs during development. 
- The `FullyQualifiedName` filter is the most precise way to select tests, and the existing test documentation in `UnitTests\TraceLogger\` already uses FQNs.

## High-level architecture

- `ASCOM.Common` is the foundation layer. It contains the shared device interfaces, enums, response types, helpers, and extension points that the rest of the solution builds on.
- `ASCOM.Tools` sits on top of `ASCOM.Common` and provides shared runtime utilities such as `ILogger`, `TraceLogger`, `ConsoleLogger`, `XMLProfile`, and astronomy-oriented helpers.
- `ASCOM.Alpaca` is the client-side Alpaca stack. Its main responsibilities are typed Alpaca clients and discovery. `ASCOM.Alpaca.Device` is the device-side discovery package.
- `ASCOM.Com` is the Windows COM integration layer. It depends on `ASCOM.Common` and `ASCOM.Tools` and is the package that bridges into installed ASCOM Platform drivers and profile data.
- `ASCOM.Com.ChooserSA` is a Windows Forms chooser package that references both `ASCOM.Alpaca` and `ASCOM.Com`, so it is the main place where Alpaca and COM selection are unified behind one UI surface.
- `ASCOM.AstrometryTools` is the native-wrapper layer for SOFA and NOVAS. It depends on `ASCOM.Common` and `ASCOM.Tools`, packages native runtime assets for multiple RIDs, and ships supporting data such as `JPLEPH`.
- `UnitTests` references the major libraries directly. It also copies the Windows native astrometry binaries into the test output so tests can exercise `ASCOM.AstrometryTools` and `TraceLogger` behavior end to end.
- `Tester` is a manual net8.0 x64 harness for astrometry/tools work, not the automated test suite.

## Key repository conventions

- Shared package versioning and the default library target frameworks are centralized in `Directory.Build.props` (`Version` and `LibraryFrameworks`). For the core libraries, update those shared properties before changing individual project files.
- The core package projects are intentionally multi-targeted (`netstandard2.0;net8.0;net9.0;net10.0`). Keep code in shared libraries compatible with `netstandard2.0` unless you are working in a Windows-specific project such as `ASCOM.Com.ChooserSA`.
- Most library projects are pack-first: `GeneratePackageOnBuild` is enabled, the package icon is shared from the repo root, and each NuGet package embeds its local `README.md`. Packaging metadata is maintained in the project files, not in a separate packaging layer.
- `ASCOM.Com` is Windows-specific in practice even though the repo is cross-platform overall. The root README notes that COM components will not behave as expected unless the ASCOM Platform is installed.
- `ASCOM.AstrometryTools` is not just managed code; it depends on packaged native SOFA/NOVAS libraries plus supporting data files. When changing astrometry code, also check the runtime asset entries and any test or harness project copy-to-output items.
- Tests use xUnit and the repository already documents filtering by `FullyQualifiedName`. Prefer FQN filters when running targeted tests because that matches the existing test docs in `UnitTests\TraceLogger\`.
- The style baseline comes from `.editorconfig`: block-scoped namespaces, using directives outside namespaces, braces preferred, four-space indentation, and CRLF line endings.
