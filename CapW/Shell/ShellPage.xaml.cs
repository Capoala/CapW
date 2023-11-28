namespace CapW.Shell;

public sealed partial class ShellPage : Page, IDisposable
{
    public ShellPage(ShellViewModel viewModel, MainWindow window, INavigationService navigationService, IServiceProvider provider)
    {
        _viewModel = viewModel;
        _window = window;
        _navigationService = navigationService;
        _provider = provider;

        this.DataContext = _viewModel;
        this.InitializeComponent();

        window.ExtendsContentIntoTitleBar = true;
        window.SetTitleBar(AppTitleBar);
        window.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        window.AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
        window.SetTitleBar(AppTitleBar);
        this.LayoutUpdated += OnShellPageLayoutUpdated;
    }

    private readonly Window _window;
    private readonly ShellViewModel _viewModel;
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _provider;

    private void OnNavigationViewLoaded(object sender, RoutedEventArgs e)
    {
        // We could view model this, but since everything else is located here in the code behind,
        // we'll just keep it here for now.
        ShellNavigationView.MenuItemsSource = _navigationService.NavigationItems;
        ShellNavigationView.FooterMenuItemsSource = _navigationService.FooterNavigationItems;

        // The display mode undergoes a few changes during finalization for the initial rendering.
        // This ends up causing issues with our adjustments for the pane display mode.
        // Wait until the navigation view has settled before hooking up the events.
        AdjustViewForPaneDisplayMode();
        ShellNavigationView.DisplayModeChanged += OnDisplayModeChanged;
        ShellNavigationView.SelectionChanged += OnSelectionChanged;
        _navigationService.NavigationRequested += OnNavigationRequested;

        // Set the initial page if we have at least one page provided.
        if (_navigationService.NavigationItems.Count > 0)
        {
            ShellNavigationView.SelectedItem = _navigationService.NavigationItems[0];
        }
    }

