namespace EasyTemplate.Ava.Features;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        this.DataContext = new Features.DashboardViewModel();
    }
}