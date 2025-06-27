using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Xml.Linq;
using AngleSharp.Dom;
using DnsClient.Protocol;
using Downloader;
using EasyTemplate.Ava.Tool.Entity;

namespace EasyTemplate.Ava.Tool.Util;

public class UpdateTask
{
    public static UpdateResult Result { get; set; } = UpdateResult.None;
    public static double Progress { get; set; } = 0;
    private static DownloadService downloadService { get; set; }
    public static UpdateInfo? UpdateInfo { get; set; }

    static UpdateTask()
    {
        var downloadOpt = new DownloadConfiguration()
        {
            ChunkCount = 8, // Number of file parts, default is 1
            ParallelDownload = true // Download parts in parallel (default is false)
        };
        downloadService = new DownloadService(downloadOpt);
        //downloadService.ChunkDownloadProgressChanged += (sender, args) => {
        //    Progress = args.ProgressPercentage;
        //};
        downloadService.DownloadProgressChanged += (sender, args) => {
            Progress = args.ProgressPercentage;
        };
        downloadService.DownloadFileCompleted += (sender, args) => {
            Progress = 100;
        };
    }

    /// <summary>
    /// 获取更新信息
    /// </summary>
    /// <param name="xmlUrl"></param>
    /// <returns></returns>
    public static async Task<UpdateInfo?> GetUpdateInfoAsync(string xmlUrl)
    {
        try
        {
            using var httpClient = new HttpClient();
            var xmlContent = await httpClient.GetStringAsync(xmlUrl);

            var doc = XDocument.Parse(xmlContent);
            var item = doc.Element("item");
            if (item == null) return null;

            UpdateInfo = new UpdateInfo
            {
                Version = item.Element("version")?.Value ?? "",
                Url = item.Element("url")?.Value ?? "",
                Beta = item.Element("beta")?.Value ?? "",
                Changelog = item.Element("changelog")?.Value ?? "",
                Mandatory = bool.TryParse(item.Element("mandatory")?.Value, out var m) && m
            };
            return UpdateInfo;
        }
        catch
        {
            return null;
        }
    }

    public static string GetDesktopVersion()
    {
        // 获取主程序路径
        var exePath = Path.Combine(Global.AppPath, "EasyTemplate.Ava.Desktop.exe");
        if (!File.Exists(exePath))
            return "0.0.0.0";

        var ver = FileVersionInfo.GetVersionInfo(exePath);
        return $"{ver.FileMajorPart}.{ver.FileMinorPart}.{ver.FileBuildPart}.{ver.FilePrivatePart}";
    }

    /// <summary>
    /// 检查更新
    /// </summary>
    /// <param name="isBeta">获取beta版，0：正式版，1：beta版</param>
    /// <returns></returns>
    public static async Task<bool> Check()
    {
        UpdateInfo = null;
        string version = GetDesktopVersion();
        var url = Setting.Config.Application.Beta ? Global.AppUpdateBeta : Global.AppUpdateUrl;
        await GetUpdateInfoAsync(url);
        if (UpdateInfo != null)
        {
            return new Version(UpdateInfo.Version) > new Version(version);
        }
        return false;
    }

    /// <summary>
    /// 清理更新临时目录
    /// </summary>
    private static void ClearUpdateTmp()
    {
        if (!Directory.Exists(Global.AppUpdatePath))
            Directory.CreateDirectory(Global.AppUpdatePath);

        // 删除所有文件
        foreach (var file in Directory.GetFiles(Global.AppUpdatePath, "*", SearchOption.AllDirectories))
        {
            File.Delete(file);
        }

        // 删除所有子目录
        foreach (var dir in Directory.GetDirectories(Global.AppUpdatePath, "*", SearchOption.AllDirectories))
        {
            Directory.Delete(dir, true);
        }
    }

    /// <summary>
    /// 下载更新包
    /// </summary>
    /// <returns></returns>
    public static async Task<bool> Download()
    {
        Progress = 0;
        Result = UpdateResult.None;
        try
        {
            ClearUpdateTmp();

            Result = UpdateResult.Downloading;

            await downloadService.DownloadFileTaskAsync(UpdateInfo.Url, $"{Global.AppUpdatePath}update.zip");
            //for (int i = 0; i < 100; i++)
            //{
            //    Progress += 1;
            //    await Task.Delay(200); // 模拟延迟
            //}

            Result = UpdateResult.Unzip;
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return false;
        }
    }

    /// <summary>
    /// 解压更新包
    /// </summary>
    /// <returns></returns>
    public static bool Unzip()
    {
        try
        {
            Result = UpdateResult.Unzip;

            string zipPath = Path.Combine(Global.AppUpdatePath, "update.zip");
            string extractPath = Global.AppUpdatePath;

            // 解压 update.zip 到指定目录
            if (File.Exists(zipPath))
            {
                // 解压
                ZipFile.ExtractToDirectory(zipPath, extractPath, overwriteFiles: true);
            }
            else
            {
                return false;
            }

            // 创建 success.txt
            string successFile = Path.Combine(Global.AppUpdatePath, "success.txt");
            File.WriteAllText(successFile, "success");

            // 删除 update.zip，确保文件未被占用
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (File.Exists(zipPath))
                    {
                        File.Delete(zipPath);
                    }
                    break;
                }
                catch (IOException)
                {
                    // 文件被占用，等待后重试
                    Task.Delay(200).Wait();
                }
            }

            Result = UpdateResult.Done;
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return false;
        }
    }

    /// <summary>
    /// 启动更新程序并退出当前进程
    /// </summary>
    public static void Patch()
    {
        try
        {
            // 启动 Updater.exe
            string updaterPath = Path.Combine(Global.AppPath, "Updater.exe");
            if (File.Exists(updaterPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterPath,
                    WorkingDirectory = Global.AppPath,
                    UseShellExecute = true
                });
            }

            // 退出当前进程
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}

public enum UpdateResult
{
    None,
    Downloading,
    Unzip,
    Done
}

public class UpdateInfo
{
    public string Version { get; set; } = "";
    public string Url { get; set; } = "";
    public string Beta { get; set; } = "";
    public string Changelog { get; set; } = "";
    public bool Mandatory { get; set; }
}
