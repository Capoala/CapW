namespace CapW.Areas.Settings;

public sealed partial class SettingsPage : Page
{
    public SettingsPage(SettingsPageViewModel viewModel, MainWindow window, ThemeService themeService)
    {
        _viewModel = viewModel;
        _window = window;
        _themeService = themeService;

        this.DataContext = _viewModel;
        this.InitializeComponent();

        // Changes the UI, so do this before subscribing to changes.
        InitCurrentTheme();

        this.ApplicationThemeSelection.SelectionChanged += OnApplicationThemeSelected;
    }

    private readonly SettingsPageViewModel _viewModel;
    private readonly Window _window;
    private readonly ThemeService _themeService;

    private void OnApplicationThemeSelected(object sender, RoutedEventArgs e)
    {
        var selectedTheme = ((ComboBoxItem)ApplicationThemeSelection.SelectedItem)?.Tag?.ToString();

        if (Enum.TryParse<ElementTheme>(selectedTheme, out var theme))
        {
            var element = (FrameworkElement)_window.Content;

            if (element is not null)
            {
                var defaultTheme = _themeService.GetSavedDefaultApplicationTheme();

                if (theme is ElementTheme.Default)
                {
                    _themeService.Reset();
                    element.RequestedTheme = _themeService.Translate(defaultTheme);
                }
                else
                {
                    element.RequestedTheme = theme;
                }

                _themeService.UpdateTheme(_window, theme);
            }
        }
    }

    private void InitCurrentTheme()
    {
        var theme = _themeService.GetSavedElementTheme();

        if (theme is ElementTheme.Light)
        {
            ApplicationThemeSelection.SelectedItem = LightThemeSelection;
        }
        else if (theme is ElementTheme.Dark)
        {
            ApplicationThemeSelection.SelectedItem = DarkThemeSelection;
        }
        else
        {
            ApplicationThemeSelection.SelectedItem = DefaultThemeSelection;
        }
    }

}
