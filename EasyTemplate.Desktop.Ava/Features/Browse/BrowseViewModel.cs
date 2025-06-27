using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System;
using CommunityToolkit.Mvvm.Input;
using EasyTemplate.Ava.Common;
using Xilium.CefGlue;
using Xilium.CefGlue.Avalonia;
using Xilium.CefGlue.Common.Handlers;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using EasyTemplate.Ava.Tool.Entity;

namespace EasyTemplate.Ava.Features;

/// <summary>
/// 注意：browser上无法显示任何dialog和toast，因为这些事绘制在原始窗口的，而cef是一个独立的窗口控件，除非单独创建一个窗口，这样就有单独的窗口句柄在browser上层显示弹窗
/// 这不是avalonia独有的问题，•WPF、WinForms、Qt、Electron 等所有 UI 框架都存在同样的问题：原生控件和托管控件无法在 Z 轴上正确叠加
/// </summary>
public partial class BrowseViewModel : ViewModelBase
{
    private readonly AvaloniaCefBrowser cefbrowser;
    public BrowseViewModel(Decorator _browser)
    {
        cefbrowser = new AvaloniaCefBrowser();
        cefbrowser.Address = "https://www.bing.com";
        cefbrowser.RegisterJavascriptObject(new BindingTestClass(), "boundBeforeLoadObject");
        cefbrowser.TitleChanged += OnBrowserTitleChanged;
        cefbrowser.LifeSpanHandler = new BrowserLifeSpanHandler();
        _browser.Child = cefbrowser;

        IsCanGoBack = cefbrowser.CanGoBack;
        IsCanGoForward = cefbrowser.CanGoForward;

        cefbrowser.LoadStart += (sender, e) =>
        {
            IsVisible = true;
            IsCanGoBack = cefbrowser.CanGoBack;
            IsCanGoForward = cefbrowser.CanGoForward;
            if (e.Frame.Browser.IsPopup || !e.Frame.IsMain)
            {
                return;
            }

            IsVisible = true;
        };

        cefbrowser.LoadEnd += (sender, e) =>
        {
            IsVisible = false;
        };
    }

    [RelayCommand]
    private void Back()
    {
        if (cefbrowser.CanGoBack)
        {
            cefbrowser.GoBack();
        }
        IsCanGoBack = cefbrowser.CanGoBack;
        IsCanGoForward = cefbrowser.CanGoForward;
    }

    [RelayCommand]
    private void Forward()
    {
        if (cefbrowser.CanGoForward)
        {
            cefbrowser.GoForward();
        }

        IsCanGoBack = cefbrowser.CanGoBack;
        IsCanGoForward = cefbrowser.CanGoForward;
    }

    [RelayCommand]
    private void Home()
    {
        cefbrowser.Address = "https://www.bing.com"; // 设置为默认主页

        IsCanGoBack = cefbrowser.CanGoBack;
        IsCanGoForward = cefbrowser.CanGoForward;
    }

    [RelayCommand]
    private void Refresh()
    {
        cefbrowser.Reload();
    }

    [RelayCommand]
    private void Go()
    {
        cefbrowser.Address = Url;
    }

    [RelayCommand]
    private void Dev()
    {
        cefbrowser.ShowDeveloperTools();
    }

    static Task<object> AsyncCallNativeMethod(Func<object> nativeMethod)
    {
        return Task.Run(() =>
        {
            var result = nativeMethod.Invoke();
            if (result is Task task)
            {
                if (task.GetType().IsGenericType)
                {
                    return ((dynamic)task).Result;
                }

                return task;
            }

            return result;
        });
    }

    public event Action<string> TitleChanged;

    private void OnBrowserTitleChanged(object sender, string title)
    {
        TitleChanged?.Invoke(title);
    }

