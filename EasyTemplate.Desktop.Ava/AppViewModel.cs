using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using EasyTemplate.Ava.Common;
using EasyTemplate.Ava.Tool.Util;

namespace EasyTemplate.Ava;

public partial class AppViewModel : ViewModelBase
{
    public AppViewModel()
    {
        ChangeLanguage(Setting.Config.Application.Language);
        ActionBase.ChangeLanguageAction += ChangeLanguage;
    }

    [RelayCommand]
    private void Popup()
    {
        ActionBase.WindowShowChanged?.Invoke(true);
    }

    [RelayCommand]
    private void Exit()
    {
        Environment.Exit(0);
    }

    private void ChangeLanguage(string lang)
    {
        AppName = Localization.Get("appname");
        ShowContent = Localization.Get("show");
        ExitContent = Localization.Get("exit");
        FunctionContent = Localization.Get("function");
        Func1Content = Localization.Get("func1");
        Func2Content = Localization.Get("func2");
        Func3Content = Localization.Get("func3");
    }

    [ObservableProperty] private string appName;
    [ObservableProperty] private string showContent;
    [ObservableProperty] private string exitContent;
    [ObservableProperty] private string functionContent;
    [ObservableProperty] private string func1Content;
    [ObservableProperty] private string func2Content;
    [ObservableProperty] private string func3Content;
}
