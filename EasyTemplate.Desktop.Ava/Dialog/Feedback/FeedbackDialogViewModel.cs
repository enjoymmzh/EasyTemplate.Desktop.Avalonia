using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using EasyTemplate.Ava.Common;
using SukiUI.Dialogs;
using SukiUI.Enums;
using SukiUI.Toasts;
using Avalonia.Threading;
using System;
using System.Diagnostics;
using Avalonia.Controls.Notifications;
using EasyTemplate.Ava.Tool.Util;

namespace EasyTemplate.Ava.Features;

public partial class FeedbackDialogViewModel : ViewModelBase
{
    public FeedbackDialogViewModel(ISukiDialog dialog)
    {
        this.dialog = dialog;

        ChangeLanguage(Setting.Config.Application.Language);
        ActionBase.ChangeLanguageAction += ChangeLanguage;
    }

    [RelayCommand]
    private void Feedback()
    {
        var text = Info;
        if (string.IsNullOrWhiteSpace(text))
        {
            AvaBase.ToastManager.CreateToast()
                .WithTitle(Localization.Get("error"))
                .WithContent(Localization.Get("nocontent"))
                .OfType(NotificationType.Error)
                .Dismiss().After(TimeSpan.FromSeconds(1))
                .Dismiss().ByClicking()
                .Queue();
            return;
        }

        AvaBase.ToastManager.CreateToast()
            .WithTitle(Localization.Get("success"))
            .WithContent(Localization.Get("submitted"))
            .OfType(NotificationType.Success)
            .Dismiss().After(TimeSpan.FromSeconds(1))
            .Dismiss().ByClicking()
            .Queue();
    }

    private void ChangeLanguage(string lang)
    {
        Suggestion = Localization.Get("suggestion");
        Submit = Localization.Get("submit");
    }

    private readonly ISukiDialog dialog;
    [ObservableProperty] private string info;
    [ObservableProperty] private string suggestion;
    [ObservableProperty] private string submit;
}
