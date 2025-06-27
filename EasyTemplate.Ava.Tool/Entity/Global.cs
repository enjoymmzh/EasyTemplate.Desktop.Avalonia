using System.Diagnostics;
using EasyTemplate.Ava.Tool.Util;

namespace EasyTemplate.Ava.Tool.Entity;

public class Global
{
    /// <summary>
    /// 应用名称
    /// </summary>
    public static string AppName { get { return Localization.Get("appname"); } }
    /// <summary>
    /// 应用版本
    /// </summary>
    public static string AppVersion { get; set; }
    /// <summary>
    /// API 基础 URL
    /// </summary>
    public const string BaseUrl = "https://api.example.com";
    /// <summary>
    /// 
    /// </summary>
    public static User CurrentUser { get; set; }
    /// <summary>
    /// 默认头像地址
    /// </summary>
    public static string DefaultAvatar { get { return "avares://EasyTemplate.Ava/Assets/default_avatar.png"; } }
    /// <summary>
    /// 
    /// </summary>
    public static bool IsSigninIn { get; set; }
    /// <summary>
    /// 应用程序路径 
    /// </summary>
    public static string AppPath
    {
        get
        {
            var path = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
            if (!path.EndsWith("/")) path += "/";
            return path;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static string AppUpdatePath { get { return $"{AppPath}UpdateTmp/"; } }
    /// <summary>
    /// 应用更新 XML 文件的 URL
    /// </summary>
    public static string AppUpdateUrl { get { return $"https://test.api.com/file/client/release/auto_updater_starter.xml"; } }
    /// <summary>
    /// 应用更新 Beta 版本 XML 文件的 URL
    /// </summary>
    public static string AppUpdateBeta { get { return $"https://test.api.com/file/client/beta/auto_updater_starter.xml"; } }
}
