namespace CapW.Navigation;

public sealed class NavigationRequestedEventArgs : EventArgs
{
    public NavigationRequestedEventArgs(NavigationItemViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ViewModel = viewModel;
    }

    public NavigationItemViewModel ViewModel { get; }
}
