# OpenScreenRecorder

A free, open-source screen recorder for Windows built with WPF and the Windows.Graphics.Capture API.

## Why?

Because paying $12.99/month for a screen recorder from a sketchy SaaS company is ridiculous. This is a portfolio project and a middle finger to predatory pricing models.

## Features

- Native Windows screen capture using Windows.Graphics.Capture
- Direct3D 11 hardware acceleration
- Built with .NET 8 and WPF
- Clean, simple interface
- No data collection, no subscriptions, no BS

## Requirements

- Windows 10/11
- .NET 8.0 Runtime
- DirectX 11 compatible GPU

## Building

```bash
git clone https://github.com/johnohhh1/OpenScreenRecorder.git
cd OpenScreenRecorder
dotnet build
```

## Architecture

- `OpenScreenRecorder.Core` - Core capture logic using Windows.Graphics.Capture and D3D11
- `OpenScreenRecorder.App` - WPF application with UI

## License

MIT - Do whatever you want with it. Fork it, improve it, ship it.

## Author

John Olenski (@johnohhh1)

---

*Built because sometimes the best response to a paywall is "hold my beer."*
