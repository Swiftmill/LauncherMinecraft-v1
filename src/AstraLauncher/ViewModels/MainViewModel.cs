using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AstraLauncher.Helpers;
using AstraLauncher.Models;
using AstraLauncher.Services;

namespace AstraLauncher.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private readonly ProfileService _profileService;
    private readonly AccountService _accountService;
    private readonly VersionService _versionService;
    private readonly LaunchService _launchService;
    private readonly UpdateService _updateService;
    private readonly AuthenticationService _authenticationService;

    private ViewModelBase? _currentView;
    private GameProfile? _selectedProfile;
    private string _statusMessage = "Prêt";
    private double _progress;
    private ImageSource? _logoImageSource;

    public ObservableCollection<NavigationItemViewModel> NavigationItems { get; } = new();
    public ObservableCollection<Account> Accounts { get; } = new();

    public ViewModelBase? CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            RaisePropertyChanged();
        }
    }

    public GameProfile? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            _selectedProfile = value;
            _settingsService.Settings.DefaultProfileId = value?.Id ?? string.Empty;
            RaisePropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            RaisePropertyChanged();
        }
    }

    public double Progress
    {
        get => _progress;
        set
        {
            _progress = value;
            RaisePropertyChanged();
        }
    }

    public ImageSource? LogoImageSource
    {
        get => _logoImageSource;
        private set
        {
            if (_logoImageSource != value)
            {
                _logoImageSource = value;
                RaisePropertyChanged();
            }
        }
    }

    public RelayCommand NavigateCommand { get; }
    public RelayCommand CloseCommand { get; }
    public RelayCommand MinimizeCommand { get; }
    public RelayCommand ToggleMaximizeCommand { get; }

    public DashboardViewModel Dashboard { get; }
    public SettingsViewModel Settings { get; }
    public DiagnosticsViewModel Diagnostics { get; }

    public MainViewModel(SettingsService settingsService,
        ProfileService profileService,
        AccountService accountService,
        System.Collections.Generic.IEnumerable<NewsArticle> minecraftNews,
        System.Collections.Generic.IEnumerable<NewsArticle> launcherNews,
        ThemeManager themeManager,
        VersionService versionService,
        LaunchService launchService,
        UpdateService updateService,
        AuthenticationService authenticationService)
    {
        _settingsService = settingsService;
        _profileService = profileService;
        _accountService = accountService;
                _versionService = versionService;
        _launchService = launchService;
        _updateService = updateService;
        _authenticationService = authenticationService;

        Dashboard = new DashboardViewModel(this, profileService.Profiles, minecraftNews, launcherNews);
        Settings = new SettingsViewModel(settingsService, themeManager);
        Diagnostics = new DiagnosticsViewModel();

        NavigationItems.Add(new NavigationItemViewModel("dashboard", "🏠", "Accueil"));
        NavigationItems.Add(new NavigationItemViewModel("settings", "⚙️", "Paramètres"));
        NavigationItems.Add(new NavigationItemViewModel("diagnostics", "🩺", "Diagnostic"));

        NavigateCommand = new RelayCommand(param => Navigate(param?.ToString()));
        CloseCommand = new RelayCommand(() => Application.Current.Shutdown());
        MinimizeCommand = new RelayCommand(() => Application.Current.MainWindow!.WindowState = WindowState.Minimized);
        ToggleMaximizeCommand = new RelayCommand(() =>
        {
            var window = Application.Current.MainWindow!;
            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        });

        foreach (var account in _accountService.Accounts)
        {
            Accounts.Add(account);
        }

        SelectedProfile = _profileService.GetDefaultProfile(_settingsService.Settings.DefaultProfileId) ?? _profileService.Profiles.FirstOrDefault();
        CurrentView = Dashboard;

        LoadBrandingAssets();

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await CheckForUpdatesAsync();
    }

    private void LoadBrandingAssets()
    {
        try
        {
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var assetsDirectory = Path.Combine(appDirectory, "assets");
            var logoPath = Path.Combine(assetsDirectory, "logo.png");

            if (!File.Exists(logoPath))
            {
                logoPath = Path.Combine(assetsDirectory, "logo.txt");
            }

            if (!File.Exists(logoPath) || string.Equals(Path.GetExtension(logoPath), ".txt", StringComparison.OrdinalIgnoreCase))
            {
                LogoImageSource = null;
                return;
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(logoPath, UriKind.Absolute);
            image.EndInit();
            image.Freeze();
            LogoImageSource = image;
        }
        catch
        {
            LogoImageSource = null;
        }
    }

    private void Navigate(string? identifier)
    {
        CurrentView = identifier switch
        {
            "settings" => Settings,
            "diagnostics" => Diagnostics,
            _ => Dashboard
        };
    }

    public async Task LaunchAsync()
    {
        if (SelectedProfile == null)
        {
            return;
        }

        StatusMessage = "Préparation du jeu...";
        Progress = 10;
        await _versionService.EnsureVersionAsync(SelectedProfile);
        Progress = 30;
        await _versionService.DownloadDependenciesAsync(SelectedProfile);
        Progress = 60;
        var ok = await _versionService.VerifyIntegrityAsync(SelectedProfile);
        if (!ok)
        {
            StatusMessage = "Fichiers manquants";
            return;
        }
        Progress = 80;
        await _launchService.LaunchAsync(SelectedProfile);
        Progress = 100;
        StatusMessage = "Jeu lancé";
    }

    public async Task CheckForUpdatesAsync()
    {
        StatusMessage = "Recherche de mises à jour";
        var update = await _updateService.CheckForUpdatesAsync(_settingsService.Settings.LauncherVersion);
        if (update != null)
        {
            StatusMessage = $"Nouvelle version {update.Version}";
            Dashboard.ShowUpdateDialog(update);
        }
        else
        {
            StatusMessage = "Prêt";
        }
    }

    public async Task<Account> SignInAsync()
    {
        var account = await _authenticationService.SignInWithMicrosoftAsync();
        if (!Accounts.Any(a => a.Id == account.Id))
        {
            Accounts.Add(account);
        }
        return account;
    }
}

public record NavigationItemViewModel(string Identifier, string Icon, string Title);
