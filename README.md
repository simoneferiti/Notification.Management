# Notification.Management

A Windows desktop application (WinUI 3 / Windows App SDK) that dispatches notifications to multiple, independently configurable channels (on-screen display, email, log file). The solution follows a layered architecture with a clean separation between domain contracts, business logic, presentation, and UI shell, and is covered by unit tests.

## Architecture

The solution is split into five projects, each with a single responsibility:

| Project | Responsibility |
|---|---|
| **Notification.Domain** (`Notification.Core`) | Framework-agnostic core: domain models (`NotificationEvent`), configuration types (`ChannelConfig`), and the contracts (`INotificationChannel`, `INotificationDispatcher`, `IChannelConfigLoader`, `IFileWriter`) that the rest of the solution depends on. No dependency on UI or infrastructure. |
| **Notification.Service** | Implements the domain contracts: `NotificationDispatcher` (fan-out engine) and the concrete channels (`DisplayChannel`, `EmailChannel`, `LogFileChannel`), plus infrastructure abstractions (`ISmtpClient`, `FileWriter`). Depends only on `Notification.Domain`. |
| **Notification.ViewModel** | MVVM view models (built with `CommunityToolkit.Mvvm`) that expose dispatcher state and commands to the UI, decoupled from the WinUI dispatcher via an `IUiDispatcher` abstraction. |
| **Notification.UI** | WinUI 3 application shell (views, app startup, dependency injection composition root) and the MSIX packaging project used to produce an installable app. |
| **Notification.Test** | Unit tests (NUnit + Moq) covering the dispatcher and the individual channels. |

### Key design choices

- **Dependency inversion / ports & adapters**: `Notification.Domain` defines only interfaces and models; `Notification.Service` provides the implementations. This keeps the core logic testable and lets channels or infrastructure (e.g. the SMTP client) be swapped or mocked without touching business logic.
- **Strategy pattern for channels**: every delivery mechanism implements `INotificationChannel` (`Name`, `ShouldHandle`, `DeliverAsync`). Adding a new channel means adding a new class and registering it in DI — the dispatcher requires no changes.
- **Parallel, fault-isolated dispatch**: `NotificationDispatcher` resolves all registered channels via `IEnumerable<INotificationChannel>`, filters them by configuration and `ShouldHandle`, and delivers to all active channels concurrently (`Task.WhenAll`). Each delivery is wrapped so an exception or slow channel (e.g. an unreachable SMTP server) never blocks the UI thread or prevents delivery on the other channels.
- **External, reloadable channel configuration**: enabled/disabled channels are stored in `channels.config.json` and loaded through `IChannelConfigLoader`, so channel behavior can be changed without recompiling.
- **MVVM with `CommunityToolkit.Mvvm`**: view models are UI-framework-agnostic business logic for the view layer; an `IUiDispatcher` abstraction marshals updates back to the UI thread, keeping view models testable outside of WinUI.
- **Dependency Injection as composition root**: `Notification.UI`'s `App.xaml.cs` wires up all services, channels, and view models via `Microsoft.Extensions.DependencyInjection`, keeping construction concerns out of the lower layers.
- **Testability by construction**: because channels and infrastructure are behind interfaces, `Notification.Test` can verify dispatcher and channel behavior using `Moq` fakes, without any UI or real network/file I/O.

## Prerequisites

- Windows 10 (build 17763+) or Windows 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Windows App SDK](https://learn.microsoft.com/windows/apps/windows-app-sdk/) workload (installed automatically via NuGet restore)
- Visual Studio 2022 (17.8+) with the **.NET Desktop Development** and **Windows App SDK / WinUI application development** workloads — required to build/run the WinUI projects (`Notification.UI`, `Notification.ViewModel`) and the MSIX packaging project

## Building

### Visual Studio (recommended for the full app, including MSIX packaging)

1. Open `Notification.Management.slnx` in Visual Studio 2022.
2. Select a target platform (`x86`, `x64`, or `ARM64`) — WinUI/MSIX projects require an explicit platform, not "Any CPU".
3. Restore NuGet packages (Visual Studio does this automatically on load/build).
4. Build the solution (**Build > Build Solution**) or press `F5` to run `Notification.UI`.

### .NET CLI (core, service, and test projects)

The platform-agnostic class libraries can be built and tested from the command line:

```powershell
dotnet build Notification.Domain\Notification.Core.csproj
dotnet build Notification.Service\Notification.Service.csproj
dotnet test Notification.Test\Notification.Test.csproj
```

> The WinUI/MSIX projects (`Notification.UI`, `Notification.ViewModel`, the packaging project) target `net8.0-windows10.0.19041.0` and require the Windows App SDK workload and an explicit platform (e.g. `-p:Platform=x64`); building them is best done through Visual Studio.

## Running the app

Set `Notification.UI` (or its packaging project, for an installable MSIX build) as the startup project in Visual Studio and run it with `F5`/`Ctrl+F5`.

## Configuration

Notification channels are toggled at runtime via `channels.config.json` (deployed alongside `Notification.UI`):

```json
{
  "channels": {
    "display": true,
    "email": true,
    "logFile": true
  }
}
```

Set a channel to `false` to disable it without recompiling; the dispatcher reads this file at startup through `IChannelConfigLoader`.

## Testing

```powershell
dotnet test Notification.Test\Notification.Test.csproj
```

Tests use **NUnit** as the test framework and **Moq** for mocking channel/infrastructure dependencies (e.g. `ISmtpClient`, `IFileWriter`).

## Project structure

```
Notification.Management.slnx
├── Notification.Domain/      # Core models, interfaces, config (Notification.Core)
├── Notification.Service/     # Dispatcher + channel implementations
├── Notification.ViewModel/   # MVVM view models
├── Notification.UI/          # WinUI 3 app + MSIX packaging project
├── Notification.Test/        # NUnit/Moq unit tests
└── channels.config.json      # Runtime channel enable/disable configuration
```
