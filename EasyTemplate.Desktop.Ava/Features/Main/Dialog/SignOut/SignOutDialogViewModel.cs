using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using EasyTemplate.Ava.Common;
using EasyTemplate.Ava.Tool.Entity;
using EasyTemplate.Ava.Tool.Util;

namespace EasyTemplate.Ava.Features;

public partial class SignOutDialogViewModel : ViewModelBase
{
    public SignOutDialogViewModel(ISukiDialog dialog)
    {
        this.dialog = dialog;

        ChangeLanguage(Setting.Config.Application.Language);
        ActionBase.ChangeLanguageAction += ChangeLanguage;
    }

    [RelayCommand]
    private void SignOut()
    {
        ActionBase.SignOutAction?.Invoke(true);
        Dispatcher.UIThread.Invoke(() =>
        {
            dialog.Dismiss();
        });
    }

    private void ChangeLanguage(string lang)
    {
        SignOutContent = Localization.Get("signout");
    }

    private readonly ISukiDialog dialog;
    [ObservableProperty] private string signOutContent;
}
