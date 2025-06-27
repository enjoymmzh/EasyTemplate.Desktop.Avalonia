using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using EasyTemplate.Ava.Common;
using SukiUI.Dialogs;
using SukiUI.Enums;
using SukiUI.Toasts;
using Avalonia.Threading;
using System;
using EasyTemplate.Ava.Tool.Util;

namespace EasyTemplate.Ava.Features;

public partial class AboutDialogViewModel : ViewModelBase
{
    public AboutDialogViewModel(ISukiDialog dialog)
    {
        this.dialog = dialog;

        ChangeLanguage(Setting.Config.Application.Language);
        ActionBase.ChangeLanguageAction += ChangeLanguage;
    }

    private void ChangeLanguage(string lang)
    {
        About = Localization.Get("about");
        AboutInfo = Localization.Get("aboutinfo");
    }

    private readonly ISukiDialog dialog;
    [ObservableProperty] private string about ;
    [ObservableProperty] private string aboutInfo ;
}
