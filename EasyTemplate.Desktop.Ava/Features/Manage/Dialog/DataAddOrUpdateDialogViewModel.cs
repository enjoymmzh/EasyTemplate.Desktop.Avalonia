using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using EasyTemplate.Ava.Common;
using SukiUI.Toasts;
using Avalonia.Threading;
using EasyTemplate.Ava.Tool.Util;
using Avalonia.Controls.Notifications;
using System;

namespace EasyTemplate.Ava.Features;

public partial class DataAddOrUpdateDialogViewModel : ViewModelBase
{
    public DataAddOrUpdateDialogViewModel(ISukiDialog dialog, Invoice? invoce)
    {
        this.dialog = dialog;
        this.invoce = invoce;

        ChangeLanguage(Setting.Config.Application.Language);
        ActionBase.ChangeLanguageAction += ChangeLanguage;
    }

    [RelayCommand]
    private Task Submit()
    {
        IsLoggingIn = true;
        return Task.Run(async () =>
        {
            await Task.Delay(3000);
            IsLoggingIn = false;
            Dispatcher.UIThread.Invoke(() =>
            {
                AvaBase.ToastManager.CreateToast()
                    .WithTitle(Localization.Get("success"))
                    .WithContent($"{Info}{Localization.Get("success")}")
                    .OfType(NotificationType.Success)
                    .Dismiss().After(TimeSpan.FromSeconds(1))
                    .Dismiss().ByClicking()
                    .Queue();

                dialog.Dismiss();
            });
        });
    }

    [RelayCommand]
    private void Cancel()
    {
        dialog.Dismiss();
    }

    private void ChangeLanguage(string lang)
    {
        Info = invoce is null ? Localization.Get("add") : Localization.Get("edit");
        SubmitContent = Localization.Get("submit");
        CancelContent = Localization.Get("cancel");
        SubmittingContent = Localization.Get("submitting");
    }

    private readonly ISukiDialog dialog;
    private readonly Invoice? invoce;
    [ObservableProperty] private string info;
    [ObservableProperty] private bool _isLoggingIn;
    [ObservableProperty] private string submitContent;
    [ObservableProperty] private string cancelContent;
    [ObservableProperty] private string submittingContent;
}
