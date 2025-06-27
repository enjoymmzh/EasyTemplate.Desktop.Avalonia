using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using EasyTemplate.Ava.Common;
using EasyTemplate.Ava.Tool.Entity;
using EasyTemplate.Ava.Tool.Util;
using SukiUI.Dialogs;

namespace EasyTemplate.Ava.Features;

public partial class LogViewModel : ViewModelBase
{
    public LogViewModel()
    {
        ChangeLanguage(Setting.Config.Application.Language);
        ActionBase.ChangeLanguageAction += ChangeLanguage;
    }

    private void ChangeLanguage(string lang)
    {
        LogError = Localization.Get("logerror");
    }

    [ObservableProperty] private string logError;
}
