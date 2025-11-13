using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AstraLauncher.Helpers;
using AstraLauncher.Models;

namespace AstraLauncher.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    public ObservableCollection<GameProfile> Profiles { get; } = new();
    public ObservableCollection<NewsArticle> MinecraftNews { get; } = new();
    public ObservableCollection<NewsArticle> LauncherNews { get; } = new();

    private bool _isUpdateDialogOpen;
    private UpdateInfo? _pendingUpdate;

    public bool IsUpdateDialogOpen
    {
        get => _isUpdateDialogOpen;
        set
        {
            _isUpdateDialogOpen = value;
            RaisePropertyChanged();
        }
    }

    public UpdateInfo? PendingUpdate
    {
        get => _pendingUpdate;
        set
        {
            _pendingUpdate = value;
            RaisePropertyChanged();
        }
    }

    public GameProfile? ActiveProfile => _mainViewModel.SelectedProfile;

    public RelayCommand PlayCommand { get; }
    public RelayCommand SignInCommand { get; }
    public RelayCommand CloseUpdateCommand { get; }
    public RelayCommand DownloadUpdateCommand { get; }
    public RelayCommand SelectProfileCommand { get; }

    public DashboardViewModel(MainViewModel mainViewModel,
        System.Collections.Generic.IEnumerable<GameProfile> profiles,
        System.Collections.Generic.IEnumerable<NewsArticle> minecraftNews,
        System.Collections.Generic.IEnumerable<NewsArticle> launcherNews)
    {
        _mainViewModel = mainViewModel;

        foreach (var profile in profiles)
        {
            Profiles.Add(profile);
        }

        foreach (var article in minecraftNews.OrderByDescending(n => n.ParsedDate))
        {
            MinecraftNews.Add(article);
        }

        foreach (var article in launcherNews.OrderByDescending(n => n.ParsedDate))
        {
            LauncherNews.Add(article);
        }

        PlayCommand = new RelayCommand(async () => await _mainViewModel.LaunchAsync());
        SignInCommand = new RelayCommand(async () => await _mainViewModel.SignInAsync());
        CloseUpdateCommand = new RelayCommand(() => IsUpdateDialogOpen = false);
        DownloadUpdateCommand = new RelayCommand(() => PendingUpdate?.OpenInBrowser());
        SelectProfileCommand = new RelayCommand(profile =>
        {
            if (profile is GameProfile gameProfile)
            {
                _mainViewModel.SelectedProfile = gameProfile;
                RaisePropertyChanged(nameof(ActiveProfile));
            }
        });

        if (_mainViewModel is INotifyPropertyChanged notifier)
        {
            notifier.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(MainViewModel.SelectedProfile))
                {
                    RaisePropertyChanged(nameof(ActiveProfile));
                }
            };
        }
    }

    public void ShowUpdateDialog(UpdateInfo update)
    {
        PendingUpdate = update;
        IsUpdateDialogOpen = true;
    }
}

internal static class UpdateInfoExtensions
{
    public static void OpenInBrowser(this UpdateInfo update)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = update.DownloadUrl,
            UseShellExecute = true
        });
    }
}
