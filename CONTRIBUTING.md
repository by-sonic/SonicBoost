# Contributing to SonicBoost

Thank you for your interest in contributing to SonicBoost! Here's how you can help.

## Getting Started

1. Fork the repository
2. Clone your fork: `git clone https://github.com/your-username/SonicBoost.git`
3. Create a branch: `git checkout -b feature/your-feature`
4. Make your changes
5. Build and test: `dotnet build SonicBoost.sln`
6. Commit: `git commit -m "Add your feature"`
7. Push: `git push origin feature/your-feature`
8. Open a Pull Request

## Development Setup

- .NET 8 SDK
- Visual Studio 2022 or VS Code with C# extension
- Windows 10/11 (required for WPF development)

## Project Structure

```
src/
  SonicBoost/          # WPF Application (UI, ViewModels)
  SonicBoost.Core/     # Core logic (no UI dependencies)
```

## Guidelines

- Follow existing code style and naming conventions
- Add new tweaks to `TweakDefinitions.cs` or create new service files in Core
- All system modifications must go through `BackupService` for rollback support
- Test on both Windows 10 and Windows 11 when possible
- Keep the UI consistent with the existing Fluent Design theme

## Adding New Tweaks

1. Add a `TweakItem` to `TweakDefinitions.cs` (for gaming/performance tweaks)
2. Or add to the relevant service (Privacy, Network, etc.)
3. Include: ID, name, description, registry path/key, enabled/disabled values, risk level

## Reporting Issues

- Use GitHub Issues
- Include your Windows version and build number
- Describe what you expected vs what happened
- Include screenshots if relevant

## Code of Conduct

Be respectful and constructive. We're all here to make Windows gaming better.
