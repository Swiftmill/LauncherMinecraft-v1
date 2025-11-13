using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AstraLauncher.Models;

namespace AstraLauncher.Services;

public class LaunchService
{
    private readonly SettingsService _settingsService;

    public LaunchService(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task LaunchAsync(GameProfile profile)
    {
        LoggingService.Log($"Préparation du lancement pour {profile.Name}");

        var gameDirectory = Environment.ExpandEnvironmentVariables(profile.GameDirectory);
        Directory.CreateDirectory(gameDirectory);

        // Placeholder command construction. Replace with actual Minecraft launcher invocation.
        var javaPath = "java";
        var arguments = $"-Xms{_settingsService.Settings.Game.MinRamMb}m -Xmx{_settingsService.Settings.Game.MaxRamMb}m {_settingsService.Settings.Game.JvmArguments} -jar minecraft-launcher.jar";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = javaPath,
                Arguments = arguments,
                WorkingDirectory = gameDirectory,
                UseShellExecute = false
            }
        };

        LoggingService.Log($"Commande Java: {javaPath} {arguments}");
        await Task.Run(() => process.Start());
    }
}
