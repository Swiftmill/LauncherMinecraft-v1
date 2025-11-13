using System;
using System.IO;

namespace AstraLauncher.Services;

public static class LoggingService
{
    private static readonly object LockObj = new();
    private static string _logsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AstraLauncher", "logs");

    public static void Configure(string? directory)
    {
        if (!string.IsNullOrWhiteSpace(directory))
        {
            _logsDirectory = Environment.ExpandEnvironmentVariables(directory);
        }

        Directory.CreateDirectory(_logsDirectory);
    }

    public static void Log(string message)
    {
        Write("INFO", message);
    }

    public static void LogWarning(string message)
    {
        Write("WARN", message);
    }

    public static void LogError(string message)
    {
        Write("ERROR", message);
    }

    public static void LogException(Exception ex)
    {
        Write("EXCEPTION", $"{ex.Message}\n{ex.StackTrace}");
    }

    private static void Write(string level, string message)
    {
        lock (LockObj)
        {
            Directory.CreateDirectory(_logsDirectory);
            var file = Path.Combine(_logsDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}.log");
            File.AppendAllText(file, $"[{DateTime.UtcNow:O}] [{level}] {message}{Environment.NewLine}");
        }
    }
}
