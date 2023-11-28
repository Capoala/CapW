namespace CapW.Areas.Home;

public partial class HomePageViewModel : ObservableObject
{
    public HomePageViewModel()
    {
        myIdentification = $"If anyone's asking, my identification is {Guid.NewGuid()}";
    }

    [ObservableProperty]
    private string myIdentification;
}
