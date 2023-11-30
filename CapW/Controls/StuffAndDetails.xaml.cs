using System.Windows.Input;

namespace CapW.Controls;

public sealed partial class StuffAndDetails : UserControl
{
    public StuffAndDetails()
    {
        this.InitializeComponent();

        DeleteSelectedItemCommand = new RelayCommand(DeleteItem, CanDeleteItem);
    }

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(object),
        typeof(StuffAndDetails),
        new PropertyMetadata(null));

    public object? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(object),
        typeof(StuffAndDetails),
        new PropertyMetadata(null));

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(StuffAndDetails),
        new PropertyMetadata(null));

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty DetailsTemplateProperty = DependencyProperty.Register(
        nameof(DetailsTemplate),
        typeof(DataTemplate),
        typeof(StuffAndDetails),
        new PropertyMetadata(null));

    public DataTemplate? DetailsTemplate
    {
        get => (DataTemplate)GetValue(DetailsTemplateProperty);
        set => SetValue(DetailsTemplateProperty, value);
    }

    public static readonly DependencyProperty DeleteSelectedItemCommandProperty = DependencyProperty.Register(
        nameof(DeleteSelectedItemCommand),
        typeof(ICommand),
        typeof(StuffAndDetails),
        new PropertyMetadata(null));

    public ICommand? DeleteSelectedItemCommand
    {
        get => (ICommand)GetValue(DeleteSelectedItemCommandProperty);
        set => SetValue(DeleteSelectedItemCommandProperty, value);
    }

    public static readonly DependencyProperty AddNewItemCommandProperty = DependencyProperty.Register(
        nameof(AddNewItemCommand),
        typeof(ICommand),
        typeof(StuffAndDetails),
        new PropertyMetadata(null));

    public ICommand? AddNewItemCommand
    {
        get => (ICommand)GetValue(AddNewItemCommandProperty);
        set => SetValue(AddNewItemCommandProperty, value);
    }

    private void DeleteItem()
    {
        if (ItemsSource is IList itemsSource && SelectedItem is not null)
        {
            itemsSource.Remove(SelectedItem);

            if (itemsSource.Count > 0)
            {
                SelectedItem = itemsSource[0];
            }
            else
            {
                SelectedItem = null;
            }
        }
    }

    private bool CanDeleteItem() => SelectedItem is not null;
}
