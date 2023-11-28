namespace CapW.Areas.Settings;

public partial class SettingsPageViewModel : ObservableObject
{
    public SettingsPageViewModel()
    {
        var pkg = Windows.ApplicationModel.Package.Current;
        AppName = pkg.DisplayName;
        AppVersion = $"{pkg.Id.Version.Major}.{pkg.Id.Version.Minor}.{pkg.Id.Version.Build}";
        AppId = pkg.Id.FamilyName;
        Commit = TryGetCommit();
        RuntimeVersion = RuntimeInformation.FrameworkDescription;

        if (commit is null)
        {
            FullAppVersion = $"{pkg.Id.Version.Major}.{pkg.Id.Version.Minor}.{pkg.Id.Version.Build}.{pkg.Id.Version.Revision}";
        }
        else
        {
            FullAppVersion = $"{pkg.Id.Version.Major}.{pkg.Id.Version.Minor}.{pkg.Id.Version.Build}.{pkg.Id.Version.Revision}+{Commit}";
        }
    }

    [ObservableProperty]
    private string? appName;

    [ObservableProperty]
    private string? appVersion;

    [ObservableProperty]
    private string? fullAppVersion;

    [ObservableProperty]
    private string? appId;

    [ObservableProperty]
    private string? runtimeVersion;

    [ObservableProperty]
    private string? ownerDescription;

    [ObservableProperty]
    private string? commit;

    private static string? TryGetCommit()
    {
        var commit = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        if (string.IsNullOrWhiteSpace(commit) is false)
        {
            var indexOfPlus = commit.IndexOf('+');

            if (indexOfPlus > 0)
            {
                var assumedCommit = commit[(indexOfPlus + 1)..];

                if (assumedCommit.Length is 8)
                    return assumedCommit;

                if (assumedCommit.Length is 40)
                    return assumedCommit;
            }
        }

        return default;
    }
}
