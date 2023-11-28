namespace CapW.Areas.Home;

public sealed partial class HomePage : Page
{
    public HomePage(HomePageViewModel viewModel)
    {
        _viewModel = viewModel;
        this.DataContext = _viewModel;
        this.InitializeComponent();
    }

    private readonly HomePageViewModel _viewModel;
}
