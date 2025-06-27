using Avalonia.Controls.Templates;
using EasyTemplate.Ava.Common;
using System;
using System.Collections.Generic;

namespace EasyTemplate.Ava;

public class ViewLocator : IDataTemplate
{
    private static Dictionary<string, Control> views = new Dictionary<string, Control>();
    public Control Build(object? data)
    {
        if (data is null)
        {
            return new TextBlock { Text = "data was null" };
        }
        
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            if (name.ToLower().Contains("dialog"))
            {
                return control;
            }
            else
            {
                if (views.ContainsKey(name))
                {
                    return views[name];
                }
                else
                {
                    views.Add(name, control);
                    return control;
                }
            }
        }
        else
        {
            return new TextBlock { Text = "Not Found: " + name };
        }
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
