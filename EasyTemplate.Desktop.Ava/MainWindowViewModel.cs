using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using EasyTemplate.Ava.Common;
using EasyTemplate.Ava.Features;
using EasyTemplate.Ava.Tool.Entity;
using EasyTemplate.Ava.Tool.Util;
using Material.Icons.Avalonia;
using SukiUI.Enums;
using SukiUI.Models;
using SukiUI.Toasts;

namespace EasyTemplate.Ava;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        SukiTheme theme = SukiTheme.GetInstance();
        IsLight = Setting.Config.Application.Theme == "Light";
        theme.ChangeBaseTheme(Setting.Config.Application.Theme == "Light" ? ThemeVariant.Light : ThemeVariant.Dark);

        AvailableColors = theme.ColorThemes;
        var color = AvailableColors.Where(x => x.DisplayName == Setting.Config.Application.Color).FirstOrDefault();
        SukiTheme.GetInstance().ChangeColorTheme(color);

        IsChecked = Setting.Config.Application.CloseToTray;

        ChangeLanguage(Setting.Config.Application.Language);
        ActionBase.ChangeLanguageAction += ChangeLanguage;
    }

    ///// <summary>
    ///// 定时任务
    ///// </summary>
    ///// <returns></returns>
    //private void StartJobs()
    //{
    //    var registry = new Registry();

    //    //检查更新
    //    registry.Schedule(() => {
    //        var version = "3.8.8.8";
    //        var needupdate = true;
    //        if (needupdate)
    //        {
    //            AvaBase.ToastManager.CreateToast()
    //            .WithTitle(Localization.Get("upadteavailable"))
    //            .WithContent(string.Format(Localization.Get("updateinfo"), version))
    //            .WithActionButton(Localization.Get("later"), _ => { }, true, SukiUI.Enums.SukiButtonStyles.Basic)
    //            .WithActionButton(Localization.Get("update"), _ => ShowUpdatingToast(), true)
    //            .Queue();
    //        }
    //    }).ToRunNow().AndEvery(6).Hours();

    //    //清理内存
    //    registry.Schedule(() => {
    //        GC.Collect();
    //        GC.WaitForPendingFinalizers();
    //    }).ToRunNow().AndEvery(20).Seconds();

    //}

    [RelayCommand]
    private void ChangeLD()
    {
        ThemeName = IsLight ? Localization.Get("light") : Localization.Get("dark");
        SukiTheme.GetInstance().SwitchBaseTheme();

        Setting.Config.Application.Theme = IsLight ? "Light" : "Dark";
        Setting.Save();
    }

    [RelayCommand]
    private void ToYellow()
    {
        var color = AvailableColors.Where(x => x.DisplayName == "Orange").FirstOrDefault();
        SukiTheme.GetInstance().ChangeColorTheme(color);

        Setting.Config.Application.Color = "Orange";
        Setting.Save();
    }

    [RelayCommand]
    private void ToRed()
    {
        var color = AvailableColors.Where(x => x.DisplayName == "Red").FirstOrDefault();
        SukiTheme.GetInstance().ChangeColorTheme(color);

        Setting.Config.Application.Color = "Red";
        Setting.Save();
    }

    [RelayCommand]
    private void ToGreen()
    {
        var color = AvailableColors.Where(x => x.DisplayName == "Green").FirstOrDefault();
        SukiTheme.GetInstance().ChangeColorTheme(color);

        Setting.Config.Application.Color = "Green";
        Setting.Save();
    }

    [RelayCommand]
    private void ToBlue()
    {
        var color = AvailableColors.Where(x => x.DisplayName == "Blue").FirstOrDefault();
        SukiTheme.GetInstance().ChangeColorTheme(color);

        Setting.Config.Application.Color = "Blue";
        Setting.Save();
    }

    [RelayCommand]
    private void SwitchLanguage(string lang)
    {
        Setting.Config.Application.Language = lang;
        Setting.Save();

        Localization.CurrentLanguage = lang;
        ActionBase.ChangeLanguageAction?.Invoke(lang);
        var content = string.Format(Localization.Get("languagewillchange"), lang);
        AvaBase.ToastManager.CreateToast()
            .WithTitle(Localization.Get("success"))
            .WithContent(content)
            .OfType(NotificationType.Success)
            .Dismiss().After(TimeSpan.FromSeconds(2))
            .Dismiss().ByClicking()
            .Queue();
    }

    /// <summary>
    /// 打开反馈对话框
    /// </summary>
    [RelayCommand]
    private void Feedback()
    {
        AvaBase.DialogManager.CreateDialog()
            .WithViewModel(dialog => new FeedbackDialogViewModel(dialog))
            .Dismiss()
            .ByClickingBackground()
            .TryShow();
    }

    /// <summary>
    /// 打开关于对话框
    /// </summary>
    [RelayCommand]
    private void About()
    {
        AvaBase.DialogManager.CreateDialog()
            .WithViewModel(dialog => new AboutDialogViewModel(dialog))
            .Dismiss()
            .ByClickingBackground()
            .TryShow();
    }

    /// <summary>
    /// 检查
    /// </summary>
    [RelayCommand]
    private void Check()
    {
        Setting.Config.Application.CloseToTray = IsChecked;
        Setting.Save();
    }

    /// <summary>
    /// 
    /// </summary>
    [RelayCommand]
    private async Task CheckVersion()
    {
        if (await UpdateTask.Check())
        {
            var version = UpdateTask.GetDesktopVersion();
            AvaBase.ToastManager.CreateToast()
                .WithTitle(Localization.Get("upadteavailable"))
                .WithContent(string.Format(Localization.Get("updateinfo"), UpdateTask.UpdateInfo?.Version))
                .WithActionButton(Localization.Get("later"), _ => { }, true, SukiUI.Enums.SukiButtonStyles.Basic)
                .WithActionButton(Localization.Get("update"), _ => ShowUpdatingToast(), true)
                .Queue();
        }
        else
        {
            AvaBase.ToastManager.CreateToast()
                .WithTitle(Localization.Get("info"))
                .WithContent(Localization.Get("noupdate"))
                .OfType(NotificationType.Information)
                .Dismiss().After(TimeSpan.FromSeconds(2))
                .Dismiss().ByClicking()
                .Queue();
        }
    }

    private void ShowUpdatingToast()
    {
        UpdateTask.Download();
        var progress = new ProgressBar() { Value = 0, ShowProgressText = true };
        var dialog = AvaBase.ToastManager.CreateToast()
            .WithTitle(Localization.Get("updating"))
            .WithContent(progress)
            .Queue();
        Task.Run(() => {
            while (true)
            {
                if (UpdateTask.Result == UpdateResult.Downloading)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        progress.Value = UpdateTask.Progress;
                    });
                }
                else if (UpdateTask.Result == UpdateResult.Unzip)
                {
                    UpdateTask.Unzip();
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        AvaBase.ToastManager.Dismiss(dialog);
                        dialog = AvaBase.ToastManager.CreateToast()
                            .WithTitle(Localization.Get("updating"))
                            .WithContent(Localization.Get("unzip"))
                            .Queue();
                    });
                }
                else if (UpdateTask.Result == UpdateResult.Done)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        AvaBase.ToastManager.Dismiss(dialog);
                        AvaBase.ToastManager.CreateToast()
                            .WithTitle(Localization.Get("updating"))
                            .WithContent(Localization.Get("updatedone"))
                            .OfType(NotificationType.Success)
                            .Dismiss().ByClicking()
                            .WithActionButton(new MaterialIcon()
                            {
                                Kind = MaterialIconKind.Check
                            }, _ => { UpdateTask.Patch(); }, true, SukiButtonStyles.Flat | SukiButtonStyles.Accent | SukiButtonStyles.Icon)
                            .WithActionButton(new MaterialIcon()
                            {
                                Kind = MaterialIconKind.Close
                            }, _ => { }, true, SukiButtonStyles.Icon)
                            .Queue();
                    });
                    break;
                }
            }
        });

        //var progress = new ProgressBar() { Value = 0, ShowProgressText = true };
        //var toast = AvaBase.ToastManager.CreateToast()
        //    .WithTitle(Localization.Get("updating"))
        //    .WithContent(progress)
        //    .Queue();
        //var timer = new System.Timers.Timer(20);
        //timer.Elapsed += (_, _) =>
        //{
        //    Dispatcher.UIThread.Invoke(() =>
        //    {
        //        progress.Value += 1;
        //        if (progress.Value < 100) return;
        //        timer.Dispose();
        //        AvaBase.ToastManager.Dismiss(toast);
        //    });
        //};
        //timer.Start();
    }

    private void ChangeLanguage(string lang)
    {
        ThemeName = IsLight ? Localization.Get("light") : Localization.Get("dark");
        AppName = Localization.Get("appname");
        ProxyName = Localization.Get("proxy");
        ThemeColor = Localization.Get("color");
        HelpContent = Localization.Get("help");
        LanguageContent = Localization.Get("language");
        CheckUpdateContent = Localization.Get("update");
        FeedbackContent = Localization.Get("feedback");
        AboutContent = Localization.Get("about");
        CloseToTrayContent = Localization.Get("closetotray");
        TrayContent = Localization.Get("tray");
    }

    public IAvaloniaReadOnlyList<SukiColorTheme> AvailableColors { get; }
    [ObservableProperty] private bool isLight = false;
    [ObservableProperty] private bool isProxy = false;
    [ObservableProperty] private string themeName;
    [ObservableProperty] private bool isAnimateBack = false;
    [ObservableProperty] private bool isChecked = true;
    [ObservableProperty] private string appName;
    [ObservableProperty] private string proxyName;
    [ObservableProperty] private string themeColor;
    [ObservableProperty] private string helpContent;
    [ObservableProperty] private string languageContent;
    [ObservableProperty] private string checkUpdateContent;
    [ObservableProperty] private string feedbackContent;
    [ObservableProperty] private string aboutContent;
    [ObservableProperty] private string closeToTrayContent;
    [ObservableProperty] private string trayContent;
}
