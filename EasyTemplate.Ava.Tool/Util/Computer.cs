﻿using System.Runtime.InteropServices;

namespace EasyTemplate.Ava.Tool.Util;

public class Computer
{
    public static ComputerInfo GetComputerInfo()
    {
        ComputerInfo computerInfo = new ComputerInfo();
        try
        {
            MemoryMetricsClient client = new MemoryMetricsClient();
            MemoryMetrics memoryMetrics = client.GetMetrics();
            computerInfo.TotalRAM = Math.Ceiling(memoryMetrics.Total / 1024).ToString() + " GB";
            computerInfo.RAMRate = Math.Ceiling(100 * memoryMetrics.Used / memoryMetrics.Total).ToString();
            computerInfo.CPURate = Math.Ceiling(GetCPURate().ToDouble()).ToString();
            computerInfo.RunTime = GetRunTime();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        return computerInfo;
    }

    public static bool IsUnix()
    {
        var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        return isUnix;
    }

    public static string GetCPURate()
    {
        string cpuRate = string.Empty;
        if (IsUnix())
        {
            string output = ShellCmd.Bash("top -b -n1 | grep \"Cpu(s)\" | awk '{print $2 + $4}'");
            cpuRate = output.Trim();
        }
        else
        {
            string output = ShellCmd.Cmd("wmic", "cpu get LoadPercentage");
            cpuRate = output.Replace("LoadPercentage", string.Empty).Trim();
        }
        return cpuRate;
    }

    public static string GetRunTime()
    {
        string runTime = string.Empty;
        try
        {
            if (IsUnix())
            {
                string output = ShellCmd.Bash("uptime -s");
                output = output.Trim();
                runTime = Extension.FormatTime((DateTime.Now - output.ToDate()).TotalMilliseconds.ToString().Split('.')[0].ToLong());
            }
            else
            {
                string output = ShellCmd.Cmd("wmic", "OS get LastBootUpTime/Value");
                string[] outputArr = output.Split("=", StringSplitOptions.RemoveEmptyEntries);
                if (outputArr.Length == 2)
                {
                    runTime = Extension.FormatTime((DateTime.Now - outputArr[1].Split('.')[0].ToDate()).TotalMilliseconds.ToString().Split('.')[0].ToLong());
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        return runTime;
    }
}

public class MemoryMetricsClient
{
    public MemoryMetrics GetMetrics()
    {
        if (Computer.IsUnix())
        {
            return GetUnixMetrics();
        }
        return GetWindowsMetrics();
    }

    private MemoryMetrics GetWindowsMetrics()
    {
        string output = ShellCmd.Cmd("wmic", "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value");

        var lines = output.Trim().Split("\n");
        var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
        var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

        var metrics = new MemoryMetrics();
        metrics.Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0);
        metrics.Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0);
        metrics.Used = metrics.Total - metrics.Free;

        return metrics;
    }

    private MemoryMetrics GetUnixMetrics()
    {
        string output = ShellCmd.Bash("free -m");

        var lines = output.Split("\n");
        var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

        var metrics = new MemoryMetrics();
        metrics.Total = double.Parse(memory[1]);
        metrics.Used = double.Parse(memory[2]);
        metrics.Free = double.Parse(memory[3]);

        return metrics;
    }
}

public class MemoryMetrics
{
    public double Total { get; set; }
    public double Used { get; set; }
    public double Free { get; set; }
}

public class ComputerInfo
{
    /// <summary>
    /// CPU使用率
    /// </summary>
    public string CPURate { get; set; }

    /// <summary>
    /// 总内存
    /// </summary>
    public string TotalRAM { get; set; }

    /// <summary>
    /// 内存使用率
    /// </summary>
    public string RAMRate { get; set; }

    /// <summary>
    /// 系统运行时间
    /// </summary>
    public string RunTime { get; set; }
}