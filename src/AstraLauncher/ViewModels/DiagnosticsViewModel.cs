using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using AstraLauncher.Helpers;

namespace AstraLauncher.ViewModels;

public class DiagnosticsViewModel : ViewModelBase
{
    public RelayCommand OpenLogsCommand { get; }
    public RelayCommand CopySystemInfoCommand { get; }

    public DiagnosticsViewModel()
    {
        OpenLogsCommand = new RelayCommand(OpenLogs);
        CopySystemInfoCommand = new RelayCommand(CopySystemInfo);
    }

    private void OpenLogs()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AstraLauncher", "logs");
        Directory.CreateDirectory(folder);
        Process.Start(new ProcessStartInfo
        {
            FileName = folder,
            UseShellExecute = true
        });
    }

    private void CopySystemInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Système: {Environment.OSVersion}");
        sb.AppendLine($"Architecture: {Environment.Is64BitOperatingSystem}");
        sb.AppendLine($".NET: {Environment.Version}");
        sb.AppendLine($"Mémoire: {GC.GetTotalMemory(false) / 1024 / 1024} MB utilisés");

        ClipboardService.SetText(sb.ToString());
    }
}

internal static class ClipboardService
{
    public static void SetText(string text)
    {
        System.Windows.Clipboard.SetText(text);
    }
}
