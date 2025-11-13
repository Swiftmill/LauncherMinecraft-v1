using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using AstraLauncher.Services;
using AstraLauncher.ViewModels;

namespace AstraLauncher;

public partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        InitializeComponent();

        var app = (App)Application.Current;
        var configuration = app.Configuration;
        var settingsService = app.SettingsService;

        var profileService = new ProfileService(configuration);
        profileService.Load();

        var accountService = new AccountService(configuration);
        accountService.Load();

        var newsService = new NewsService(configuration);
        var news = newsService.Load();

        var themeManager = new ThemeManager(settingsService);
        var versionService = new VersionService();
        var launchService = new LaunchService(settingsService);
        var updateService = new UpdateService(configuration);
        var authenticationService = new AuthenticationService(accountService);

        ViewModel = new MainViewModel(settingsService, profileService, accountService, news.minecraft, news.launcher, themeManager, versionService, launchService, updateService, authenticationService);
        DataContext = ViewModel;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (Resources["IntroAnimation"] is Storyboard storyboard)
        {
            storyboard.Begin(Root);
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}
