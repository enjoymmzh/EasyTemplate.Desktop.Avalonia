using SukiUI.Toasts;

namespace EasyTemplate.Ava.Common;

public class AvaBase
{
    /// <summary>
    /// 
    /// </summary>
    public static ISukiDialogManager DialogManager { get; } = new SukiDialogManager();
    /// <summary>
    /// 
    /// </summary>
    public static ISukiToastManager ToastManager { get; } = new SukiToastManager();
}
