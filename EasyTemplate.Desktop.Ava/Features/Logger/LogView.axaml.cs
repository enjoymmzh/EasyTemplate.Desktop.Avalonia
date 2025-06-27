using EasyTemplate.Ava.Features;

namespace EasyTemplate.Ava.Features;

public partial class LogView : UserControl
{
    public LogView()
    {
        InitializeComponent();
        this.DataContext = new LogViewModel();
    }
}