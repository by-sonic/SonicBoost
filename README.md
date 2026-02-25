<div align="center">

# ⚡ SonicBoost

### Open-Source Windows Gaming Optimizer

[![Build](https://github.com/user/SonicBoost/actions/workflows/build-release.yml/badge.svg)](https://github.com/user/SonicBoost/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![Windows 10/11](https://img.shields.io/badge/Windows-10%20%7C%2011-0078D6.svg)](https://www.microsoft.com/windows)

**SonicBoost** is a free, open-source tool that optimizes Windows 10/11 for maximum gaming performance. One-click system tweaks, driver detection, bloatware removal, and privacy protection — all in a modern Fluent Design interface.

*Fast as Sonic. Powerful as a boost.*

</div>

---

## Features

### Gaming Tweaks
- Disable Game Bar, Game DVR, and fullscreen optimizations
- Enable Hardware-Accelerated GPU Scheduling
- Set high GPU/CPU priority for games
- Disable mouse acceleration, animations, transparency
- High timer resolution for smoother frame pacing

### Service Manager
- Safely disable 18+ unnecessary Windows services
- Categorized by risk level (Safe / Caution)
- One-click "Disable All Safe" button

### Privacy & Telemetry
- Disable Windows telemetry, advertising ID, activity history
- Block telemetry endpoints via hosts file
- Disable Cortana, Copilot, and Windows Recall
- 9 privacy tweaks with individual toggles

### Debloat
- Remove 26+ pre-installed UWP apps (Candy Crush, Clipchamp, etc.)
- Clean temporary files and Windows Update cache
- Free up disk space instantly

### Network Optimization
- Disable Nagle's Algorithm for lower ping
- Optimize TCP ACK frequency and disable network throttling
- Quick DNS switching (Cloudflare, Google, Quad9, OpenDNS)

### Driver Manager
- Auto-detect GPU, chipset, network, and audio devices via WMI
- Direct links to official driver download pages (NVIDIA, AMD, Intel, etc.)
- Shows current driver versions

### Power Plan
- Activate Ultimate Performance or High Performance power plan
- Disable hibernation to free disk space
- View and switch between all available power plans

### System Dashboard
- Real-time hardware detection (CPU, GPU, RAM, motherboard, storage)
- Optimization score showing current system state
- Beautiful dark theme with Fluent Design

## Installation

### Download (Recommended)

1. Go to [**Releases**](https://github.com/user/SonicBoost/releases)
2. Download `SonicBoost.exe`
3. Run as Administrator

No .NET installation required — the app is self-contained.

### Build from Source

```bash
git clone https://github.com/user/SonicBoost.git
cd SonicBoost
dotnet build SonicBoost.sln
```

To publish a single-file .exe:
```bash
dotnet publish src/SonicBoost/SonicBoost.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Requirements

- Windows 10 (1903+) or Windows 11
- Administrator privileges (required for registry and service modifications)

## Tech Stack

- **C# / .NET 8** — native Windows performance
- **WPF + [WPF UI](https://github.com/lepoco/wpfui)** — Fluent Design with dark theme
- **CommunityToolkit.Mvvm** — MVVM pattern with source generators
- **System.Management** — WMI hardware detection

## Safety

SonicBoost creates automatic backups before applying any changes. You can restore your original settings at any time.

> **Warning**: Always create a system restore point before making system-wide changes. While SonicBoost is designed to be safe, registry and service modifications carry inherent risks.

## Powered by SonicVPN

Protect your gaming connection with [**SonicVPN**](https://sonicvpn.com) — zero-lag VPN optimized for gamers.

## Contributing

Contributions are welcome! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

<div align="center">

**Made with speed by the Sonic community**

</div>
