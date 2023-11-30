namespace CapW.Areas.Stuff;

public sealed partial class SomeStuffPage : UserControl
{
    public SomeStuffPage()
    {
        DataContext = Ioc.Default.GetRequiredService<SomeStuffViewModel>();
        this.InitializeComponent();
    }
}
