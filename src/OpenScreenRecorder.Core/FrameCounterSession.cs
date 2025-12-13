using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;

namespace OpenScreenRecorder.Core;

internal sealed class FrameCounterSession : IDisposable
{
    private readonly GraphicsCaptureItem _item;

    private IDirect3DDevice? _device;
    private Direct3D11CaptureFramePool? _framePool;
    private GraphicsCaptureSession? _session;
    private Windows.Graphics.SizeInt32 _currentSize;

    public int FramesCaptured { get; private set; }

    public FrameCounterSession(GraphicsCaptureItem item)
    {
        _item = item;
        _currentSize = item.Size;
    }

    public void Start()
    {
        _device = D3D11Device.Create();

        _framePool = Direct3D11CaptureFramePool.Create(
            _device,
            DirectXPixelFormat.B8G8R8A8UIntNormalized,
            2,
            _currentSize);

        _framePool.FrameArrived += OnFrameArrived;

        _session = _framePool.CreateCaptureSession(_item);
        _session.StartCapture();
    }

    private void OnFrameArrived(Direct3D11CaptureFramePool sender, object args)
    {
        using var frame = sender.TryGetNextFrame();
        FramesCaptured++;

        // Handle resizing
        if (frame.ContentSize.Width != _currentSize.Width || frame.ContentSize.Height != _currentSize.Height)
        {
            // Update stored size
            _currentSize = frame.ContentSize;
            sender.Recreate(_device!, DirectXPixelFormat.B8G8R8A8UIntNormalized, 2, frame.ContentSize);
        }
    }

    public void Dispose()
    {
        if (_framePool is not null)
            _framePool.FrameArrived -= OnFrameArrived;

        _session?.Dispose();
        _framePool?.Dispose();

        _session = null;
        _framePool = null;
        _device = null;
    }
}
