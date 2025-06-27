namespace EasyTemplate.Ava.Features;

public partial class BrowseView : UserControl
{
    public BrowseView()
    {
        InitializeComponent();

        var browserWrapper = this.FindControl<Decorator>("browserWrapper");
        this.DataContext = new BrowseViewModel(browserWrapper);
    }
}

