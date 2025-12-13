# OpenScreenRecorder

A free, open-source screen recorder for Windows built with WPF and the Windows.Graphics.Capture API.

## Why?

Because paying $12.99/month for a screen recorder from a sketchy SaaS company is ridiculous. This is a portfolio project and a middle finger to predatory pricing models.

## Features

- Record any display or window to MP4 (H.264) with hardware encoding
- System audio + microphone capture with click highlight overlay
- Automatic MP4 output to `Videos/OpenScreenRecorder`
- Simple UI: Pick source, Record, Stop
- Built with .NET 9, WPF, and Media Foundation (via ScreenRecorderLib)
- No data collection, no subscriptions, no BS

## Requirements

- Windows 10/11
- .NET 9 SDK (or runtime) installed
- DirectX 11 compatible GPU

## Building

```bash
git clone https://github.com/johnohhh1/OpenScreenRecorder.git
cd OpenScreenRecorder
dotnet build OpenScreenRecorder.sln
```

Run the WPF app from Visual Studio or:

```bash
dotnet run --project src/OpenScreenRecorder.App
```

## Usage

- Launch the app, pick a display/window from the drop-down.
- Hit **Record**.
- Hit **Stop** when done.
- Click **Open File** or **Show in Folder** to see your recording.

## Architecture

- `OpenScreenRecorder.Core` - Core capture logic using Windows.Graphics.Capture and D3D11
- `OpenScreenRecorder.App` - WPF application with UI

## License

MIT - Do whatever you want with it. Fork it, improve it, ship it.

## Author

John Olenski (@johnohhh1)

---

*Built because sometimes the best response to a paywall is "hold my coca-cola classic."*
