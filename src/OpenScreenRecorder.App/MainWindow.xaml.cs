using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using ScreenRecorderLib;

namespace OpenScreenRecorder.App;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(200) };
    private DateTimeOffset? _recordingStart;
    private string? _lastRecordingPath;
    private string _logPath = "";
    private Recorder? _recorder;
    private bool _isRecording = false;

    public MainWindow()
    {
        InitializeComponent();
        _timer.Tick += (_, _) => UpdateTimer();

        PopulateSources();
        // Defaults
        ResultPanel.Visibility = Visibility.Collapsed;
        SetStatus("Ready.");
    }

    private void PopulateSources()
    {
        var sources = new List<SourceOption>();
        foreach (var display in Recorder.GetDisplays()) sources.Add(SourceOption.FromDisplay(display));
        foreach (var window in Recorder.GetWindows()) sources.Add(SourceOption.FromWindow(window));

        SourceCombo.ItemsSource = sources;
        SourceCombo.DisplayMemberPath = nameof(SourceOption.Label);
        if (sources.Count > 0) SourceCombo.SelectedIndex = 0;
    }

    private void Record_Click(object sender, RoutedEventArgs e)
    {
        if (_isRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }

    private void StartRecording()
    {
        try
        {
            if (SourceCombo.SelectedItem is not SourceOption source)
            {
                SetStatus("Select a screen first.");
                return;
            }

            ResultPanel.Visibility = Visibility.Collapsed;

            var baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "OpenScreenRecorder");
            Directory.CreateDirectory(baseDir);
            var fileName = $"Recording_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";
            var outputPath = Path.Combine(baseDir, fileName);

            _logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenScreenRecorder", "recorder.log");
            Directory.CreateDirectory(Path.GetDirectoryName(_logPath)!);

            var options = new RecorderOptions
            {
                SourceOptions = new SourceOptions { RecordingSources = new List<RecordingSourceBase> { source.Source } },
                AudioOptions = new AudioOptions
                {
                    IsAudioEnabled = SystemAudioCheck.IsChecked == true || MicAudioCheck.IsChecked == true,
                    IsOutputDeviceEnabled = SystemAudioCheck.IsChecked == true,
                    IsInputDeviceEnabled = MicAudioCheck.IsChecked == true,
                },
                VideoEncoderOptions = new VideoEncoderOptions
                {
                    Bitrate = 10 * 1_000_000, // 10 Mbps fixed
                    Framerate = 60,           // 60 FPS fixed
                    Encoder = new H264VideoEncoder { BitrateMode = H264BitrateControlMode.Quality, EncoderProfile = H264Profile.Main },
                    IsHardwareEncodingEnabled = true,
                    IsMp4FastStartEnabled = true
                },
                MouseOptions = new MouseOptions { IsMousePointerEnabled = true, IsMouseClicksDetected = true, MouseClickDetectionMode = MouseDetectionMode.Hook },
                LogOptions = new LogOptions { IsLogEnabled = true, LogFilePath = _logPath, LogSeverityLevel = ScreenRecorderLib.LogLevel.Debug }
            };

            _recorder = Recorder.CreateRecorder(options);
            _recorder.OnRecordingComplete += OnRecordingComplete;
            _recorder.OnRecordingFailed += OnRecordingFailed;
            _recorder.OnStatusChanged += OnRecorderStatusChanged;

            _recorder.Record(outputPath);
            _recordingStart = DateTimeOffset.Now;
            _timer.Start();
            
            _isRecording = true;
            RecordButton.Content = "Stop";
            RecordButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 77, 109)); // Red #ff4d6d
            SetStatus("Recording...");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to start: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CleanupRecorder();
        }
    }

    private void StopRecording()
    {
        try
        {
            RecordButton.IsEnabled = false;
            SetStatus("Finalizing...");
            _recorder?.Stop();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to stop: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CleanupRecorder();
        }
    }

    private void OnRecorderStatusChanged(object sender, RecordingStatusEventArgs e)
    {
        // Optional: Update status text if needed
    }

    private void OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            _lastRecordingPath = e.FilePath;
            _isRecording = false;
            
            // Reset UI
            RecordButton.Content = "Record";
            RecordButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(56, 182, 255)); // Blue #38b6ff
            RecordButton.IsEnabled = true;
            TimerText.Text = "00:00:00";
            _timer.Stop();
            _recordingStart = null;

            if (File.Exists(_lastRecordingPath))
            {
                SetStatus("Saved.");
                ResultPanel.Visibility = Visibility.Visible;
            }
            else
            {
                SetStatus("Error saving.");
            }

            CleanupRecorder();
        });
    }

    private void OnRecordingFailed(object sender, RecordingFailedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            _isRecording = false;
            RecordButton.Content = "Record";
            RecordButton.IsEnabled = true;
            SetStatus("Failed.");
            MessageBox.Show(this, $"Recording failed: {e.Error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CleanupRecorder();
        });
    }

    private void CleanupRecorder()
    {
        if (_recorder is null) return;
        _recorder.OnRecordingComplete -= OnRecordingComplete;
        _recorder.OnRecordingFailed -= OnRecordingFailed;
        _recorder.OnStatusChanged -= OnRecorderStatusChanged;
        _recorder.Dispose();
        _recorder = null;
    }

    private void UpdateTimer()
    {
        if (_recordingStart is null) return;
        var elapsed = DateTimeOffset.Now - _recordingStart.Value;
        TimerText.Text = elapsed.ToString(@"hh\:mm\:ss");
    }

    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        if (File.Exists(_lastRecordingPath))
            Process.Start(new ProcessStartInfo { FileName = _lastRecordingPath, UseShellExecute = true });
    }

    private void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        if (File.Exists(_lastRecordingPath))
            Process.Start(new ProcessStartInfo { FileName = "explorer.exe", Arguments = $"/select,\"{_lastRecordingPath}\"" });
    }
    
    // CopyPath_Click removed as it wasn't requested in new flow, but simple enough to re-add if needed.
    // Keeping it simple.

    private void SetStatus(string message) => StatusText.Text = message;
}

internal sealed record SourceOption(string Label, RecordingSourceBase Source)
{
    public static SourceOption FromDisplay(RecordableDisplay display) =>
        new($"{(string.IsNullOrWhiteSpace(display.FriendlyName) ? "Display" : display.FriendlyName)} (Display)", display);

    public static SourceOption FromWindow(RecordableWindow window) =>
        new($"{(string.IsNullOrWhiteSpace(window.Title) ? "Window" : window.Title)} (Window)", window);
}
