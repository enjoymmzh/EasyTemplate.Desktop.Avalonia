using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Avalonia;
using EasyTemplate.Ava;
using EasyTemplate.Ava.Tool.Entity;
using Xilium.CefGlue.Common.Shared;
using Xilium.CefGlue.Common;
using Xilium.CefGlue;
using System.IO;

namespace EasyTemplate.Desktop.Ava.Desktop;

class Program
{
    private static string cachePath { get; set; }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        Global.AppVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString(4);
        cachePath = Path.Combine(Path.GetTempPath(), "CefGlue_" + Guid.NewGuid().ToString().Replace("-", null));
        AppDomain.CurrentDomain.ProcessExit += delegate { Cleanup(cachePath); };
        var builder = BuildAvaloniaApp();
        if(args.Contains("--drm"))
        {
            SilenceConsole();
                
            // If Card0, Card1 and Card2 all don't work. You can also try:                 
            // return builder.StartLinuxFbDev(args);
            // return builder.StartLinuxDrm(args, "/dev/dri/card1");
            return builder.StartLinuxDrm(args, "/dev/dri/card1", 1D);
        }

        return builder.StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .AfterSetup(_ => CefRuntimeLoader.Initialize(new CefSettings()
                {
                    RootCachePath = cachePath,
    #if WINDOWLESS
                              // its recommended to leave this off (false), since its less performant and can cause more issues
                              WindowlessRenderingEnabled = true
    #else
                    WindowlessRenderingEnabled = false
    #endif
                },
                customSchemes: new[] {
                  new CustomScheme()
                  {
                      SchemeName = "test",
                      SchemeHandlerFactory = new CustomSchemeHandler()
                  }
                }
            ));
    }

    private static void Cleanup(string cachePath)
    {
        CefRuntime.Shutdown(); // must shutdown cef to free cache files (so that cleanup is able to delete files)

        try
        {
            var dirInfo = new DirectoryInfo(cachePath);
            if (dirInfo.Exists)
            {
                dirInfo.Delete(true);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // ignore
        }
        catch (IOException)
        {
            // ignore
        }
    }

    private static void SilenceConsole()
    {
        new Thread(() =>
            {
                Console.CursorVisible = false;
                while(true)
                    Console.ReadKey(true);
            })
            { IsBackground = true }.Start();
    }
}

public class CustomSchemeHandler : CefSchemeHandlerFactory
{
    protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
    {
        throw new System.NotImplementedException();
    }
}