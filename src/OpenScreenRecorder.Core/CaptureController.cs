using Windows.Graphics.Capture;

namespace OpenScreenRecorder.Core;

public sealed class CaptureController : IDisposable
{
    private FrameCounterSession? _session;

    public int FramesCaptured => _session?.FramesCaptured ?? 0;

    public void Start(GraphicsCaptureItem item)
    {
        _session?.Dispose();
        _session = new FrameCounterSession(item);
        _session.Start();
    }

    public void Stop()
    {
        _session?.Dispose();
        _session = null;
    }

    public void Dispose() => Stop();
}
