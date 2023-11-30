using CapW.Areas.Stuff;
using CapW.Navigation;

namespace CapW;

public partial class App : Application
{
    public App()
    {
        this.UnhandledException += (sender, e) =>
        {
            // Log or inspect e.Exception
            System.Diagnostics.Debug.WriteLine(e.Exception.Message);
        };

        this.InitializeComponent();

        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<App>(this)
                .AddSingleton<MainWindow>()
                .AddSingleton<ShellPage>()
                .AddSingleton<ShellViewModel>()
                .AddTransient<HomePage>()
                .AddTransient<HomePageViewModel>()
                .AddSingleton<SettingsPage>()
                .AddSingleton<SettingsPageViewModel>()
                .AddSingleton<SomeStuffPage>()
                .AddSingleton<SomeStuffViewModel>()
                .AddSingleton<IStorageService, StorageService>()
                .AddSingleton<ThemeService>()
                .AddSingleton<INavigationService>(_ =>
                {
                    return new NavigationService(
                        navigationItems: [
                            new NavigationItemViewModel<HomePage>("Home", "\uE80F"),
                            new NavigationItemViewModel<SomeStuffPage>("Stuff", "\uE726"),
                            new NavigationItemViewModel<HomePage>("Children", "\uEA37", children: [
                                new NavigationItemViewModel<HomePage>("Child One", "\uE726"),
                                new NavigationItemViewModel<HomePage>("Child Two", "\uE726"),
                                new NavigationItemViewModel<HomePage>("Child Three", "\uE726", children: [
                                    new NavigationItemViewModel<HomePage>("Grandchild One", "\uE726"),
                                    new NavigationItemViewModel<HomePage>("Grandchild Two", "\uE726"),
                                ])
                            ])
                        ],
                        footerNavigationItems: [
                            new NavigationItemViewModel<HomePage>("Away Home", "\uE80F")
                        ],
                        settings: new NavigationItemViewModel<SettingsPage>("Settings", "\uE726")
                    );
                })
                .BuildServiceProvider()
            );

        // We can only set the application theme during construction.
        var themeService = Ioc.Default.GetRequiredService<ThemeService>();
        this.RequestedTheme = themeService.GetSavedApplicationTheme();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        mainWindow.Content = Ioc.Default.GetRequiredService<ShellPage>();

        // We still need to set the window caption buttons (minimize, maximize, close) to the correct colors.
        // We need the window to be instantiated before we can do this.
        var themeService = Ioc.Default.GetRequiredService<ThemeService>();
        themeService.UpdateTheme(mainWindow, themeService.GetSavedElementTheme());

        mainWindow.Activate();
    }

}
