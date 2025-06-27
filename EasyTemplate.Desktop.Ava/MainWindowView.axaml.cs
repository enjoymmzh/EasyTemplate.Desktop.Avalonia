using EasyTemplate.Ava.Common;

namespace EasyTemplate.Ava;

public partial class MainWindowView : SukiWindow
{
    public MainWindowView()
    {
        InitializeComponent();
        DialogHost.Manager = AvaBase.DialogManager;
        ToastHost.Manager = AvaBase.ToastManager;

        ActionBase.WindowShowChanged += show =>
        {
            if (show)
            {
                WindowState = WindowState.Normal;
                this.Activate();
                this.Show(); // 或 BringToFront/Activate 等
            }
        };
    }
}
