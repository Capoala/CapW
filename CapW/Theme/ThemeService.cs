// This is still based on Template Studio, which you can find from the WinUI 3 Gallery app.
// https://github.com/microsoft/TemplateStudio/blob/main/code/TemplateStudioForWinUICs/Templates/Proj/Default/Param_ProjectName/Helpers/TitleBarHelper.cs
// Why? Because we can't seem to find them anywhere else to reference.

namespace CapW.Theme;

public sealed partial class ThemeService
{
    public ThemeService(IStorageService storage)
    {
        _storage = storage;
        SaveDefaultApplicationTheme();
    }

    public const string DefaultThemeKey = "defaultTheme";
    public const string ApplicationThemeKey = "appTheme";
    public const string ElementThemeKey = "elementTheme";

    private readonly IStorageService _storage;

    public void Reset()
    {
        _storage.DeleteSimple(ApplicationThemeKey);
        _storage.DeleteSimple(ElementThemeKey);
    }

    public void SaveDefaultApplicationTheme()
    {
        _storage.StoreSimple(DefaultThemeKey, (int)Application.Current.RequestedTheme);
    }

    public ApplicationTheme GetSavedDefaultApplicationTheme()
    {
        var theme = _storage.RetrieveNumber<int>(DefaultThemeKey);

        if (theme.Equals((int)ApplicationTheme.Dark))
            return ApplicationTheme.Dark;

        return ApplicationTheme.Light;
    }

    public void SaveApplicationTheme(ApplicationTheme theme)
    {
        _storage.StoreSimple(ApplicationThemeKey, (int)theme);
    }

    public ApplicationTheme GetSavedApplicationTheme()
    {
        var theme = _storage.RetrieveNumber<int>(ApplicationThemeKey);

        if (theme.Equals((int)ApplicationTheme.Dark))
            return ApplicationTheme.Dark;

        return ApplicationTheme.Light;
    }

    public void SaveApplicationTheme(ElementTheme theme)
    {
        if (theme is ElementTheme.Dark)
        {
            SaveApplicationTheme(ApplicationTheme.Dark);
        }
        else if (theme is ElementTheme.Light)
        {
            SaveApplicationTheme(ApplicationTheme.Light);
        }

        // There shouldn't be anything to set here if we're "Default"
        // The app is already running under a "default" for the application theme,
        // so we're already aligned.
    }

    public void SaveElementTheme(ElementTheme theme)
    {
        _storage.StoreSimple(ElementThemeKey, (int)theme);
    }

    public ElementTheme GetSavedElementTheme()
    {
        var theme = _storage.RetrieveNumber<int>(ElementThemeKey);

        if (theme.Equals((int)ElementTheme.Dark))
            return ElementTheme.Dark;

        if (theme.Equals((int)ElementTheme.Light))
            return ElementTheme.Light;

        return ElementTheme.Default;
    }


    public ApplicationTheme Translate(ElementTheme theme)
    {
        if (theme is ElementTheme.Dark)
            return ApplicationTheme.Dark;

        if (theme is ElementTheme.Light)
            return ApplicationTheme.Light;

        // Try to return what was saved, since default should translate to the system theme.
        return GetSavedDefaultApplicationTheme();
    }

    public ElementTheme Translate(ApplicationTheme theme)
    {
        if (theme is ApplicationTheme.Dark)
            return ElementTheme.Dark;

        if (theme is ApplicationTheme.Light)
            return ElementTheme.Light;

        return ElementTheme.Default;
    }


    public void UpdateTheme(Window window, ApplicationTheme theme)
    {
        if (theme is ApplicationTheme.Dark)
        {
            UpdateTheme(window, ElementTheme.Dark);
        }
        else
        {
            UpdateTheme(window, ElementTheme.Light);
        }
    }

    public void UpdateTheme(Window window, ElementTheme theme)
    {
        var savedTheme = GetSavedApplicationTheme();

        SaveElementTheme(theme);
        SaveApplicationTheme(theme);

        try
        {
            TitleBarHelper.UpdateTitleBar(window, theme, savedTheme);
        }
        catch { }
    }


    private static partial class TitleBarHelper
    {
        private const int WAINACTIVE = 0x00;
        private const int WAACTIVE = 0x01;
        private const int WMACTIVATE = 0x0006;

        [LibraryImport("user32.dll", SetLastError = true)]
        private static partial nint GetActiveWindow();

        [LibraryImport("user32.dll", SetLastError = true)]
        private static partial nint SendMessageA(nint hWnd, uint Msg, nint wParam, nint lParam);

        internal static void UpdateTitleBar(Window window, ElementTheme theme, ApplicationTheme systemAppTheme)
        {
            if (theme == ElementTheme.Default)
            {
                theme = systemAppTheme is ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
            }

            window.AppWindow.TitleBar.ButtonForegroundColor = theme switch
            {
                ElementTheme.Dark => Colors.White,
                ElementTheme.Light => Colors.Black,
                _ => Colors.Transparent
            };

            window.AppWindow.TitleBar.ButtonHoverForegroundColor = theme switch
            {
                ElementTheme.Dark => Colors.White,
                ElementTheme.Light => Colors.Black,
                _ => Colors.Transparent
            };

            window.AppWindow.TitleBar.ButtonHoverBackgroundColor = theme switch
            {
                ElementTheme.Dark => Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF),
                ElementTheme.Light => Color.FromArgb(0x33, 0x00, 0x00, 0x00),
                _ => Colors.Transparent
            };

            window.AppWindow.TitleBar.ButtonPressedBackgroundColor = theme switch
            {
                ElementTheme.Dark => Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF),
                ElementTheme.Light => Color.FromArgb(0x66, 0x00, 0x00, 0x00),
                _ => Colors.Transparent
            };

            window.AppWindow.TitleBar.BackgroundColor = Colors.Transparent;

            var activeWindow = GetActiveWindow();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            if (hwnd == activeWindow)
            {
                SendMessageA(hwnd, WMACTIVATE, WAINACTIVE, nint.Zero);
                SendMessageA(hwnd, WMACTIVATE, WAACTIVE, nint.Zero);
            }
            else
            {
                SendMessageA(hwnd, WMACTIVATE, WAACTIVE, nint.Zero);
                SendMessageA(hwnd, WMACTIVATE, WAINACTIVE, nint.Zero);
            }
        }
    }
}