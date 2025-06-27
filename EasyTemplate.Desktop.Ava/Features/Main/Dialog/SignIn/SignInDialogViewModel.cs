using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using EasyTemplate.Ava.Common;
using SukiUI.Dialogs;
using SukiUI.Enums;
using SukiUI.Toasts;
using Avalonia.Threading;
using System;
using EasyTemplate.Ava.Tool.Util;
using EasyTemplate.Ava.Tool.Entity;

namespace EasyTemplate.Ava.Features;

public partial class SignInDialogViewModel : ViewModelBase
{
    public SignInDialogViewModel(ISukiDialog dialog)
    {
        this.dialog = dialog;

        ChangeLanguage(Setting.Config.Application.Language);
        ActionBase.ChangeLanguageAction += ChangeLanguage;
    }

    [RelayCommand]
    private Task Login()
    {
        //AvaBase.ToastManager.CreateToast()
        //.WithTitle("Update Available")
        //.WithContent("Information, Update v1.0.0.0 is Now Available.")
        //.WithActionButton("Later", _ => { }, true, SukiButtonStyles.Basic)
        //.WithActionButton("Update", _ => ShowUpdatingToast(), true)
        //.Queue();

        IsLoggingIn = true;
        return Task.Run(async () =>
        {
            await Task.Delay(3000);

            Global.CurrentUser = new User
            {
                Name = "Admin",
                Avatar = "https://c-ssl.dtstatic.com/uploads/blog/202207/09/20220709150824_97667.thumb.1000_0.jpg",
                Email = "123456@qq.com",
                IsAdmin = true,
            };

            Global.IsSigninIn = true;

            Setting.Config.Application.Remember = IsRemember;
            Setting.Save();

            IsLoggingIn = false;
            Dispatcher.UIThread.Invoke(() =>
            {
                dialog.Dismiss();
            });
        });
    }

    [RelayCommand]
    private void CloseDialog()
    {
        dialog.Dismiss();
    }

    private void ChangeLanguage(string lang)
    {
        LoginContent = Localization.Get("login");
        UsernameContent = Localization.Get("username");
        PasswordContent = Localization.Get("password");
        LogginginContent = Localization.Get("loggingin");
        LogininfoContent = Localization.Get("logininfo");
        RememberContent = Localization.Get("remember");
    }

    private readonly ISukiDialog dialog;
    [ObservableProperty] private bool _isLoggingIn;
    [ObservableProperty] private bool isRemember;
    [ObservableProperty] private string loginContent;
    [ObservableProperty] private string usernameContent;
    [ObservableProperty] private string passwordContent;
    [ObservableProperty] private string logginginContent;
    [ObservableProperty] private string logininfoContent;
    [ObservableProperty] private string rememberContent;

}
