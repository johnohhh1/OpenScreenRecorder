# OpenScreenRecorder

A dead-simple, free, open-source screen recorder for Windows. Built because 150MB Electron apps and monthly subscriptions for screen recording are a crime.

## Why?

Because paying $12.99/month for a simple utility is ridiculous. This is a portfolio project and a middle finger to predatory pricing models.

**No ads. No watermarks. No time limits. No BS.**

## Features

- **Snipping Tool Flow**: Pick a screen, hit Record, hit Stop. Done.
- **Hardware Native**: Uses `Windows.Graphics.Capture` and Media Foundation for high-performance, low-overhead recording.
- **Auto-Save**: Recordings are automatically saved to `Videos/OpenScreenRecorder`.
- **System Audio + Mic**: Capture what you hear and what you say.
- **Portable**: Runs as a single `.exe` without installation if needed.

## Installation

### Option 1: Installer
Download `OpenScreenRecorder_Setup.exe`, run it, and you're good to go.

### Option 2: Portable
Download `OpenScreenRecorder_Portable.zip`, unzip it, and run `OpenScreenRecorder.App.exe`.

## Development

### Requirements
- Windows 10/11
- .NET 9 SDK
- [Inno Setup 6](https://jrsoftware.org/isinfo.php) (optional, for building the installer)

### Building form source

```powershell
git clone https://github.com/johnohhh1/OpenScreenRecorder.git
cd OpenScreenRecorder
dotnet build OpenScreenRecorder.sln
```

### Creating Releases

**1. Build Standalone EXE & Zip:**
```powershell
# Publish single-file executable
dotnet publish src\OpenScreenRecorder.App -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

# Create Portable Zip
Compress-Archive -Path "src\OpenScreenRecorder.App\bin\x64\Release\net9.0-windows10.0.26100.0\win-x64\publish\OpenScreenRecorder.App.exe" -DestinationPath "OpenScreenRecorder_Portable.zip" -Force
```

**2. Build Installer (`setup.exe`):**
```powershell
# Compiles setup.iss into OpenScreenRecorder_Setup.exe
# Ensure ISCC is in your PATH or use full path
iscc setup.iss
```

## Architecture

- `OpenScreenRecorder.App`: WPF UI (Simple Mode).
- `OpenScreenRecorder.Core`: Capture logic using Windows APIs.
- Built on top of the excellent `ScreenRecorderLib`.

## License

MIT - Do whatever you want with it. Fork it, improve it, ship it.

## Author

**John Olenski**  
[johnohhh1.dev](https://johnohhh1.dev) | [@johnohhh1](https://github.com/johnohhh1)

---

*Built because sometimes the best response to a paywall is "hold my coca-cola classic."*
