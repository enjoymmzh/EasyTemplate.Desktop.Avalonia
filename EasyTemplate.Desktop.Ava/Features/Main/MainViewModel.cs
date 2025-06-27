using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.Input;
using EasyTemplate.Ava.Common;
using EasyTemplate.Ava.Tool.Entity;
using EasyTemplate.Ava.Tool.Util;
using SukiUI.Dialogs;

namespace EasyTemplate.Ava.Features;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel() 
    {
        ActionBase.SignOutAction = show =>
        {
            Global.CurrentUser = null;
            Global.IsSigninIn = false;
            UserName = Localization.Get("signin");
            ImageFromWebsite = LoadFromResource(new Uri(Global.DefaultAvatar));
        };

        ChangeLanguage(Setting.Config.Application.Language);
        ActionBase.ChangeLanguageAction += ChangeLanguage;
    }

    [RelayCommand]
    private void SignIn()
    {
        if (!Global.IsSigninIn)
        {
            AvaBase.DialogManager.CreateDialog()
                .WithViewModel(dialog => new SignInDialogViewModel(dialog))
                .Dismiss()
                .ByClickingBackground()
                .OnDismissed(dialog =>
                {
                    if (Global.CurrentUser is not null)
                    {
                        UserName = Global.CurrentUser.Name;
                        if (!string.IsNullOrWhiteSpace(Global.CurrentUser.Avatar))
                        {
                            ImageFromWebsite = LoadFromWeb(new Uri(Global.CurrentUser.Avatar));
                        }
                    }
                })
                .TryShow();
        }
        else
        {
            AvaBase.DialogManager.CreateDialog()
                .WithViewModel(dialog => new SignOutDialogViewModel(dialog))
                .Dismiss()
                .ByClickingBackground()
                .TryShow();
        }
    }

    public static async Task<Bitmap?> LoadFromResource(Uri resourceUri)
    {
        return new Bitmap(AssetLoader.Open(resourceUri));
    }

    public static async Task<Bitmap?> LoadFromWeb(Uri url)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return new Bitmap(new MemoryStream(data));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
            return null;
        }
    }

    private void ChangeLanguage(string lang)
    {
        SearchContent = Localization.Get("search");
        HomePageContent = Localization.Get("home");
        ManagementContent = Localization.Get("management");
        DatamanagementContent = Localization.Get("datamanagement");
        LogContent = Localization.Get("log");
        SettingContent = Localization.Get("setting");
        BrowseContent = Localization.Get("browse");
        AppVersion = $"{Localization.Get("appname")} v{Global.AppVersion}";
        UserName = Localization.Get("signin");
    }

    [RelayCommand]
    private static void OpenUrl(string url) => Extension.OpenUrl(url);

    [ObservableProperty] public string userName;
    [ObservableProperty] public string appVersion;
    [ObservableProperty] private Task<Bitmap?> imageFromWebsite = LoadFromResource(new Uri(Global.DefaultAvatar));
    [ObservableProperty] private string searchContent;
    [ObservableProperty] private string homePageContent;
    [ObservableProperty] private string managementContent;
    [ObservableProperty] private string datamanagementContent;
    [ObservableProperty] private string logContent;
    [ObservableProperty] private string settingContent;
    [ObservableProperty] private string browseContent;

}
