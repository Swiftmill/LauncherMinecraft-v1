using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using AstraLauncher.Services;

using System;

namespace AstraLauncher;

public partial class App : Application
{
    private SettingsService? _settingsService;
    private ThemeManager? _themeManager;
    private JsonDocument? _configuration;

    public SettingsService SettingsService => _settingsService ?? throw new InvalidOperationException("SettingsService non initialisé.");
    public JsonDocument Configuration => _configuration ?? throw new InvalidOperationException("Configuration non initialisée.");

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var appDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var configPath = Path.Combine(appDir, "appsettings.json");
        _configuration = JsonDocument.Parse(File.ReadAllText(configPath));

        _settingsService = new SettingsService(_configuration);
        _settingsService.Load();

        var language = _settingsService.Settings.Language;
        if (!string.IsNullOrWhiteSpace(language))
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo(language);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }
            catch (CultureNotFoundException)
            {
                // fallback to system culture
            }
        }

        _themeManager = new ThemeManager(_settingsService);
        _themeManager.ApplyTheme(_settingsService.Settings.Theme);

        this.DispatcherUnhandledException += (_, args) =>
        {
            LoggingService.LogException(args.Exception);
            MessageBox.Show($"Une erreur inattendue est survenue:\n{args.Exception.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        _settingsService?.Save();
        _configuration?.Dispose();
    }
}