    private void OnAddressTextBoxKeyDown(object sender, global::Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            cefbrowser.Address = ((TextBox)sender).Text;
        }
    }

    public async void EvaluateJavascript()
    {
        var result = new StringWriter();

        result.Write(await cefbrowser.EvaluateJavaScript<string>("return \"Hello World!\""));

        result.Write("; " + await cefbrowser.EvaluateJavaScript<int>("return 1+1"));

        result.Write("; " + await cefbrowser.EvaluateJavaScript<bool>("return false"));

        result.Write("; " + await cefbrowser.EvaluateJavaScript<double>("return 1.5+1.5"));

        result.Write("; " + await cefbrowser.EvaluateJavaScript<double>("return 3+1.5"));

        result.Write("; " + await cefbrowser.EvaluateJavaScript<DateTime>("return new Date()"));

        result.Write("; " + string.Join(", ", await cefbrowser.EvaluateJavaScript<object[]>("return [1, 2, 3]")));

        result.Write("; " + string.Join(", ", (await cefbrowser.EvaluateJavaScript<ExpandoObject>("return (function() { return { a: 'valueA', b: 1, c: true } })()")).Select(p => p.Key + ":" + p.Value)));

        cefbrowser.ExecuteJavaScript($"alert(\"{result.ToString().Replace("\r\n", " | ").Replace("\"", "\\\"")}\")");
    }

    public void BindJavascriptObject()
    {
        const string TestObject = "dotNetObject";

        var obj = new BindingTestClass();
        cefbrowser.RegisterJavascriptObject(obj, TestObject, AsyncCallNativeMethod);

        var methods = obj.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                                   .Where(m => m.GetParameters().Length == 0)
                                   .Select(m => m.Name.Substring(0, 1).ToLowerInvariant() + m.Name.Substring(1));

        var script = "(function () {" +
            "let calls = [];" +
            string.Join("", methods.Select(m => $"calls.push({{ name: '{m}', promise: {TestObject}.{m}() }});")) +
            $"calls.push({{ name: 'asyncGetObjectWithParams', promise: {TestObject}.asyncGetObjectWithParams('a string') }});" +
            $"calls.push({{ name: 'getObjectWithParams', promise: {TestObject}.getObjectWithParams(5, 'a string', {{ Name: 'obj name', Value: 10 }}, [ 1, 2 ]) }});" +
            "calls.forEach(c => c.promise.then(r => console.log(c.name + ': ' + JSON.stringify(r))).catch(e => console.log(e)));" +
            "})()";

        cefbrowser.ExecuteJavaScript(script);
    }

    public void Dispose()
    {
        cefbrowser.Dispose();
    }

    [ObservableProperty] private bool isCanGoBack;
    [ObservableProperty] private bool isVisible = true;
    [ObservableProperty] private bool isCanGoForward;
    [ObservableProperty] private string url = "https://www.bing.com/"; // 默认网址

    private class BrowserLifeSpanHandler : LifeSpanHandler
    {
        protected override bool OnBeforePopup(
            CefBrowser browser,
            CefFrame frame,
            string targetUrl,
            string targetFrameName,
            CefWindowOpenDisposition targetDisposition,
            bool userGesture,
            CefPopupFeatures popupFeatures,
            CefWindowInfo windowInfo,
            ref CefClient client,
            CefBrowserSettings settings,
            ref CefDictionaryValue extraInfo,
            ref bool noJavascriptAccess)
        {
            browser.GetMainFrame().LoadUrl(targetUrl);
            //var bounds = windowInfo.Bounds;
            //Dispatcher.UIThread.Post(() =>
            //{
            //    var window = new Window();
            //    var popupBrowser = new AvaloniaCefBrowser();
            //    popupBrowser.Address = targetUrl;
            //    window.Content = popupBrowser;
            //    window.Position = new PixelPoint(bounds.X, bounds.Y);
            //    //window.Height = bounds.Height;
            //    //window.Width = bounds.Width;
            //    window.Title = targetUrl;
            //    window.Show();
            //});
            return true;
        }
    }
}

internal class BindingTestClass
{
    public class InnerObject
    {
        public string Name;
        public int Value;
    }

    public DateTime GetDate()
    {
        return DateTime.Now;
    }

    public string GetString()
    {
        return "Hello World!";
    }

    public int GetInt()
    {
        return 10;
    }

    public double GetDouble()
    {
        return 10.45;
    }

    public bool GetBool()
    {
        return true;
    }

    public string[] GetList()
    {
        return new[] { "item 1", "item 2", "item 3" };
    }

    public IDictionary<string, object> GetDictionary()
    {
        return new Dictionary<string, object>
            {
                { "Name", "This is a dictionary" },
                { "Value", 10.5 }
            };
    }

    public object GetObject()
    {
        return new InnerObject { Name = "This is an object", Value = 5 };
    }

    public object GetObjectWithParams(int anIntParam, string aStringParam, InnerObject anObjectParam, int[] intArrayParam)
    {
        return new InnerObject { Name = "This is an object", Value = 5 };
    }

    public async Task<bool> AsyncGetObjectWithParams(string aStringParam)
    {
        Console.WriteLine(DateTime.Now + ": Called " + nameof(AsyncGetObjectWithParams));
        await Task.Delay(5000).ConfigureAwait(false);
        Console.WriteLine(DateTime.Now + ":  Continuing " + nameof(AsyncGetObjectWithParams));
        return true;
    }

    public string[] GetObjectWithParamArray(int anIntParam, params string[] paramWithParamArray)
    {
        return paramWithParamArray;
    }
}