using Vortice.Direct3D11;
using Windows.Graphics.DirectX.Direct3D11;

namespace OpenScreenRecorder.Core;

internal static class D3D11Device
{
    public static IDirect3DDevice Create()
    {
        using var d3d = D3D11.D3D11CreateDevice(
            null,
            DriverType.Hardware,
            DeviceCreationFlags.BgraSupport);

        // Vortice provides this helper for WinRT interop
        return Vortice.Win32.Windows.Graphics.DirectX.Direct3D11.Direct3D11Helper.CreateDirect3DDevice(d3d.NativePointer);
    }
}
