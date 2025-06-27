using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTemplate.Ava.Common;

public class ActionBase
{
    public static Action<bool>? WindowShowChanged { get; set; }
    public static Action<bool>? SignOutAction { get; set; }
    public static Action<bool>? CheckUpdateAction { get; set; }
    public static Action<string>? ChangeLanguageAction { get; set; }
}
