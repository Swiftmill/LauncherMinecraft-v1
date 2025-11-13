using System;
using System.Collections.ObjectModel;
using System.Linq;
using AstraLauncher.Helpers;
using AstraLauncher.Models;
using AstraLauncher.Services;

namespace AstraLauncher.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private readonly ThemeManager _themeManager;

    public ObservableCollection<string> Themes { get; } = new(new[] { "light", "dark", "auto" });
    public ObservableCollection<string> Languages { get; } = new(new[] { "fr-FR", "en-US" });

    public int MinRam
    {
        get => _settingsService.Settings.Game.MinRamMb;
        set
        {
            _settingsService.Settings.Game.MinRamMb = value;
            RaisePropertyChanged();
        }
    }

    public int MaxRam
    {
        get => _settingsService.Settings.Game.MaxRamMb;
        set
        {
            _settingsService.Settings.Game.MaxRamMb = value;
            RaisePropertyChanged();
        }
    }

    public string Theme
    {
        get => _settingsService.Settings.Theme;
        set
        {
            _settingsService.Settings.Theme = value;
            _themeManager.ApplyTheme(value);
            RaisePropertyChanged();
        }
    }

    public string Language
    {
        get => _settingsService.Settings.Language;
        set
        {
            _settingsService.Settings.Language = value;
            RaisePropertyChanged();
        }
    }

    public string LogsDirectory
    {
        get => _settingsService.Settings.LogsDirectory;
        set
        {
            _settingsService.Settings.LogsDirectory = value;
            RaisePropertyChanged();
        }
    }

    public string GameDataRoot
    {
        get => _settingsService.Settings.GameDataRoot;
        set
        {
            _settingsService.Settings.GameDataRoot = value;
            RaisePropertyChanged();
        }
    }

    public string JvmArguments
    {
        get => _settingsService.Settings.Game.JvmArguments;
        set
        {
            _settingsService.Settings.Game.JvmArguments = value;
            RaisePropertyChanged();
        }
    }

    public RelayCommand SaveCommand { get; }

    public SettingsViewModel(SettingsService settingsService, ThemeManager themeManager)
    {
        _settingsService = settingsService;
        _themeManager = themeManager;
        SaveCommand = new RelayCommand(() => _settingsService.Save());
    }
}
