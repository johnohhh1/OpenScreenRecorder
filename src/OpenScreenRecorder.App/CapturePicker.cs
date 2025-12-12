using System.Windows;
using System.Windows.Interop;
using Windows.Graphics.Capture;
using WinRT.Interop;

namespace OpenScreenRecorder.App;

internal static class CapturePicker
{
    public static async Task<GraphicsCaptureItem?> PickAsync(Window owner)
    {
        var picker = new GraphicsCapturePicker();
        var hwnd = new WindowInteropHelper(owner).Handle;
        InitializeWithWindow.Initialize(picker, hwnd);
        return await picker.PickSingleItemAsync();
    }
}
