using System.Text.Json;
using EasyTemplate.Ava.Tool.Entity;
using Microsoft.Extensions.Configuration;

namespace EasyTemplate.Ava.Tool.Util;

public class Setting
{
    /// <summary>
    /// 配置对象
    /// </summary>
    public static Config Config { get; set; } = new Config();

    /// <summary>
    /// 初始化配置
    /// </summary>
    /// <returns></returns>
    public static bool AddConfiguration()
    {
        var path = $"{Global.AppPath}Configuration.json";
        var json = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        if (string.IsNullOrEmpty(json))
        {
            Log.Info("Configuration.json文件不存在或内容为空，使用默认配置");
            return false;
        }
        Config = json.ToEntity<Config>();
        return true;
    }

    /// <summary>
    /// 保存配置到文件
    /// </summary>
    /// <param name="filePath"></param>
    public static void Save()
    {
        try
        {
            var path = $"{Global.AppPath}Configuration.json";
            File.WriteAllText(path, Config.ToJson());
        }
        catch (Exception ex)
        {
            Log.Error($"保存配置失败: {ex.Message}");
        }
    }

}
