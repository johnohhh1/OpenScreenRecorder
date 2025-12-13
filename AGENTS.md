# AGENTS.md (repo-wide)

This file is instructions + a running log for future work on `OpenScreenRecorder`.

## Ground rules

- Keep changes scoped to the user request; don’t refactor unrelated code.
- Prefer small commits/patches that keep the app runnable after each change.
- Always validate with a real build + launch on Windows.

## Repo state (as of 2025-12-13)

- `OpenScreenRecorder.App` is a WPF UI that records via `ScreenRecorderLib` (Media Foundation).
  - **Simplified UI**: The main window is now a compact "Snipping Tool" style interface (fixed 480x150 size).
  - **Defaults**: Hardcoded to 60 FPS / 10 Mbps / Auto-save to `Videos/OpenScreenRecorder`.
- `OpenScreenRecorder.Core` still contains early `Windows.Graphics.Capture` frame-counter code; it must compile but is not used by the current recorder UI.
- `ScreenRecorderLib` requires a concrete platform (`x64`, `x86`, `ARM64`) — **AnyCPU will fail**.
  - Solution (`OpenScreenRecorder.sln`) maps `Any CPU` configs to `x64` so `dotnet build` works without extra flags.
- Output defaults to: `%USERPROFILE%\Videos\OpenScreenRecorder\Recording_*.mp4`
- Recorder log defaults to: `%LOCALAPPDATA%\OpenScreenRecorder\recorder.log`

## Build / run (Windows)

### Use these exact commands

- Build:
  - `C:\Progra~1\dotnet\dotnet.exe build OpenScreenRecorder.sln`
- Run via dotnet:
  - `C:\Progra~1\dotnet\dotnet.exe run --project src\OpenScreenRecorder.App`
- Run the built EXE directly (fastest):
  - `src\OpenScreenRecorder.App\bin\x64\Debug\net9.0-windows10.0.26100.0\win-x64\OpenScreenRecorder.App.exe`

### Common gotchas

- If build fails with “file is locked”, the app is still running:
  - `C:\Windows\System32\taskkill.exe /IM OpenScreenRecorder.App.exe /F`
- If `dotnet` isn’t on PATH, use `C:\Progra~1\dotnet\dotnet.exe` (or add `C:\Program Files\dotnet` to PATH).

## Runtime prerequisites (important)

`ScreenRecorderLib` uses native Media Foundation + VC++ runtime. If the app “doesn’t open” or exits instantly:

- Install **Microsoft Visual C++ Redistributable (x64)**:
  - https://aka.ms/vs/17/release/vc_redist.x64.exe
- On Windows **N/KN** editions, install the Media Feature Pack (Media Foundation).
- Check `%LOCALAPPDATA%\OpenScreenRecorder\recorder.log` for the error cause.

## UI theming note

- WPF default theme can ignore `Background/Foreground` on `ComboBox` unless a custom `ControlTemplate` is used.
- `src/OpenScreenRecorder.App/MainWindow.xaml` contains custom templates for `ComboBox` + `TextBox` to avoid “white on white”.

## Completed Work

- "Add a snipping-tool-like flow": Implemented simplified UI (Pick → Record → Stop) and removed complex options.

## Current user feedback / issues to prioritize

- “App doesn’t open when I click it” (likely missing VC++ runtime or immediate crash; check Event Viewer + recorder log).
- “Doesn’t record / doesn’t save / hard to find output”:
  - Ensure the recorder start/stop success/failure dialogs are visible and output directory exists.


