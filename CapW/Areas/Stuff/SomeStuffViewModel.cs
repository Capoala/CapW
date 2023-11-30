namespace CapW.Areas.Stuff;

public partial class SomeStuffViewModel : ObservableObject
{
    public SomeStuffViewModel()
    {
        Somethings = [];

        for (int i = 0; i < 3; i++)
        {
            Somethings.Add(new()
            {
                Name = $"Something {i}",
                Username = $"something{i}"
            });
        }

        SelectedSomething = Somethings[0];
    }


    [ObservableProperty]
    private Something? selectedSomething;

    [ObservableProperty]
    private ObservableCollection<Something> somethings;

    [RelayCommand]
    private void AddNewSomething()
    {
        var theNewThing = new Something
        {
            Name = "New Something",
            Username = "newSomething"
        };

        Somethings.Add(theNewThing);

        SelectedSomething = theNewThing;
    }

    [RelayCommand]
    private void RemoveSelectedSomething()
    {
        if (SelectedSomething is not null)
        {
            //Somethings.Remove(SelectedSomething);
            System.Diagnostics.Debug.WriteLine($"Removing {SelectedSomething}");
        }
    }
}

public partial class Something : ObservableObject
{
    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? username;

    public override string ToString()
    {
        return $"{Name} has a username of {Username}";
    }
}