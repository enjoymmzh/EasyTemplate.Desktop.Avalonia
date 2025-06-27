using EasyTemplate.Ava.Features;

namespace EasyTemplate.Ava.Features;

public partial class SettingView : UserControl
{
    public SettingView()
    {
        InitializeComponent();
        this.DataContext = new SettingViewModel();
    }
}