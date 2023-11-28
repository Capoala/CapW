namespace CapW.Navigation;

public interface INavigationService
{
    public event StrongTypedEventHandler<INavigationService, NavigationRequestedEventArgs>? NavigationRequested;
    public IReadOnlyList<NavigationItemViewModel> NavigationItems { get; }
    public IReadOnlyList<NavigationItemViewModel> FooterNavigationItems { get; }
    public NavigationItemViewModel Settings { get; }
    public bool TryNavigate(string name);
}

public sealed class NavigationService : INavigationService
{
    public NavigationService(
        IEnumerable<NavigationItemViewModel> navigationItems,
        IEnumerable<NavigationItemViewModel> footerNavigationItems,
        NavigationItemViewModel settings)
    {
        NavigationItems = new List<NavigationItemViewModel>(navigationItems).AsReadOnly();
        FooterNavigationItems = new List<NavigationItemViewModel>(footerNavigationItems).AsReadOnly();
        Settings = settings;
    }

    public event StrongTypedEventHandler<INavigationService, NavigationRequestedEventArgs>? NavigationRequested;

    public IReadOnlyList<NavigationItemViewModel> NavigationItems { get; }
    public IReadOnlyList<NavigationItemViewModel> FooterNavigationItems { get; }
    public NavigationItemViewModel Settings { get; }

    public bool TryNavigate(string name)
    {
        var didNavigate = TryNavigate(name, out var found);

        if (didNavigate)
            NavigationRequested?.Invoke(this, new NavigationRequestedEventArgs(found!));

        return didNavigate;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryNavigate(string name, [NotNullWhen(true)] out NavigationItemViewModel? found)
    {
        return TryFindNavigationViewModel(name, out found);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryFindNavigationViewModel(string name, [NotNullWhen(true)] out NavigationItemViewModel? found)
    {
        if (name.Equals(Settings.Name, StringComparison.Ordinal))
        {
            found = Settings;
            return true;
        }

        foreach (var vm in NavigationItems)
        {
            if (TryFindNavigationViewModel(vm, name, out found))
            {
                return true;
            }
        }

        foreach (var vm in FooterNavigationItems)
        {
            if (TryFindNavigationViewModel(vm, name, out found))
            {
                return true;
            }
        }

        found = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryFindNavigationViewModel(NavigationItemViewModel source, string name, [NotNullWhen(true)] out NavigationItemViewModel? found)
    {
        if (name.Equals(source.Name, StringComparison.Ordinal))
        {
            found = source;
            return true;
        }

        if (source.Children is not null)
        {
            foreach (var child in source.Children)
            {
                if (TryFindNavigationViewModel(child, name, out found))
                {
                    return true;
                }
            }
        }

        found = null;
        return false;
    }
}
