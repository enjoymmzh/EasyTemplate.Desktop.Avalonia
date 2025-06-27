using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EasyTemplate.Ava.Features;

public partial class DataView : UserControl
{
    public DataView()
    {
        InitializeComponent();
        this.DataContext = new DataViewModel();
    }
}