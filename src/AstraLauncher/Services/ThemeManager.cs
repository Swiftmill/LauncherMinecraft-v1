using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using AstraLauncher.Models;

namespace AstraLauncher.Services;

public class ThemeManager
{
    private readonly SettingsService _settingsService;
    private static readonly Uri LightUri = new("Resources/Themes/LightTheme.xaml", UriKind.Relative);
    private static readonly Uri DarkUri = new("Resources/Themes/DarkTheme.xaml", UriKind.Relative);

    public ThemeManager(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public void ApplyTheme(string theme)
    {
        var actualTheme = theme.ToLowerInvariant() switch
        {
            "dark" => Theme.Dark,
            "light" => Theme.Light,
            _ => DetectSystemTheme()
        };

        var dictionaries = Application.Current.Resources.MergedDictionaries;
        RemoveDictionary(dictionaries, LightUri);
        RemoveDictionary(dictionaries, DarkUri);

        var selectedUri = actualTheme == Theme.Dark ? DarkUri : LightUri;
        dictionaries.Insert(0, new ResourceDictionary { Source = selectedUri });

        Application.Current.Resources["AppTheme"] = actualTheme;
    }

    private static Theme DetectSystemTheme()
    {
        if (SystemParameters.WindowGlassBrush is SolidColorBrush brush)
        {
            return brush.Color.R < 128 ? Theme.Dark : Theme.Light;
        }
        return Theme.Light;
    }

    private static void RemoveDictionary(Collection<ResourceDictionary> dictionaries, Uri source)
    {
        var existing = dictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Equals(source.OriginalString, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            dictionaries.Remove(existing);
        }
    }
}

public enum Theme
{
    Light,
    Dark
}