    private void OnDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        AdjustViewForPaneDisplayMode();
    }

    private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var name = args.SelectedItemContainer?.Tag as string;

        if (name is not null)
        {
            _navigationService.TryNavigate(name);
        }
    }

    private void OnNavigationRequested(INavigationService sender, NavigationRequestedEventArgs e)
    {
        ShellNavigationView.Header = e.ViewModel.Name;
        ShellNavigationViewContentFrame.Content = _provider.GetRequiredService(e.ViewModel.PageType);
    }

    private void OnShellPageLayoutUpdated(object? sender, object e)
    {
        SetDragRegionForCustomTitleBar();
    }

    private void AdjustForTopDisplayMode()
    {
        ShellNavigationView.Margin = new Thickness(0, 48, 0, 0);
        ShellNavigationViewContentFrame.Margin = new Thickness(0, 0, 0, 0);

        if (ShellNavigationView.IsBackButtonVisible is NavigationViewBackButtonVisible.Visible)
        {
            PART_ButtonsHolderColumn.Width = new GridLength(48);
        }
        else
        {
            PART_ButtonsHolderColumn.Width = new GridLength(12); // Cheat for padding; it's just simpler.
        }
    }

    private void AdjustForLeftDisplayMode()
    {
        ShellNavigationView.Margin = new Thickness(0, 0, 0, 0);
        ShellNavigationViewContentFrame.Margin = new Thickness(0, 0, 0, 0);
        PART_ButtonsHolderColumn.Width = new GridLength(48);
    }

    private void AdjustForLeftCompactDisplayMode()
    {
        ShellNavigationView.Margin = new Thickness(0, 0, 0, 0);
        ShellNavigationViewContentFrame.Margin = new Thickness(0, 0, 0, 0);
        PART_ButtonsHolderColumn.Width = new GridLength(48);
    }

    private void AdjustForLeftMinimalDisplayMode()
    {
        ShellNavigationView.Margin = new Thickness(0, 0, 0, 0);
        ShellNavigationViewContentFrame.Margin = new Thickness(0, 0, 0, 0);

        if (ShellNavigationView.IsBackButtonVisible is NavigationViewBackButtonVisible.Visible)
        {
            PART_ButtonsHolderColumn.Width = new GridLength(48 * 2);
        }
        else
        {
            PART_ButtonsHolderColumn.Width = new GridLength(48);
        }
    }

    private void AdjustViewForPaneDisplayMode()
    {
        if (ShellNavigationView.PaneDisplayMode is NavigationViewPaneDisplayMode.Top)
        {
            AdjustForTopDisplayMode();
        }
        else if (ShellNavigationView.PaneDisplayMode is NavigationViewPaneDisplayMode.LeftMinimal)
        {
            AdjustForLeftMinimalDisplayMode();
        }
        else if (ShellNavigationView.PaneDisplayMode is NavigationViewPaneDisplayMode.LeftCompact)
        {
            AdjustForLeftCompactDisplayMode();
        }
        else if (ShellNavigationView.PaneDisplayMode is NavigationViewPaneDisplayMode.Left)
        {
            AdjustForLeftDisplayMode();
        }
        else if (ShellNavigationView.PaneDisplayMode is NavigationViewPaneDisplayMode.Auto)
        {
            if (ShellNavigationView.DisplayMode is NavigationViewDisplayMode.Compact)
            {
                AdjustForLeftCompactDisplayMode();
            }
            else if (ShellNavigationView.DisplayMode is NavigationViewDisplayMode.Minimal)
            {
                AdjustForLeftMinimalDisplayMode();
            }
            else if (ShellNavigationView.DisplayMode is NavigationViewDisplayMode.Expanded)
            {
                AdjustForLeftDisplayMode();
            }
        }

        SetDragRegionForCustomTitleBar();
    }

    private void SetDragRegionForCustomTitleBar()
    {
        if (_window is null)
            return;

        if (_window.AppWindow is null)
            return;

        if (_window.AppWindow.TitleBar is null)
            return;

        if (_window.AppWindow.TitleBar.ExtendsContentIntoTitleBar is false)
            return;

        // We have no idea why we need the extra width and height, but we do.
        // The actual controls in the XAML are correct, so the math here should also be correct
        // ... but the final result is still not right.

        // 24 works in most configurations. Start with it to simplify the changes.
        var extraWidth = 24;
        var extraHeight = 24;

        if (ShellNavigationView.PaneDisplayMode is NavigationViewPaneDisplayMode.Top)
        {
            extraWidth = 0;
        }
        else if (ShellNavigationView.PaneDisplayMode is NavigationViewPaneDisplayMode.LeftMinimal || ShellNavigationView.DisplayMode is NavigationViewDisplayMode.Minimal)
        {
            if (ShellNavigationView.IsBackButtonVisible is NavigationViewBackButtonVisible.Visible)
            {
                extraWidth = 32;
            }
        }

        if (ShellNavigationView.PaneDisplayMode is NavigationViewPaneDisplayMode.Top)
        {
            extraHeight = 0;
        }

        var calculatedWidth = int.CreateTruncating(PART_LeftPaddingColumn.ActualWidth) + int.CreateTruncating(PART_ButtonsHolderColumn.ActualWidth);
        var finalWidth = calculatedWidth + extraWidth;

        var calculatedHeight = int.CreateTruncating(AppTitleBar.ActualHeight);
        var finalHeight = calculatedHeight + extraHeight;

        var navigationViewArea = new RectInt32(
            _X: 0,
            _Y: 0,
            _Width: finalWidth,
            _Height: finalHeight
        );

        var client = InputNonClientPointerSource.GetForWindowId(_window.AppWindow.Id);
        client.SetRegionRects(NonClientRegionKind.Passthrough, [navigationViewArea]);
    }

    public void Dispose()
    {
        ShellNavigationView.DisplayModeChanged -= OnDisplayModeChanged;
        ShellNavigationView.SelectionChanged -= OnSelectionChanged;
        _navigationService.NavigationRequested -= OnNavigationRequested;
    }
}
