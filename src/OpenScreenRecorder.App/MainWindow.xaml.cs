using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using OpenScreenRecorder.Core;
using Windows.Graphics.Capture;

namespace OpenScreenRecorder.App;

public partial class MainWindow : Window
{
    private readonly Vm _vm = new();
    private readonly CaptureController _cap = new();
    private GraphicsCaptureItem? _item;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
        _vm.Status = "Ready. Click Pick Source.";
    }

    private async void PickSource_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _item = await CapturePicker.PickAsync(this);
            _vm.Status = _item is not null ? "Source selected. Click Start." : "No source selected.";
        }
        catch (Exception ex)
        {
            _vm.Status = $"Pick failed: {ex.Message}";
        }
    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_item is null) throw new InvalidOperationException("Pick a source first.");
            _cap.Start(_item);
            _vm.Status = "Capturing… Click Stop to finish.";
        }
        catch (Exception ex)
        {
            _vm.Status = $"Start failed: {ex.Message}";
        }
    }

    private void Stop_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _cap.Stop();
            _vm.Status = $"Stopped. Frames captured: {_cap.FramesCaptured}";
        }
        catch (Exception ex)
        {
            _vm.Status = $"Stop failed: {ex.Message}";
        }
    }
}

public sealed class Vm : INotifyPropertyChanged
{
    private string _status = "";
    public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
