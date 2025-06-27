﻿using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace EasyTemplate.Ava.Tool.Util;

/// <summary>
/// 拓展方法写这里
/// 这是一个静态类
/// </summary>
public static class Extension
{
    /// <summary>
    /// 时间戳转时间
    /// </summary>
    /// <param name="datetime"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime ToDateTime(this long timestamp)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        var dateTime = dateTimeOffset.DateTime;
        return dateTime;
    }

    public static void OpenUrl(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Process.Start(new ProcessStartInfo(url.Replace("&", "^&")) { UseShellExecute = true });
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            Process.Start("xdg-open", url);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            Process.Start("open", url);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ToJson(this object data) => JsonConvert.SerializeObject(data, Formatting.Indented);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T? ToEntity<T>(this string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// 转简体
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToTraditional(this string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return CNConverter.S2T(text);
    }

    /// <summary>
    /// 转繁体
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToSimplified(this string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return CNConverter.T2S(text);
    }

    public static T Map<T>(this object source)
        => source.Map<T>();

    /// <summary>
    /// 扩展方法，获得枚举的Description
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <param name="nameInstead">当枚举值没有定义DescriptionAttribute，是否使用枚举名代替，默认是使用</param>
    /// <returns>枚举的Description</returns>
    public static string Description(this Enum value, Boolean nameInstead = true)
    {
        Type type = value.GetType();
        string name = Enum.GetName(type, value);
        if (name == null)
        {
            return null;
        }

        FieldInfo field = type.GetField(name);
        DescriptionAttribute attribute = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

        if (attribute == null && nameInstead == true)
        {
            return name;
        }
        return attribute?.Description;
    }

    /// <summary>
    /// 转换为双精度浮点数
    /// </summary>
    /// <param name="data">数据</param>
    public static double ToDouble(this object data)
    {
        if (data == null)
            return 0;
        double result;
        return double.TryParse(data.ToString(), out result) ? result : 0;
    }

    /// <summary>
    /// 转换为日期
    /// </summary>
    /// <param name="data">数据</param>
    public static DateTime ToDate(this object data)
    {
        if (data == null)
            return DateTime.MinValue;
        DateTime result;
        return DateTime.TryParse(data.ToString(), out result) ? result : DateTime.MinValue;
    }

    /// <summary>
    /// 将object转换为long，若转换失败，则返回0。不抛出异常。
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static long ToLong(this object obj)
    {
        try
        {
            return long.Parse(obj.ToString());
        }
        catch
        {
            return 0L;
        }
    }

    /// <summary>
    /// 毫秒转天时分秒
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    public static string FormatTime(long ms)
    {
        int ss = 1000;
        int mi = ss * 60;
        int hh = mi * 60;
        int dd = hh * 24;

        long day = ms / dd;
        long hour = (ms - day * dd) / hh;
        long minute = (ms - day * dd - hour * hh) / mi;
        long second = (ms - day * dd - hour * hh - minute * mi) / ss;
        long milliSecond = ms - day * dd - hour * hh - minute * mi - second * ss;

        string sDay = day < 10 ? "0" + day : "" + day; //天
        string sHour = hour < 10 ? "0" + hour : "" + hour;//小时
        string sMinute = minute < 10 ? "0" + minute : "" + minute;//分钟
        string sSecond = second < 10 ? "0" + second : "" + second;//秒
        string sMilliSecond = milliSecond < 10 ? "0" + milliSecond : "" + milliSecond;//毫秒
        sMilliSecond = milliSecond < 100 ? "0" + sMilliSecond : "" + sMilliSecond;

        return string.Format("{0} 天 {1} 小时 {2} 分 {3} 秒", sDay, sHour, sMinute, sSecond);
    }

    /// <summary>
    /// 将文本换行符转换为HTML换行标签（支持混合换行符）
    /// </summary>
    /// <param name="input">原始文本</param>
    /// <param name="isXhtml">是否生成XHTML闭合标签，默认true</param>
    /// <returns>带HTML换行的安全文本</returns>
    public static string ConvertNewlinesToHtml(this string input, bool isXhtml = true)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // 配置换行标签类型
        var brTag = isXhtml ? "<br />" : "<br>";

        // 三合一正则匹配：\r\n、\n、\r
        // 使用RegexOptions.Compiled提升性能
        return Regex.Replace(
            input,
            @"\r\n?|\n",
            brTag,
            RegexOptions.Compiled
        );
    }

    //
    // 摘要:
    //     将 DateTimeOffset 转换成本地 DateTime
    //
    // 参数:
    //   dateTime:
    public static DateTime ConvertToDateTime(this DateTimeOffset dateTime)
    {
        if (dateTime.Offset.Equals(TimeSpan.Zero))
        {
            return dateTime.UtcDateTime;
        }

        if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
        {
            return dateTime.ToLocalTime().DateTime;
        }

        return dateTime.DateTime;
    }

    //
    // 摘要:
    //     将 DateTimeOffset? 转换成本地 DateTime?
    //
    // 参数:
    //   dateTime:
    public static DateTime? ConvertToDateTime(this DateTimeOffset? dateTime)
    {
        if (!dateTime.HasValue)
        {
            return null;
        }

        return dateTime.Value.ConvertToDateTime();
    }

    //
    // 摘要:
    //     将 DateTime 转换成 DateTimeOffset
    //
    // 参数:
    //   dateTime:
    public static DateTimeOffset ConvertToDateTimeOffset(this DateTime dateTime)
    {
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
    }

    //
    // 摘要:
    //     将 DateTime? 转换成 DateTimeOffset?
    //
    // 参数:
    //   dateTime:
    public static DateTimeOffset? ConvertToDateTimeOffset(this DateTime? dateTime)
    {
        if (!dateTime.HasValue)
        {
            return null;
        }

        return dateTime.Value.ConvertToDateTimeOffset();
    }

    //
    // 摘要:
    //     将时间戳转换为 DateTime
    //
    // 参数:
    //   timestamp:
    internal static DateTime ConvertToDateTime(this long timestamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int num = (int)Math.Floor(Math.Log10(timestamp) + 1.0);
        if (num != 13 && num != 10)
        {
            throw new ArgumentException("Data is not a valid timestamp format.");
        }

        return ((num == 13) ? dateTime.AddMilliseconds(timestamp) : dateTime.AddSeconds(timestamp)).ToLocalTime();
    }

    //
    // 摘要:
    //     将流保存到本地磁盘
    //
    // 参数:
    //   stream:
    //
    //   path:
    public static void CopyToSave(this Stream stream, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException("path");
        }

        using FileStream destination = File.Create(path);
        stream.CopyTo(destination);
    }

    //
    // 摘要:
    //     将字节数组保存到本地磁盘
    //
    // 参数:
    //   bytes:
    //
    //   path:
    public static void CopyToSave(this byte[] bytes, string path)
    {
        using MemoryStream stream = new MemoryStream(bytes);
        stream.CopyToSave(path);
    }

    //
    // 摘要:
    //     将流保存到本地磁盘
    //
    // 参数:
    //   stream:
    //
    //   path:
    public static async Task CopyToSaveAsync(this Stream stream, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException("path");
        }

        using FileStream fileStream = File.Create(path);
        await stream.CopyToAsync(fileStream);
    }

    //
    // 摘要:
    //     将字节数组保存到本地磁盘
    //
    // 参数:
    //   bytes:
    //
    //   path:
    public static async Task CopyToSaveAsync(this byte[] bytes, string path)
    {
        using MemoryStream stream = new MemoryStream(bytes);
        await stream.CopyToSaveAsync(path);
    }

    //
    // 摘要:
    //     判断是否是富基元类型
    //
    // 参数:
    //   type:
    //     类型
    internal static bool IsRichPrimitive(this Type type)
    {
        if (type.IsValueTuple())
        {
            return false;
        }

        if (type.IsArray)
        {
            return type.GetElementType().IsRichPrimitive();
        }

        if (type.IsPrimitive || type.IsValueType || type == typeof(string))
        {
            return true;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return type.GenericTypeArguments[0].IsRichPrimitive();
        }

        return false;
    }

    //
    // 摘要:
    //     合并两个字典
    //
    // 参数:
    //   dic:
    //     字典
    //
    //   newDic:
    //     新字典
    //
    // 类型参数:
    //   T:
    internal static Dictionary<string, T> AddOrUpdate<T>(this Dictionary<string, T> dic, IDictionary<string, T> newDic)
    {
        foreach (string key in newDic.Keys)
        {
            if (dic.TryGetValue(key, out var value))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, newDic[key]);
            }
        }

        return dic;
    }

    //
    // 摘要:
    //     判断是否是元组类型
    //
    // 参数:
    //   type:
    //     类型
    internal static bool IsValueTuple(this Type type)
    {
        if (type.Namespace == "System")
        {
            return type.Name.Contains("ValueTuple`");
        }

        return false;
    }

    //
    // 摘要:
    //     判断方法是否是异步
    //
    // 参数:
    //   method:
    //     方法
    internal static bool IsAsync(this MethodInfo method)
    {
        if (method.GetCustomAttribute<AsyncMethodBuilderAttribute>() == null)
        {
            return method.ReturnType.ToString().StartsWith(typeof(Task).FullName);
        }

        return true;
    }

    //
    // 摘要:
    //     判断类型是否实现某个泛型
    //
    // 参数:
    //   type:
    //     类型
    //
    //   generic:
    //     泛型类型
    //
    // 返回结果:
    //     bool
    internal static bool HasImplementedRawGeneric(this Type type, Type generic)
    {
        if (type.GetInterfaces().Any(IsTheRawGenericType))
        {
            return true;
        }

        while (type != null && type != typeof(object))
        {
            if (IsTheRawGenericType(type))
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
        bool IsTheRawGenericType(Type type)
        {
            return generic == (type.IsGenericType ? type.GetGenericTypeDefinition() : type);
        }
    }

    //
    // 摘要:
    //     判断是否是匿名类型
    //
    // 参数:
    //   obj:
    //     对象
    internal static bool IsAnonymous(this object obj)
    {
        Type type2 = ((obj is Type type) ? type : obj.GetType());
        if (Attribute.IsDefined(type2, typeof(CompilerGeneratedAttribute), inherit: false) && type2.IsGenericType && type2.Name.Contains("AnonymousType") && (type2.Name.StartsWith("<>") || type2.Name.StartsWith("VB$")))
        {
            return type2.Attributes.HasFlag(TypeAttributes.AnsiClass);
        }

        return false;
    }

    //
    // 摘要:
    //     获取所有祖先类型
    //
    // 参数:
    //   type:
    internal static IEnumerable<Type> GetAncestorTypes(this Type type)
    {
        List<Type> list = new List<Type>();
        while (type != null && type != typeof(object) && IsNoObjectBaseType(type))
        {
            Type baseType = type.BaseType;
            list.Add(baseType);
            type = baseType;
        }

        return list;
        static bool IsNoObjectBaseType(Type type)
        {
            return type.BaseType != typeof(object);
        }
    }

    //
    // 摘要:
    //     获取方法真实返回类型
    //
    // 参数:
    //   method:
    internal static Type GetRealReturnType(this MethodInfo method)
    {
        bool num = method.IsAsync();
        Type returnType = method.ReturnType;
        if (!num)
        {
            return returnType;
        }

        return returnType.GenericTypeArguments.FirstOrDefault() ?? typeof(void);
    }

    //
    // 摘要:
    //     将一个对象转换为指定类型
    //
    // 参数:
    //   obj:
    //
    // 类型参数:
    //   T:
    internal static T ChangeType<T>(this object obj)
    {
        return (T)obj.ChangeType(typeof(T));
    }

    //
    // 摘要:
    //     将一个对象转换为指定类型
    //
    // 参数:
    //   obj:
    //     待转换的对象
    //
    //   type:
    //     目标类型
    //
    // 返回结果:
    //     转换后的对象
    internal static object ChangeType(this object obj, Type type)
    {
        if (type == null)
        {
            return obj;
        }

        if (type == typeof(string))
        {
            return obj?.ToString();
        }

        if (type == typeof(Guid) && obj != null)
        {
            return Guid.Parse(obj.ToString());
        }

        if (type == typeof(bool) && obj != null && !(obj is bool))
        {
            switch (obj.ToString().ToLower())
            {
                case "1":
                case "true":
                case "yes":
                case "on":
                    return true;
                default:
                    return false;
            }
        }

        if (obj == null)
        {
            if (!type.IsValueType)
            {
                return null;
            }

            return Activator.CreateInstance(type);
        }

        Type underlyingType = Nullable.GetUnderlyingType(type);
        if (type.IsAssignableFrom(obj.GetType()))
        {
            return obj;
        }

        if ((underlyingType ?? type).IsEnum)
        {
            if (underlyingType != null && string.IsNullOrWhiteSpace(obj.ToString()))
            {
                return null;
            }

            return Enum.Parse(underlyingType ?? type, obj.ToString());
        }

        if (obj.GetType().Equals(typeof(DateTime)) && (underlyingType ?? type).Equals(typeof(DateTimeOffset)))
        {
            return ((DateTime)obj).ConvertToDateTimeOffset();
        }

        if (obj.GetType().Equals(typeof(DateTimeOffset)) && (underlyingType ?? type).Equals(typeof(DateTime)))
        {
            return ((DateTimeOffset)obj).ConvertToDateTime();
        }

        if (typeof(IConvertible).IsAssignableFrom(underlyingType ?? type))
        {
            try
            {
                return Convert.ChangeType(obj, underlyingType ?? type, null);
            }
            catch
            {
                return (underlyingType == null) ? Activator.CreateInstance(type) : null;
            }
        }

        TypeConverter converter = TypeDescriptor.GetConverter(type);
        if (converter.CanConvertFrom(obj.GetType()))
        {
            return converter.ConvertFrom(obj);
        }

        ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
        if (constructor != null)
        {
            object obj3 = constructor.Invoke(null);
            PropertyInfo[] properties = type.GetProperties();
            Type type2 = obj.GetType();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                PropertyInfo property = type2.GetProperty(propertyInfo.Name);
                if (propertyInfo.CanWrite && property != null && property.CanRead)
                {
                    propertyInfo.SetValue(obj3, property.GetValue(obj, null).ChangeType(propertyInfo.PropertyType), null);
                }
            }

            return obj3;
        }

        return obj;
    }

    //
    // 摘要:
    //     查找方法指定特性，如果没找到则继续查找声明类
    //
    // 参数:
    //   method:
    //
    //   inherit:
    //
    // 类型参数:
    //   TAttribute:
    internal static TAttribute GetFoundAttribute<TAttribute>(this MethodInfo method, bool inherit) where TAttribute : Attribute
    {
        Type declaringType = method.DeclaringType;
        Type typeFromHandle = typeof(TAttribute);
        if (!method.IsDefined(typeFromHandle, inherit))
        {
            if (!declaringType.IsDefined(typeFromHandle, inherit))
            {
                return null;
            }

            return declaringType.GetCustomAttribute<TAttribute>(inherit);
        }

        return method.GetCustomAttribute<TAttribute>(inherit);
    }

    //
    // 摘要:
    //     格式化字符串
    //
    // 参数:
    //   str:
    //
    //   args:
    internal static string Format(this string str, params object[] args)
    {
        if (args != null && args.Length != 0)
        {
            return string.Format(str, args);
        }

        return str;
    }

    //
    // 摘要:
    //     切割骆驼命名式字符串
    //
    // 参数:
    //   str:
    internal static string[] SplitCamelCase(this string str)
    {
        if (str == null)
        {
            return Array.Empty<string>();
        }

        if (string.IsNullOrWhiteSpace(str))
        {
            return new string[1] { str };
        }

        if (str.Length == 1)
        {
            return new string[1] { str };
        }

        return (from u in Regex.Split(str, "(?=\\p{Lu}\\p{Ll})|(?<=\\p{Ll})(?=\\p{Lu})")
                where u.Length > 0
                select u).ToArray();
    }

    //
    // 摘要:
    //     JsonElement 转 Object
    //
    // 参数:
    //   jsonElement:
    internal static object ToObject(this JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.String:
                return jsonElement.GetString();
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
                return null;
            case JsonValueKind.Number:
                return jsonElement.GetDecimal();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return jsonElement.GetBoolean();
            case JsonValueKind.Object:
                {
                    JsonElement.ObjectEnumerator objectEnumerator = jsonElement.EnumerateObject();
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    {
                        foreach (JsonProperty item in objectEnumerator)
                        {
                            dictionary.Add(item.Name, item.Value.ToObject());
                        }

                        return dictionary;
                    }
                }
            case JsonValueKind.Array:
                {
                    JsonElement.ArrayEnumerator arrayEnumerator = jsonElement.EnumerateArray();
                    List<object> list = new List<object>();
                    {
                        foreach (JsonElement item2 in arrayEnumerator)
                        {
                            list.Add(item2.ToObject());
                        }

                        return list;
                    }
                }
            default:
                return null;
        }
    }

    //
    // 摘要:
    //     清除字符串前后缀
    //
    // 参数:
    //   str:
    //     字符串
    //
    //   pos:
    //     0：前后缀，1：后缀，-1：前缀
    //
    //   affixes:
    //     前后缀集合
    internal static string ClearStringAffixes(this string str, int pos = 0, params string[] affixes)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if (affixes == null || affixes.Length == 0)
        {
            return str;
        }

        bool flag = false;
        bool flag2 = false;
        string text = null;
        foreach (string text2 in affixes)
        {
            if (string.IsNullOrWhiteSpace(text2))
            {
                continue;
            }

            if (pos != 1 && !flag && str.StartsWith(text2, StringComparison.OrdinalIgnoreCase))
            {
                string text3 = str;
                int length = text2.Length;
                text = text3.Substring(length, text3.Length - length);
                flag = true;
            }

            if (pos != -1 && !flag2 && str.EndsWith(text2, StringComparison.OrdinalIgnoreCase))
            {
                string text3 = ((!string.IsNullOrWhiteSpace(text)) ? text : str);
                int length = text2.Length;
                text = text3.Substring(0, text3.Length - length);
                flag2 = true;
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = null;
                    flag2 = false;
                }
            }

            if (flag && flag2)
            {
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return str;
        }

        return text;
    }

    //
    // 摘要:
    //     判断集合是否为空
    //
    // 参数:
    //   collection:
    //     集合对象
    //
    // 类型参数:
    //   T:
    //     元素类型
    //
    // 返回结果:
    //     System.Boolean 实例，true 表示空集合，false 表示非空集合
    internal static bool IsEmpty<T>(this IEnumerable<T> collection)
    {
        if (collection != null)
        {
            return !collection.Any();
        }

        return true;
    }

    //
    // 摘要:
    //     获取类型自定义特性
    //
    // 参数:
    //   type:
    //     类类型
    //
    //   inherit:
    //     是否继承查找
    //
    // 类型参数:
    //   TAttribute:
    //     特性类型
    //
    // 返回结果:
    //     特性对象
    internal static TAttribute GetTypeAttribute<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        if (!type.IsDefined(typeof(TAttribute), inherit))
        {
            return null;
        }

        return type.GetCustomAttribute<TAttribute>(inherit);
    }

    #region 时间美化
    private const int Second = 1;
    private const int Minute = 60 * Second;
    private const int Hour = 60 * Minute;
    private const int Day = 24 * Hour;
    private const int Month = 30 * Day;

    // todo: Need to be localized
    public static string ToFriendlyDisplay(this DateTime dateTime)
    {
        var ts = DateTime.Now - dateTime;
        var delta = ts.TotalSeconds;
        if (delta < 0)
        {
            return "not yet";
        }
        if (delta < 1 * Minute)
        {
            return ts.Seconds == 1 ? "1 second ago" : ts.Seconds + " seconds ago";
        }
        if (delta < 2 * Minute)
        {
            return "1 minute ago";
        }
        if (delta < 45 * Minute)
        {
            return ts.Minutes + "minute";
        }
        if (delta < 90 * Minute)
        {
            return "1 hour ago";
        }
        if (delta < 24 * Hour)
        {
            return ts.Hours + " hours ago";
        }
        if (delta < 48 * Hour)
        {
            return "yesterday";
        }
        if (delta < 30 * Day)
        {
            return ts.Days + " days ago";
        }
        if (delta < 12 * Month)
        {
            var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
            return months <= 1 ? "A month ago" : months + " months ago";
        }
        else
        {
            var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "a year ago" : years + " years ago";
        }
    }
    #endregion
}

public class CNConverter
{
    private static Dictionary<string, string> jianDic = new Dictionary<string, string>();
    private static Dictionary<string, string> fanDic = new Dictionary<string, string>();
    static CNConverter()
    {
        #region 繁转简
        fanDic.Add("雲", "云");
        fanDic.Add("闆", "板");
        fanDic.Add("萬", "万");
        fanDic.Add("與", "与");
        fanDic.Add("專", "专");
        fanDic.Add("業", "业");
        fanDic.Add("叢", "丛");
        fanDic.Add("東", "东");
        fanDic.Add("絲", "丝");
        fanDic.Add("丟", "丢");
        fanDic.Add("兩", "两");
        fanDic.Add("嚴", "严");
        fanDic.Add("喪", "丧");
        fanDic.Add("個", "个");
        fanDic.Add("豐", "丰");
        fanDic.Add("臨", "临");
        fanDic.Add("為", "为");
        fanDic.Add("麗", "丽");
        fanDic.Add("舉", "举");
        fanDic.Add("義", "义");
        fanDic.Add("烏", "乌");
        fanDic.Add("樂", "乐");
        fanDic.Add("喬", "乔");
        fanDic.Add("習", "习");
        fanDic.Add("鄉", "乡");
        fanDic.Add("書", "书");
        fanDic.Add("買", "买");
        fanDic.Add("亂", "乱");
        fanDic.Add("爭", "争");
        fanDic.Add("虧", "亏");
        fanDic.Add("亙", "亘");
        fanDic.Add("亞", "亚");
        fanDic.Add("產", "产");
        fanDic.Add("畝", "亩");
        fanDic.Add("親", "亲");
        fanDic.Add("褻", "亵");
        fanDic.Add("億", "亿");
        fanDic.Add("僅", "仅");
        fanDic.Add("從", "从");
        fanDic.Add("侖", "仑");
        fanDic.Add("倉", "仓");
        fanDic.Add("儀", "仪");
        fanDic.Add("們", "们");
        fanDic.Add("價", "价");
        fanDic.Add("眾", "众");
        fanDic.Add("優", "优");
        fanDic.Add("會", "会");
        fanDic.Add("傴", "伛");
        fanDic.Add("傘", "伞");
        fanDic.Add("偉", "伟");
        fanDic.Add("傳", "传");
        fanDic.Add("傷", "伤");
        fanDic.Add("倀", "伥");
        fanDic.Add("倫", "伦");
        fanDic.Add("傖", "伧");
        fanDic.Add("偽", "伪");
        fanDic.Add("佇", "伫");
        fanDic.Add("體", "体");
        fanDic.Add("傭", "佣");
        fanDic.Add("僉", "佥");
        fanDic.Add("俠", "侠");
        fanDic.Add("侶", "侣");
        fanDic.Add("僥", "侥");
        fanDic.Add("偵", "侦");
        fanDic.Add("側", "侧");
        fanDic.Add("僑", "侨");
        fanDic.Add("儈", "侩");
        fanDic.Add("儕", "侪");
        fanDic.Add("儂", "侬");
        fanDic.Add("俁", "俣");
        fanDic.Add("儔", "俦");
        fanDic.Add("儼", "俨");
        fanDic.Add("倆", "俩");
        fanDic.Add("儷", "俪");
        fanDic.Add("儉", "俭");
        fanDic.Add("債", "债");
        fanDic.Add("傾", "倾");
        fanDic.Add("傯", "偬");
        fanDic.Add("僂", "偻");
        fanDic.Add("僨", "偾");
        fanDic.Add("償", "偿");
        fanDic.Add("儻", "傥");
        fanDic.Add("儐", "傧");
        fanDic.Add("儲", "储");
        fanDic.Add("儺", "傩");
        fanDic.Add("兒", "儿");
        fanDic.Add("兌", "兑");
        fanDic.Add("兗", "兖");
        fanDic.Add("黨", "党");
        fanDic.Add("蘭", "兰");
        fanDic.Add("關", "关");
        fanDic.Add("興", "兴");
        fanDic.Add("茲", "兹");
        fanDic.Add("養", "养");
        fanDic.Add("獸", "兽");
        fanDic.Add("囅", "冁");
        fanDic.Add("內", "内");
        fanDic.Add("岡", "冈");
        fanDic.Add("冊", "册");
        fanDic.Add("寫", "写");
        fanDic.Add("軍", "军");
        fanDic.Add("農", "农");
        fanDic.Add("馮", "冯");
        fanDic.Add("沖", "冲");
        fanDic.Add("決", "决");
        fanDic.Add("況", "况");
        fanDic.Add("凍", "冻");
        fanDic.Add("凈", "净");
        fanDic.Add("準", "准");
        fanDic.Add("涼", "凉");
        fanDic.Add("減", "减");
        fanDic.Add("湊", "凑");
        fanDic.Add("凜", "凛");
        fanDic.Add("幾", "几");
        fanDic.Add("鳳", "凤");
        fanDic.Add("鳧", "凫");
        fanDic.Add("憑", "凭");
        fanDic.Add("凱", "凯");
        fanDic.Add("兇", "凶");
        fanDic.Add("擊", "击");
        fanDic.Add("鑿", "凿");
        fanDic.Add("芻", "刍");
        fanDic.Add("劃", "划");
        fanDic.Add("劉", "刘");
        fanDic.Add("則", "则");
        fanDic.Add("剛", "刚");
        fanDic.Add("創", "创");
        fanDic.Add("刪", "删");
        fanDic.Add("別", "别");
        fanDic.Add("剄", "刭");
        fanDic.Add("剎", "刹");
        fanDic.Add("劊", "刽");
        fanDic.Add("劌", "刿");
        fanDic.Add("剴", "剀");
        fanDic.Add("劑", "剂");
        fanDic.Add("剮", "剐");
        fanDic.Add("劍", "剑");
        fanDic.Add("剝", "剥");
        fanDic.Add("劇", "剧");
        fanDic.Add("勸", "劝");
        fanDic.Add("辦", "办");
        fanDic.Add("務", "务");
        fanDic.Add("勱", "劢");
        fanDic.Add("動", "动");
        fanDic.Add("勵", "励");
        fanDic.Add("勁", "劲");
        fanDic.Add("勞", "劳");
        fanDic.Add("勢", "势");
        fanDic.Add("勛", "勋");
        fanDic.Add("勻", "匀");
        fanDic.Add("匭", "匦");
        fanDic.Add("匱", "匮");
        fanDic.Add("區", "区");
        fanDic.Add("醫", "医");
        fanDic.Add("華", "华");
        fanDic.Add("協", "协");
        fanDic.Add("單", "单");
        fanDic.Add("賣", "卖");
        fanDic.Add("盧", "卢");
        fanDic.Add("鹵", "卤");
        fanDic.Add("臥", "卧");
        fanDic.Add("衛", "卫");
        fanDic.Add("卻", "却");
        fanDic.Add("巹", "卺");
        fanDic.Add("廠", "厂");
        fanDic.Add("廳", "厅");
        fanDic.Add("歷", "历");
        fanDic.Add("厲", "厉");
        fanDic.Add("壓", "压");
        fanDic.Add("厭", "厌");
        fanDic.Add("厙", "厍");
        fanDic.Add("廁", "厕");
        fanDic.Add("廂", "厢");
        fanDic.Add("厴", "厣");
        fanDic.Add("廈", "厦");
        fanDic.Add("廚", "厨");
        fanDic.Add("廄", "厩");
        fanDic.Add("廝", "厮");
        fanDic.Add("縣", "县");
        fanDic.Add("參", "参");
        fanDic.Add("雙", "双");
        fanDic.Add("發", "发");
        fanDic.Add("變", "变");
        fanDic.Add("敘", "叙");
        fanDic.Add("疊", "叠");
        fanDic.Add("臺", "台");
        fanDic.Add("葉", "叶");
        fanDic.Add("號", "号");
        fanDic.Add("嘆", "叹");
        fanDic.Add("嘰", "叽");
        fanDic.Add("嚇", "吓");
        fanDic.Add("呂", "吕");
        fanDic.Add("嗎", "吗");
        fanDic.Add("噸", "吨");
        fanDic.Add("聽", "听");
        fanDic.Add("啟", "启");
        fanDic.Add("吳", "吴");
        fanDic.Add("吶", "呐");
        fanDic.Add("嘸", "呒");
        fanDic.Add("囈", "呓");
        fanDic.Add("嘔", "呕");
        fanDic.Add("嚦", "呖");
        fanDic.Add("唄", "呗");
        fanDic.Add("員", "员");
        fanDic.Add("咼", "呙");
        fanDic.Add("嗆", "呛");
        fanDic.Add("嗚", "呜");
        fanDic.Add("詠", "咏");
        fanDic.Add("嚨", "咙");
        fanDic.Add("嚀", "咛");
        fanDic.Add("響", "响");
        fanDic.Add("啞", "哑");
        fanDic.Add("噠", "哒");
        fanDic.Add("嘵", "哓");
        fanDic.Add("嗶", "哔");
        fanDic.Add("噦", "哕");
        fanDic.Add("嘩", "哗");
        fanDic.Add("噲", "哙");
        fanDic.Add("嚌", "哜");
        fanDic.Add("噥", "哝");
        fanDic.Add("喲", "哟");
        fanDic.Add("嘜", "唛");
        fanDic.Add("嘮", "唠");
        fanDic.Add("嗩", "唢");
        fanDic.Add("喚", "唤");
        fanDic.Add("嘖", "啧");
        fanDic.Add("嗇", "啬");
        fanDic.Add("囀", "啭");
        fanDic.Add("嚙", "啮");
        fanDic.Add("嘯", "啸");
        fanDic.Add("噴", "喷");
        fanDic.Add("嘍", "喽");
        fanDic.Add("嚳", "喾");
        fanDic.Add("囁", "嗫");
        fanDic.Add("噯", "嗳");
        fanDic.Add("噓", "嘘");
        fanDic.Add("嚶", "嘤");
        fanDic.Add("囑", "嘱");
        fanDic.Add("嚕", "噜");
        fanDic.Add("囂", "嚣");
        fanDic.Add("團", "团");
        fanDic.Add("園", "园");
        fanDic.Add("囪", "囱");
        fanDic.Add("圍", "围");
        fanDic.Add("圇", "囵");
        fanDic.Add("國", "国");
        fanDic.Add("圖", "图");
        fanDic.Add("圓", "圆");
        fanDic.Add("壙", "圹");
        fanDic.Add("場", "场");
        fanDic.Add("壞", "坏");
        fanDic.Add("塊", "块");
        fanDic.Add("堅", "坚");
        fanDic.Add("壇", "坛");
        fanDic.Add("壢", "坜");
        fanDic.Add("壩", "坝");
        fanDic.Add("塢", "坞");
        fanDic.Add("墳", "坟");
        fanDic.Add("墜", "坠");
        fanDic.Add("壟", "垄");
        fanDic.Add("壚", "垆");
        fanDic.Add("壘", "垒");
        fanDic.Add("墾", "垦");
        fanDic.Add("坰", "垧");
        fanDic.Add("堊", "垩");
        fanDic.Add("墊", "垫");
        fanDic.Add("埡", "垭");
        fanDic.Add("塏", "垲");
        fanDic.Add("塒", "埘");
        fanDic.Add("塤", "埙");
        fanDic.Add("堝", "埚");
        fanDic.Add("塹", "堑");
        fanDic.Add("墮", "堕");
        fanDic.Add("墑", "墒");
        fanDic.Add("墻", "墙");
        fanDic.Add("壯", "壮");
        fanDic.Add("聲", "声");
        fanDic.Add("殼", "壳");
        fanDic.Add("壺", "壶");
        fanDic.Add("處", "处");
        fanDic.Add("備", "备");
        fanDic.Add("復", "复");
        fanDic.Add("夠", "够");
        fanDic.Add("頭", "头");
        fanDic.Add("夾", "夹");
        fanDic.Add("奪", "夺");
        fanDic.Add("奩", "奁");
        fanDic.Add("奐", "奂");
        fanDic.Add("奮", "奋");
        fanDic.Add("獎", "奖");
        fanDic.Add("奧", "奥");
        fanDic.Add("妝", "妆");
        fanDic.Add("婦", "妇");
        fanDic.Add("媽", "妈");
        fanDic.Add("嫵", "妩");
        fanDic.Add("嫗", "妪");
        fanDic.Add("媯", "妫");
        fanDic.Add("姍", "姗");
        fanDic.Add("婁", "娄");
        fanDic.Add("婭", "娅");
        fanDic.Add("嬈", "娆");
        fanDic.Add("嬌", "娇");
        fanDic.Add("孌", "娈");
        fanDic.Add("娛", "娱");
        fanDic.Add("媧", "娲");
        fanDic.Add("嫻", "娴");
        fanDic.Add("嬰", "婴");
        fanDic.Add("嬋", "婵");
        fanDic.Add("嬸", "婶");
        fanDic.Add("媼", "媪");
        fanDic.Add("嬡", "嫒");
        fanDic.Add("嬪", "嫔");
        fanDic.Add("嬙", "嫱");
        fanDic.Add("嬤", "嬷");
        fanDic.Add("孫", "孙");
        fanDic.Add("學", "学");
        fanDic.Add("孿", "孪");
        fanDic.Add("寧", "宁");
        fanDic.Add("寶", "宝");
        fanDic.Add("實", "实");
        fanDic.Add("寵", "宠");
        fanDic.Add("審", "审");
        fanDic.Add("憲", "宪");
        fanDic.Add("宮", "宫");
        fanDic.Add("寬", "宽");
        fanDic.Add("賓", "宾");
        fanDic.Add("寢", "寝");
        fanDic.Add("對", "对");
        fanDic.Add("尋", "寻");
        fanDic.Add("導", "导");
        fanDic.Add("壽", "寿");
        fanDic.Add("將", "将");
        fanDic.Add("爾", "尔");
        fanDic.Add("塵", "尘");
        fanDic.Add("嘗", "尝");
        fanDic.Add("堯", "尧");
        fanDic.Add("尷", "尴");
        fanDic.Add("盡", "尽");
        fanDic.Add("層", "层");
        fanDic.Add("屜", "屉");
        fanDic.Add("屆", "届");
        fanDic.Add("屬", "属");
        fanDic.Add("屢", "屡");
        fanDic.Add("屨", "屦");
        fanDic.Add("嶼", "屿");
        fanDic.Add("歲", "岁");
        fanDic.Add("豈", "岂");
        fanDic.Add("嶇", "岖");
        fanDic.Add("崗", "岗");
        fanDic.Add("峴", "岘");
        fanDic.Add("嵐", "岚");
        fanDic.Add("島", "岛");
        fanDic.Add("巖", "岩");
        fanDic.Add("嶺", "岭");
        fanDic.Add("崠", "岽");
        fanDic.Add("巋", "岿");
        fanDic.Add("嶧", "峄");
        fanDic.Add("峽", "峡");
        fanDic.Add("嶠", "峤");
        fanDic.Add("崢", "峥");
        fanDic.Add("巒", "峦");
        fanDic.Add("嶗", "崂");
        fanDic.Add("崍", "崃");
        fanDic.Add("嶄", "崭");
        fanDic.Add("嶸", "嵘");
        fanDic.Add("崳", "嵛");
        fanDic.Add("嶁", "嵝");
        fanDic.Add("巔", "巅");
        fanDic.Add("鞏", "巩");
        fanDic.Add("巰", "巯");
        fanDic.Add("幣", "币");
        fanDic.Add("帥", "帅");
        fanDic.Add("師", "师");
        fanDic.Add("幃", "帏");
        fanDic.Add("帳", "帐");
        fanDic.Add("簾", "帘");
        fanDic.Add("幟", "帜");
        fanDic.Add("帶", "带");
        fanDic.Add("幀", "帧");
        fanDic.Add("幫", "帮");
        fanDic.Add("幬", "帱");
        fanDic.Add("幘", "帻");
        fanDic.Add("幗", "帼");
        fanDic.Add("冪", "幂");
        fanDic.Add("廣", "广");
        fanDic.Add("莊", "庄");
        fanDic.Add("慶", "庆");
        fanDic.Add("廬", "庐");
        fanDic.Add("廡", "庑");
        fanDic.Add("庫", "库");
        fanDic.Add("應", "应");
        fanDic.Add("廟", "庙");
        fanDic.Add("龐", "庞");
        fanDic.Add("廢", "废");
        fanDic.Add("廩", "廪");
        fanDic.Add("開", "开");
        fanDic.Add("異", "异");
        fanDic.Add("棄", "弃");
        fanDic.Add("弒", "弑");
        fanDic.Add("張", "张");
        fanDic.Add("彌", "弥");
        fanDic.Add("弳", "弪");
        fanDic.Add("彎", "弯");
        fanDic.Add("彈", "弹");
        fanDic.Add("強", "强");
        fanDic.Add("歸", "归");
        fanDic.Add("當", "当");
        fanDic.Add("錄", "录");
        fanDic.Add("彥", "彦");
        fanDic.Add("徹", "彻");
        fanDic.Add("徑", "径");
        fanDic.Add("徠", "徕");
        fanDic.Add("憶", "忆");
        fanDic.Add("懺", "忏");
        fanDic.Add("憂", "忧");
        fanDic.Add("愾", "忾");
        fanDic.Add("懷", "怀");
        fanDic.Add("態", "态");
        fanDic.Add("慫", "怂");
        fanDic.Add("憮", "怃");
        fanDic.Add("慪", "怄");
        fanDic.Add("悵", "怅");
        fanDic.Add("愴", "怆");
        fanDic.Add("憐", "怜");
        fanDic.Add("總", "总");
        fanDic.Add("懟", "怼");
        fanDic.Add("懌", "怿");
        fanDic.Add("戀", "恋");
        fanDic.Add("懇", "恳");
        fanDic.Add("惡", "恶");
        fanDic.Add("慟", "恸");
        fanDic.Add("懨", "恹");
        fanDic.Add("愷", "恺");
        fanDic.Add("惻", "恻");
        fanDic.Add("惱", "恼");
        fanDic.Add("惲", "恽");
        fanDic.Add("悅", "悦");
        fanDic.Add("愨", "悫");
        fanDic.Add("懸", "悬");
        fanDic.Add("慳", "悭");
        fanDic.Add("憫", "悯");
        fanDic.Add("驚", "惊");
        fanDic.Add("懼", "惧");
        fanDic.Add("慘", "惨");
        fanDic.Add("懲", "惩");
        fanDic.Add("憊", "惫");
        fanDic.Add("愜", "惬");
        fanDic.Add("慚", "惭");
        fanDic.Add("憚", "惮");
        fanDic.Add("慣", "惯");
        fanDic.Add("慍", "愠");
        fanDic.Add("憤", "愤");
        fanDic.Add("憒", "愦");
        fanDic.Add("懾", "慑");
        fanDic.Add("懣", "懑");
        fanDic.Add("懶", "懒");
        fanDic.Add("戇", "戆");
        fanDic.Add("戔", "戋");
        fanDic.Add("戲", "戏");
        fanDic.Add("戧", "戗");
        fanDic.Add("戰", "战");
        fanDic.Add("戩", "戬");
        fanDic.Add("戶", "户");
        fanDic.Add("撲", "扑");
        fanDic.Add("捍", "扞");
        fanDic.Add("執", "执");
        fanDic.Add("擴", "扩");
        fanDic.Add("捫", "扪");
        fanDic.Add("掃", "扫");
        fanDic.Add("揚", "扬");
        fanDic.Add("擾", "扰");
        fanDic.Add("撫", "抚");
        fanDic.Add("拋", "抛");
        fanDic.Add("摶", "抟");
        fanDic.Add("摳", "抠");
        fanDic.Add("掄", "抡");
        fanDic.Add("搶", "抢");
        fanDic.Add("護", "护");
        fanDic.Add("報", "报");
        fanDic.Add("擔", "担");
        fanDic.Add("擬", "拟");
        fanDic.Add("攏", "拢");
        fanDic.Add("揀", "拣");
        fanDic.Add("擁", "拥");
        fanDic.Add("攔", "拦");
        fanDic.Add("擰", "拧");
        fanDic.Add("撥", "拨");
        fanDic.Add("擇", "择");
        fanDic.Add("掛", "挂");
        fanDic.Add("摯", "挚");
        fanDic.Add("攣", "挛");
        fanDic.Add("撾", "挝");
        fanDic.Add("撻", "挞");
        fanDic.Add("挾", "挟");
        fanDic.Add("撓", "挠");
        fanDic.Add("擋", "挡");
        fanDic.Add("撟", "挢");
        fanDic.Add("掙", "挣");
        fanDic.Add("擠", "挤");
        fanDic.Add("揮", "挥");
        fanDic.Add("撈", "捞");
        fanDic.Add("損", "损");
        fanDic.Add("撿", "捡");
        fanDic.Add("換", "换");
        fanDic.Add("搗", "捣");
        fanDic.Add("據", "据");
        fanDic.Add("擄", "掳");
        fanDic.Add("摑", "掴");
        fanDic.Add("擲", "掷");
        fanDic.Add("撣", "掸");
        fanDic.Add("摻", "掺");
        fanDic.Add("摜", "掼");
        fanDic.Add("攬", "揽");
        fanDic.Add("撳", "揿");
        fanDic.Add("攙", "搀");
        fanDic.Add("擱", "搁");
        fanDic.Add("摟", "搂");
        fanDic.Add("攪", "搅");
        fanDic.Add("攜", "携");
        fanDic.Add("攝", "摄");
        fanDic.Add("攄", "摅");
        fanDic.Add("擺", "摆");
        fanDic.Add("搖", "摇");
        fanDic.Add("擯", "摈");
        fanDic.Add("攤", "摊");
        fanDic.Add("攖", "撄");
        fanDic.Add("撐", "撑");
        fanDic.Add("攆", "撵");
        fanDic.Add("擷", "撷");
        fanDic.Add("擼", "撸");
        fanDic.Add("攛", "撺");
        fanDic.Add("搟", "擀");
        fanDic.Add("擻", "擞");
        fanDic.Add("攢", "攒");
        fanDic.Add("敵", "敌");
        fanDic.Add("斂", "敛");
        fanDic.Add("數", "数");
        fanDic.Add("齋", "斋");
        fanDic.Add("斕", "斓");
        fanDic.Add("斬", "斩");
        fanDic.Add("斷", "断");
        fanDic.Add("無", "无");
        fanDic.Add("舊", "旧");
        fanDic.Add("時", "时");
        fanDic.Add("曠", "旷");
        fanDic.Add("曇", "昙");
        fanDic.Add("晝", "昼");
        fanDic.Add("顯", "显");
        fanDic.Add("晉", "晋");
        fanDic.Add("曬", "晒");
        fanDic.Add("曉", "晓");
        fanDic.Add("曄", "晔");
        fanDic.Add("暈", "晕");
        fanDic.Add("暉", "晖");
        fanDic.Add("暫", "暂");
        fanDic.Add("曖", "暧");
        fanDic.Add("術", "术");
        fanDic.Add("樸", "朴");
        fanDic.Add("機", "机");
        fanDic.Add("殺", "杀");
        fanDic.Add("雜", "杂");
        fanDic.Add("權", "权");
        fanDic.Add("桿", "杆");
        fanDic.Add("條", "条");
        fanDic.Add("來", "来");
        fanDic.Add("楊", "杨");
        fanDic.Add("榪", "杩");
        fanDic.Add("極", "极");
        fanDic.Add("構", "构");
        fanDic.Add("樅", "枞");
        fanDic.Add("樞", "枢");
        fanDic.Add("棗", "枣");
        fanDic.Add("櫪", "枥");
        fanDic.Add("棖", "枨");
        fanDic.Add("槍", "枪");
        fanDic.Add("楓", "枫");
        fanDic.Add("梟", "枭");
        fanDic.Add("檸", "柠");
        fanDic.Add("檉", "柽");
        fanDic.Add("梔", "栀");
        fanDic.Add("柵", "栅");
        fanDic.Add("標", "标");
        fanDic.Add("棧", "栈");
        fanDic.Add("櫛", "栉");
        fanDic.Add("櫳", "栊");
        fanDic.Add("棟", "栋");
        fanDic.Add("櫨", "栌");
        fanDic.Add("櫟", "栎");
        fanDic.Add("欄", "栏");
        fanDic.Add("樹", "树");
        fanDic.Add("棲", "栖");
        fanDic.Add("樣", "样");
        fanDic.Add("欒", "栾");
        fanDic.Add("椏", "桠");
        fanDic.Add("橈", "桡");
        fanDic.Add("楨", "桢");
        fanDic.Add("檔", "档");
        fanDic.Add("榿", "桤");
        fanDic.Add("橋", "桥");
        fanDic.Add("樺", "桦");
        fanDic.Add("檜", "桧");
        fanDic.Add("槳", "桨");
        fanDic.Add("樁", "桩");
        fanDic.Add("夢", "梦");
        fanDic.Add("檢", "检");
        fanDic.Add("欞", "棂");
        fanDic.Add("槨", "椁");
        fanDic.Add("櫝", "椟");
        fanDic.Add("槧", "椠");
        fanDic.Add("欏", "椤");
        fanDic.Add("橢", "椭");
        fanDic.Add("樓", "楼");
        fanDic.Add("欖", "榄");
        fanDic.Add("櫬", "榇");
        fanDic.Add("櫚", "榈");
        fanDic.Add("櫸", "榉");
        fanDic.Add("檻", "槛");
        fanDic.Add("檳", "槟");
        fanDic.Add("櫧", "槠");
        fanDic.Add("橫", "横");
        fanDic.Add("檣", "樯");
        fanDic.Add("櫻", "樱");
        fanDic.Add("櫥", "橱");
        fanDic.Add("櫓", "橹");
        fanDic.Add("櫞", "橼");
        fanDic.Add("檁", "檩");
        fanDic.Add("歡", "欢");
        fanDic.Add("歟", "欤");
        fanDic.Add("歐", "欧");
        fanDic.Add("殲", "歼");
        fanDic.Add("歿", "殁");
        fanDic.Add("殤", "殇");
        fanDic.Add("殘", "残");
        fanDic.Add("殞", "殒");
        fanDic.Add("殮", "殓");
        fanDic.Add("殫", "殚");
        fanDic.Add("殯", "殡");
        fanDic.Add("毆", "殴");
        fanDic.Add("毀", "毁");
        fanDic.Add("轂", "毂");
        fanDic.Add("畢", "毕");
        fanDic.Add("斃", "毙");
        fanDic.Add("氈", "毡");
        fanDic.Add("毿", "毵");
        fanDic.Add("氌", "氇");
        fanDic.Add("氣", "气");
        fanDic.Add("氫", "氢");
        fanDic.Add("氬", "氩");
        fanDic.Add("氳", "氲");
        fanDic.Add("匯", "汇");
        fanDic.Add("漢", "汉");
        fanDic.Add("湯", "汤");
        fanDic.Add("洶", "汹");
        fanDic.Add("溝", "沟");
        fanDic.Add("沒", "没");
        fanDic.Add("灃", "沣");
        fanDic.Add("漚", "沤");
        fanDic.Add("瀝", "沥");
        fanDic.Add("淪", "沦");
        fanDic.Add("滄", "沧");
        fanDic.Add("溈", "沩");
        fanDic.Add("滬", "沪");
        fanDic.Add("濘", "泞");
        fanDic.Add("淚", "泪");
        fanDic.Add("澩", "泶");
        fanDic.Add("瀧", "泷");
        fanDic.Add("瀘", "泸");
        fanDic.Add("濼", "泺");
        fanDic.Add("瀉", "泻");
        fanDic.Add("潑", "泼");
        fanDic.Add("澤", "泽");
        fanDic.Add("涇", "泾");
        fanDic.Add("潔", "洁");
        fanDic.Add("灑", "洒");
        fanDic.Add("浹", "浃");
        fanDic.Add("淺", "浅");
        fanDic.Add("漿", "浆");
        fanDic.Add("澆", "浇");
        fanDic.Add("湞", "浈");
        fanDic.Add("濁", "浊");
        fanDic.Add("測", "测");
        fanDic.Add("澮", "浍");
        fanDic.Add("濟", "济");
        fanDic.Add("瀏", "浏");
        fanDic.Add("渾", "浑");
        fanDic.Add("滸", "浒");
        fanDic.Add("濃", "浓");
        fanDic.Add("潯", "浔");
        fanDic.Add("濤", "涛");
        fanDic.Add("澇", "涝");
        fanDic.Add("淶", "涞");
        fanDic.Add("漣", "涟");
        fanDic.Add("潿", "涠");
        fanDic.Add("渦", "涡");
        fanDic.Add("渙", "涣");
        fanDic.Add("滌", "涤");
        fanDic.Add("潤", "润");
        fanDic.Add("澗", "涧");
        fanDic.Add("漲", "涨");
        fanDic.Add("澀", "涩");
        fanDic.Add("淵", "渊");
        fanDic.Add("淥", "渌");
        fanDic.Add("漬", "渍");
        fanDic.Add("瀆", "渎");
        fanDic.Add("漸", "渐");
        fanDic.Add("澠", "渑");
        fanDic.Add("漁", "渔");
        fanDic.Add("瀋", "渖");
        fanDic.Add("滲", "渗");
        fanDic.Add("溫", "温");
        fanDic.Add("灣", "湾");
        fanDic.Add("濕", "湿");
        fanDic.Add("潰", "溃");
        fanDic.Add("濺", "溅");
        fanDic.Add("潷", "滗");
        fanDic.Add("滾", "滚");
        fanDic.Add("滯", "滞");
        fanDic.Add("灄", "滠");
        fanDic.Add("滿", "满");
        fanDic.Add("瀅", "滢");
        fanDic.Add("濾", "滤");
        fanDic.Add("濫", "滥");
        fanDic.Add("灤", "滦");
        fanDic.Add("濱", "滨");
        fanDic.Add("灘", "滩");
        fanDic.Add("瀠", "潆");
        fanDic.Add("瀟", "潇");
        fanDic.Add("瀲", "潋");
        fanDic.Add("濰", "潍");
        fanDic.Add("潛", "潜");
        fanDic.Add("瀾", "澜");
        fanDic.Add("瀨", "濑");
        fanDic.Add("瀕", "濒");
        fanDic.Add("灝", "灏");
        fanDic.Add("滅", "灭");
        fanDic.Add("燈", "灯");
        fanDic.Add("靈", "灵");
        fanDic.Add("災", "灾");
        fanDic.Add("燦", "灿");
        fanDic.Add("煬", "炀");
        fanDic.Add("爐", "炉");
        fanDic.Add("燉", "炖");
        fanDic.Add("煒", "炜");
        fanDic.Add("熗", "炝");
        fanDic.Add("點", "点");
        fanDic.Add("煉", "炼");
        fanDic.Add("熾", "炽");
        fanDic.Add("爍", "烁");
        fanDic.Add("爛", "烂");
        fanDic.Add("烴", "烃");
        fanDic.Add("燭", "烛");
        fanDic.Add("煙", "烟");
        fanDic.Add("煩", "烦");
        fanDic.Add("燒", "烧");
        fanDic.Add("燁", "烨");
        fanDic.Add("燴", "烩");
        fanDic.Add("燙", "烫");
        fanDic.Add("燼", "烬");
        fanDic.Add("熱", "热");
        fanDic.Add("煥", "焕");
        fanDic.Add("燜", "焖");
        fanDic.Add("燾", "焘");
        fanDic.Add("愛", "爱");
        fanDic.Add("爺", "爷");
        fanDic.Add("牘", "牍");
        fanDic.Add("牽", "牵");
        fanDic.Add("犧", "牺");
        fanDic.Add("犢", "犊");
        fanDic.Add("狀", "状");
        fanDic.Add("獷", "犷");
        fanDic.Add("猶", "犹");
        fanDic.Add("狽", "狈");
        fanDic.Add("獰", "狞");
        fanDic.Add("獨", "独");
        fanDic.Add("狹", "狭");
        fanDic.Add("獅", "狮");
        fanDic.Add("獪", "狯");
        fanDic.Add("猙", "狰");
        fanDic.Add("獄", "狱");
        fanDic.Add("猻", "狲");
        fanDic.Add("貍", "狸");
        fanDic.Add("獫", "猃");
        fanDic.Add("獵", "猎");
        fanDic.Add("獼", "猕");
        fanDic.Add("玀", "猡");
        fanDic.Add("豬", "猪");
        fanDic.Add("貓", "猫");
        fanDic.Add("獻", "献");
        fanDic.Add("獺", "獭");
        fanDic.Add("璣", "玑");
        fanDic.Add("瑪", "玛");
        fanDic.Add("瑋", "玮");
        fanDic.Add("環", "环");
        fanDic.Add("現", "现");
        fanDic.Add("璽", "玺");
        fanDic.Add("玨", "珏");
        fanDic.Add("琺", "珐");
        fanDic.Add("瓏", "珑");
        fanDic.Add("琿", "珲");
        fanDic.Add("瑯", "琅");
        fanDic.Add("璉", "琏");
        fanDic.Add("瑣", "琐");
        fanDic.Add("瓊", "琼");
        fanDic.Add("瑤", "瑶");
        fanDic.Add("璦", "瑷");
        fanDic.Add("瓔", "璎");
        fanDic.Add("瓚", "瓒");
        fanDic.Add("甕", "瓮");
        fanDic.Add("甌", "瓯");
        fanDic.Add("電", "电");
        fanDic.Add("畫", "画");
        fanDic.Add("暢", "畅");
        fanDic.Add("畬", "畲");
        fanDic.Add("疇", "畴");
        fanDic.Add("癤", "疖");
        fanDic.Add("療", "疗");
        fanDic.Add("瘧", "疟");
        fanDic.Add("癘", "疠");
        fanDic.Add("瘍", "疡");
        fanDic.Add("瘡", "疮");
        fanDic.Add("瘋", "疯");
        fanDic.Add("皰", "疱");
        fanDic.Add("癥", "症");
        fanDic.Add("癰", "痈");
        fanDic.Add("痙", "痉");
        fanDic.Add("癢", "痒");
        fanDic.Add("癆", "痨");
        fanDic.Add("瘓", "痪");
        fanDic.Add("癇", "痫");
        fanDic.Add("癡", "痴");
        fanDic.Add("癉", "瘅");
        fanDic.Add("瘞", "瘗");
        fanDic.Add("瘺", "瘘");
        fanDic.Add("癟", "瘪");
        fanDic.Add("癱", "瘫");
        fanDic.Add("癮", "瘾");
        fanDic.Add("癭", "瘿");
        fanDic.Add("癩", "癞");
        fanDic.Add("癬", "癣");
        fanDic.Add("癲", "癫");
        fanDic.Add("皚", "皑");
        fanDic.Add("皺", "皱");
        fanDic.Add("皸", "皲");
        fanDic.Add("盞", "盏");
        fanDic.Add("鹽", "盐");
        fanDic.Add("監", "监");
        fanDic.Add("蓋", "盖");
        fanDic.Add("盜", "盗");
        fanDic.Add("盤", "盘");
        fanDic.Add("眥", "眦");
        fanDic.Add("瞇", "眯");
        fanDic.Add("著", "着");
        fanDic.Add("睜", "睁");
        fanDic.Add("脧", "睃");
        fanDic.Add("睞", "睐");
        fanDic.Add("瞼", "睑");
        fanDic.Add("睪", "睾");
        fanDic.Add("瞞", "瞒");
        fanDic.Add("矚", "瞩");
        fanDic.Add("矯", "矫");
        fanDic.Add("磯", "矶");
        fanDic.Add("礬", "矾");
        fanDic.Add("礦", "矿");
        fanDic.Add("碭", "砀");
        fanDic.Add("碼", "码");
        fanDic.Add("磚", "砖");
        fanDic.Add("硨", "砗");
        fanDic.Add("硯", "砚");
        fanDic.Add("礪", "砺");
        fanDic.Add("礱", "砻");
        fanDic.Add("礫", "砾");
        fanDic.Add("礎", "础");
        fanDic.Add("碩", "硕");
        fanDic.Add("硤", "硖");
        fanDic.Add("磽", "硗");
        fanDic.Add("確", "确");
        fanDic.Add("鹼", "硷");
        fanDic.Add("礙", "碍");
        fanDic.Add("磧", "碛");
        fanDic.Add("磣", "碜");
        fanDic.Add("堿", "碱");
        fanDic.Add("禮", "礼");
        fanDic.Add("禰", "祢");
        fanDic.Add("禎", "祯");
        fanDic.Add("禱", "祷");
        fanDic.Add("禍", "祸");
        fanDic.Add("稟", "禀");
        fanDic.Add("祿", "禄");
        fanDic.Add("禪", "禅");
        fanDic.Add("離", "离");
        fanDic.Add("禿", "秃");
        fanDic.Add("稈", "秆");
        fanDic.Add("種", "种");
        fanDic.Add("積", "积");
        fanDic.Add("稱", "称");
        fanDic.Add("穢", "秽");
        fanDic.Add("稅", "税");
        fanDic.Add("穌", "稣");
        fanDic.Add("穩", "稳");
        fanDic.Add("穡", "穑");
        fanDic.Add("窮", "穷");
        fanDic.Add("竊", "窃");
        fanDic.Add("竅", "窍");
        fanDic.Add("窯", "窑");
        fanDic.Add("竄", "窜");
        fanDic.Add("窩", "窝");
        fanDic.Add("窺", "窥");
        fanDic.Add("竇", "窦");
        fanDic.Add("窶", "窭");
        fanDic.Add("豎", "竖");
        fanDic.Add("競", "竞");
        fanDic.Add("篤", "笃");
        fanDic.Add("筍", "笋");
        fanDic.Add("筆", "笔");
        fanDic.Add("筧", "笕");
        fanDic.Add("箋", "笺");
        fanDic.Add("籠", "笼");
        fanDic.Add("籩", "笾");
        fanDic.Add("篳", "筚");
        fanDic.Add("篩", "筛");
        fanDic.Add("箏", "筝");
        fanDic.Add("籌", "筹");
        fanDic.Add("簽", "签");
        fanDic.Add("簡", "简");
        fanDic.Add("簀", "箦");
        fanDic.Add("篋", "箧");
        fanDic.Add("籜", "箨");
        fanDic.Add("籮", "箩");
        fanDic.Add("簞", "箪");
        fanDic.Add("簫", "箫");
        fanDic.Add("簣", "篑");
        fanDic.Add("簍", "篓");
        fanDic.Add("籃", "篮");
        fanDic.Add("籬", "篱");
        fanDic.Add("籪", "簖");
        fanDic.Add("籟", "籁");
        fanDic.Add("糴", "籴");
        fanDic.Add("類", "类");
        fanDic.Add("秈", "籼");
        fanDic.Add("糶", "粜");
        fanDic.Add("糲", "粝");
        fanDic.Add("粵", "粤");
        fanDic.Add("糞", "粪");
        fanDic.Add("糧", "粮");
        fanDic.Add("糝", "糁");
        fanDic.Add("緊", "紧");
        fanDic.Add("縶", "絷");
        fanDic.Add("糾", "纠");
        fanDic.Add("紆", "纡");
        fanDic.Add("紅", "红");
        fanDic.Add("紂", "纣");
        fanDic.Add("纖", "纤");
        fanDic.Add("紇", "纥");
        fanDic.Add("約", "约");
        fanDic.Add("級", "级");
        fanDic.Add("紈", "纨");
        fanDic.Add("纊", "纩");
        fanDic.Add("紀", "纪");
        fanDic.Add("紉", "纫");
        fanDic.Add("緯", "纬");
        fanDic.Add("紜", "纭");
        fanDic.Add("純", "纯");
        fanDic.Add("紕", "纰");
        fanDic.Add("紗", "纱");
        fanDic.Add("綱", "纲");
        fanDic.Add("納", "纳");
        fanDic.Add("縱", "纵");
        fanDic.Add("綸", "纶");
        fanDic.Add("紛", "纷");
        fanDic.Add("紙", "纸");
        fanDic.Add("紋", "纹");
        fanDic.Add("紡", "纺");
        fanDic.Add("紐", "纽");
        fanDic.Add("紓", "纾");
        fanDic.Add("線", "线");
        fanDic.Add("紺", "绀");
        fanDic.Add("紲", "绁");
        fanDic.Add("紱", "绂");
        fanDic.Add("練", "练");
        fanDic.Add("組", "组");
        fanDic.Add("紳", "绅");
        fanDic.Add("細", "细");
        fanDic.Add("織", "织");
        fanDic.Add("終", "终");
        fanDic.Add("縐", "绉");
        fanDic.Add("絆", "绊");
        fanDic.Add("紼", "绋");
        fanDic.Add("絀", "绌");
        fanDic.Add("紹", "绍");
        fanDic.Add("繹", "绎");
        fanDic.Add("經", "经");
        fanDic.Add("紿", "绐");
        fanDic.Add("綁", "绑");
        fanDic.Add("絨", "绒");
        fanDic.Add("結", "结");
        fanDic.Add("繞", "绕");
        fanDic.Add("絎", "绗");
        fanDic.Add("繪", "绘");
        fanDic.Add("給", "给");
        fanDic.Add("絢", "绚");
        fanDic.Add("絳", "绛");
        fanDic.Add("絡", "络");
        fanDic.Add("絕", "绝");
        fanDic.Add("絞", "绞");
        fanDic.Add("統", "统");
        fanDic.Add("綆", "绠");
        fanDic.Add("綃", "绡");
        fanDic.Add("絹", "绢");
        fanDic.Add("繡", "绣");
        fanDic.Add("綏", "绥");
        fanDic.Add("繼", "继");
        fanDic.Add("綈", "绨");
        fanDic.Add("績", "绩");
        fanDic.Add("緒", "绪");
        fanDic.Add("綾", "绫");
        fanDic.Add("續", "续");
        fanDic.Add("綺", "绮");
        fanDic.Add("緋", "绯");
        fanDic.Add("綽", "绰");
        fanDic.Add("緄", "绲");
        fanDic.Add("繩", "绳");
        fanDic.Add("維", "维");
        fanDic.Add("綿", "绵");
        fanDic.Add("綬", "绶");
        fanDic.Add("繃", "绷");
        fanDic.Add("綢", "绸");
        fanDic.Add("綹", "绺");
        fanDic.Add("綣", "绻");
        fanDic.Add("綜", "综");
        fanDic.Add("綻", "绽");
        fanDic.Add("綰", "绾");
        fanDic.Add("綠", "绿");
        fanDic.Add("綴", "缀");
        fanDic.Add("緇", "缁");
        fanDic.Add("緙", "缂");
        fanDic.Add("緗", "缃");
        fanDic.Add("緘", "缄");
        fanDic.Add("緬", "缅");
        fanDic.Add("纜", "缆");
        fanDic.Add("緹", "缇");
        fanDic.Add("緲", "缈");
        fanDic.Add("緝", "缉");
        fanDic.Add("繢", "缋");
        fanDic.Add("緦", "缌");
        fanDic.Add("綞", "缍");
        fanDic.Add("緞", "缎");
        fanDic.Add("緶", "缏");
        fanDic.Add("緱", "缑");
        fanDic.Add("縋", "缒");
        fanDic.Add("緩", "缓");
        fanDic.Add("締", "缔");
        fanDic.Add("縷", "缕");
        fanDic.Add("編", "编");
        fanDic.Add("緡", "缗");
        fanDic.Add("緣", "缘");
        fanDic.Add("縉", "缙");
        fanDic.Add("縛", "缚");
        fanDic.Add("縟", "缛");
        fanDic.Add("縝", "缜");
        fanDic.Add("縫", "缝");
        fanDic.Add("縞", "缟");
        fanDic.Add("纏", "缠");
        fanDic.Add("縭", "缡");
        fanDic.Add("縊", "缢");
        fanDic.Add("縑", "缣");
        fanDic.Add("繽", "缤");
        fanDic.Add("縹", "缥");
        fanDic.Add("縵", "缦");
        fanDic.Add("縲", "缧");
        fanDic.Add("纓", "缨");
        fanDic.Add("縮", "缩");
        fanDic.Add("繆", "缪");
        fanDic.Add("繅", "缫");
        fanDic.Add("纈", "缬");
        fanDic.Add("繚", "缭");
        fanDic.Add("繕", "缮");
        fanDic.Add("繒", "缯");
        fanDic.Add("韁", "缰");
        fanDic.Add("繾", "缱");
        fanDic.Add("繰", "缲");
        fanDic.Add("繯", "缳");
        fanDic.Add("繳", "缴");
        fanDic.Add("纘", "缵");
        fanDic.Add("罌", "罂");
        fanDic.Add("網", "网");
        fanDic.Add("羅", "罗");
        fanDic.Add("罰", "罚");
        fanDic.Add("罷", "罢");
        fanDic.Add("羆", "罴");
        fanDic.Add("羈", "羁");
        fanDic.Add("羥", "羟");
        fanDic.Add("羨", "羡");
        fanDic.Add("翹", "翘");
        fanDic.Add("耬", "耧");
        fanDic.Add("聳", "耸");
        fanDic.Add("恥", "耻");
        fanDic.Add("聶", "聂");
        fanDic.Add("聾", "聋");
        fanDic.Add("職", "职");
        fanDic.Add("聹", "聍");
        fanDic.Add("聯", "联");
        fanDic.Add("聵", "聩");
        fanDic.Add("聰", "聪");
        fanDic.Add("肅", "肃");
        fanDic.Add("腸", "肠");
        fanDic.Add("膚", "肤");
        fanDic.Add("骯", "肮");
        fanDic.Add("腎", "肾");
        fanDic.Add("腫", "肿");
        fanDic.Add("脹", "胀");
        fanDic.Add("脅", "胁");
        fanDic.Add("膽", "胆");
        fanDic.Add("勝", "胜");
        fanDic.Add("朧", "胧");
        fanDic.Add("臚", "胪");
        fanDic.Add("脛", "胫");
        fanDic.Add("膠", "胶");
        fanDic.Add("脈", "脉");
        fanDic.Add("膾", "脍");
        fanDic.Add("臟", "脏");
        fanDic.Add("臍", "脐");
        fanDic.Add("腦", "脑");
        fanDic.Add("膿", "脓");
        fanDic.Add("臠", "脔");
        fanDic.Add("腳", "脚");
        fanDic.Add("脫", "脱");
        fanDic.Add("腡", "脶");
        fanDic.Add("臉", "脸");
        fanDic.Add("臘", "腊");
        fanDic.Add("膩", "腻");
        fanDic.Add("靦", "腼");
        fanDic.Add("膃", "腽");
        fanDic.Add("騰", "腾");
        fanDic.Add("臏", "膑");
        fanDic.Add("輿", "舆");
        fanDic.Add("艤", "舣");
        fanDic.Add("艦", "舰");
        fanDic.Add("艙", "舱");
        fanDic.Add("艫", "舻");
        fanDic.Add("艱", "艰");
        fanDic.Add("艷", "艳");
        fanDic.Add("艸", "艹");
        fanDic.Add("藝", "艺");
        fanDic.Add("節", "节");
        fanDic.Add("羋", "芈");
        fanDic.Add("薌", "芗");
        fanDic.Add("蕪", "芜");
        fanDic.Add("蘆", "芦");
        fanDic.Add("蕓", "芸");
        fanDic.Add("蓯", "苁");
        fanDic.Add("芐", "苄");
        fanDic.Add("葦", "苇");
        fanDic.Add("藶", "苈");
        fanDic.Add("莧", "苋");
        fanDic.Add("萇", "苌");
        fanDic.Add("蒼", "苍");
        fanDic.Add("苧", "苎");
        fanDic.Add("蘇", "苏");
        fanDic.Add("茍", "苟");
        fanDic.Add("蘋", "苹");
        fanDic.Add("莖", "茎");
        fanDic.Add("蘢", "茏");
        fanDic.Add("蔦", "茑");
        fanDic.Add("塋", "茔");
        fanDic.Add("煢", "茕");
        fanDic.Add("繭", "茧");
        fanDic.Add("荊", "荆");
        fanDic.Add("薦", "荐");
        fanDic.Add("莢", "荚");
        fanDic.Add("蕘", "荛");
        fanDic.Add("蓽", "荜");
        fanDic.Add("蕎", "荞");
        fanDic.Add("薈", "荟");
        fanDic.Add("薺", "荠");
        fanDic.Add("蕩", "荡");
        fanDic.Add("榮", "荣");
        fanDic.Add("葷", "荤");
        fanDic.Add("滎", "荥");
        fanDic.Add("犖", "荦");
        fanDic.Add("熒", "荧");
        fanDic.Add("蕁", "荨");
        fanDic.Add("藎", "荩");
        fanDic.Add("蓀", "荪");
        fanDic.Add("蔭", "荫");
        fanDic.Add("葒", "荭");
        fanDic.Add("藥", "药");
        fanDic.Add("蒞", "莅");
        fanDic.Add("萊", "莱");
        fanDic.Add("蓮", "莲");
        fanDic.Add("蒔", "莳");
        fanDic.Add("萵", "莴");
        fanDic.Add("薟", "莶");
        fanDic.Add("獲", "获");
        fanDic.Add("蕕", "莸");
        fanDic.Add("瑩", "莹");
        fanDic.Add("鶯", "莺");
        fanDic.Add("蘿", "萝");
        fanDic.Add("螢", "萤");
        fanDic.Add("營", "营");
        fanDic.Add("縈", "萦");
        fanDic.Add("蕭", "萧");
        fanDic.Add("薩", "萨");
        fanDic.Add("蔥", "葱");
        fanDic.Add("蕆", "蒇");
        fanDic.Add("蕢", "蒉");
        fanDic.Add("蔣", "蒋");
        fanDic.Add("蔞", "蒌");
        fanDic.Add("藍", "蓝");
        fanDic.Add("薊", "蓟");
        fanDic.Add("蘺", "蓠");
        fanDic.Add("蕷", "蓣");
        fanDic.Add("鎣", "蓥");
        fanDic.Add("驀", "蓦");
        fanDic.Add("薔", "蔷");
        fanDic.Add("蘞", "蔹");
        fanDic.Add("藺", "蔺");
        fanDic.Add("藹", "蔼");
        fanDic.Add("蘄", "蕲");
        fanDic.Add("蘊", "蕴");
        fanDic.Add("藪", "薮");
        fanDic.Add("蘚", "藓");
        fanDic.Add("蘗", "蘖");
        fanDic.Add("虜", "虏");
        fanDic.Add("慮", "虑");
        fanDic.Add("虛", "虚");
        fanDic.Add("蟲", "虫");
        fanDic.Add("蟣", "虮");
        fanDic.Add("雖", "虽");
        fanDic.Add("蝦", "虾");
        fanDic.Add("蠆", "虿");
        fanDic.Add("蝕", "蚀");
        fanDic.Add("蟻", "蚁");
        fanDic.Add("螞", "蚂");
        fanDic.Add("蠶", "蚕");
        fanDic.Add("蠔", "蚝");
        fanDic.Add("蜆", "蚬");
        fanDic.Add("蠱", "蛊");
        fanDic.Add("蠣", "蛎");
        fanDic.Add("蟶", "蛏");
        fanDic.Add("蠻", "蛮");
        fanDic.Add("蟄", "蛰");
        fanDic.Add("蛺", "蛱");
        fanDic.Add("蟯", "蛲");
        fanDic.Add("螄", "蛳");
        fanDic.Add("蠐", "蛴");
        fanDic.Add("蛻", "蜕");
        fanDic.Add("蝸", "蜗");
        fanDic.Add("蠟", "蜡");
        fanDic.Add("蠅", "蝇");
        fanDic.Add("蟈", "蝈");
        fanDic.Add("蟬", "蝉");
        fanDic.Add("螻", "蝼");
        fanDic.Add("蠑", "蝾");
        fanDic.Add("釁", "衅");
        fanDic.Add("銜", "衔");
        fanDic.Add("補", "补");
        fanDic.Add("襯", "衬");
        fanDic.Add("袞", "衮");
        fanDic.Add("襖", "袄");
        fanDic.Add("裊", "袅");
        fanDic.Add("襪", "袜");
        fanDic.Add("襲", "袭");
        fanDic.Add("裝", "装");
        fanDic.Add("襠", "裆");
        fanDic.Add("褳", "裢");
        fanDic.Add("襝", "裣");
        fanDic.Add("褲", "裤");
        fanDic.Add("褸", "褛");
        fanDic.Add("襤", "褴");
        fanDic.Add("見", "见");
        fanDic.Add("觀", "观");
        fanDic.Add("規", "规");
        fanDic.Add("覓", "觅");
        fanDic.Add("視", "视");
        fanDic.Add("覘", "觇");
        fanDic.Add("覽", "览");
        fanDic.Add("覺", "觉");
        fanDic.Add("覬", "觊");
        fanDic.Add("覡", "觋");
        fanDic.Add("覿", "觌");
        fanDic.Add("覦", "觎");
        fanDic.Add("覯", "觏");
        fanDic.Add("覲", "觐");
        fanDic.Add("覷", "觑");
        fanDic.Add("觴", "觞");
        fanDic.Add("觸", "触");
        fanDic.Add("觶", "觯");
        fanDic.Add("譽", "誉");
        fanDic.Add("謄", "誊");
        fanDic.Add("計", "计");
        fanDic.Add("訂", "订");
        fanDic.Add("訃", "讣");
        fanDic.Add("認", "认");
        fanDic.Add("譏", "讥");
        fanDic.Add("訐", "讦");
        fanDic.Add("訌", "讧");
        fanDic.Add("討", "讨");
        fanDic.Add("讓", "让");
        fanDic.Add("訕", "讪");
        fanDic.Add("訖", "讫");
        fanDic.Add("訓", "训");
        fanDic.Add("議", "议");
        fanDic.Add("訊", "讯");
        fanDic.Add("記", "记");
        fanDic.Add("講", "讲");
        fanDic.Add("諱", "讳");
        fanDic.Add("謳", "讴");
        fanDic.Add("詎", "讵");
        fanDic.Add("訝", "讶");
        fanDic.Add("訥", "讷");
        fanDic.Add("許", "许");
        fanDic.Add("訛", "讹");
        fanDic.Add("論", "论");
        fanDic.Add("訟", "讼");
        fanDic.Add("諷", "讽");
        fanDic.Add("設", "设");
        fanDic.Add("訪", "访");
        fanDic.Add("訣", "诀");
        fanDic.Add("證", "证");
        fanDic.Add("詁", "诂");
        fanDic.Add("訶", "诃");
        fanDic.Add("評", "评");
        fanDic.Add("詛", "诅");
        fanDic.Add("識", "识");
        fanDic.Add("詐", "诈");
        fanDic.Add("訴", "诉");
        fanDic.Add("診", "诊");
        fanDic.Add("詆", "诋");
        fanDic.Add("謅", "诌");
        fanDic.Add("詞", "词");
        fanDic.Add("詘", "诎");
        fanDic.Add("詔", "诏");
        fanDic.Add("譯", "译");
        fanDic.Add("詒", "诒");
        fanDic.Add("誆", "诓");
        fanDic.Add("誄", "诔");
        fanDic.Add("試", "试");
        fanDic.Add("詿", "诖");
        fanDic.Add("詩", "诗");
        fanDic.Add("詰", "诘");
        fanDic.Add("詼", "诙");
        fanDic.Add("誠", "诚");
        fanDic.Add("誅", "诛");
        fanDic.Add("詵", "诜");
        fanDic.Add("話", "话");
        fanDic.Add("誕", "诞");
        fanDic.Add("詬", "诟");
        fanDic.Add("詮", "诠");
        fanDic.Add("詭", "诡");
        fanDic.Add("詢", "询");
        fanDic.Add("詣", "诣");
        fanDic.Add("諍", "诤");
        fanDic.Add("該", "该");
        fanDic.Add("詳", "详");
        fanDic.Add("詫", "诧");
        fanDic.Add("諢", "诨");
        fanDic.Add("詡", "诩");
        fanDic.Add("誡", "诫");
        fanDic.Add("誣", "诬");
        fanDic.Add("語", "语");
        fanDic.Add("誚", "诮");
        fanDic.Add("誤", "误");
        fanDic.Add("誥", "诰");
        fanDic.Add("誘", "诱");
        fanDic.Add("誨", "诲");
        fanDic.Add("誑", "诳");
        fanDic.Add("說", "说");
        fanDic.Add("誦", "诵");
        fanDic.Add("誒", "诶");
        fanDic.Add("請", "请");
        fanDic.Add("諸", "诸");
        fanDic.Add("諏", "诹");
        fanDic.Add("諾", "诺");
        fanDic.Add("讀", "读");
        fanDic.Add("諑", "诼");
        fanDic.Add("誹", "诽");
        fanDic.Add("課", "课");
        fanDic.Add("諉", "诿");
        fanDic.Add("諛", "谀");
        fanDic.Add("誰", "谁");
        fanDic.Add("諗", "谂");
        fanDic.Add("調", "调");
        fanDic.Add("諂", "谄");
        fanDic.Add("諒", "谅");
        fanDic.Add("諄", "谆");
        fanDic.Add("誶", "谇");
        fanDic.Add("談", "谈");
        fanDic.Add("誼", "谊");
        fanDic.Add("謀", "谋");
        fanDic.Add("諶", "谌");
        fanDic.Add("諜", "谍");
        fanDic.Add("謊", "谎");
        fanDic.Add("諫", "谏");
        fanDic.Add("諧", "谐");
        fanDic.Add("謔", "谑");
        fanDic.Add("謁", "谒");
        fanDic.Add("謂", "谓");
        fanDic.Add("諤", "谔");
        fanDic.Add("諭", "谕");
        fanDic.Add("諼", "谖");
        fanDic.Add("讒", "谗");
        fanDic.Add("諮", "谘");
        fanDic.Add("諳", "谙");
        fanDic.Add("諺", "谚");
        fanDic.Add("諦", "谛");
        fanDic.Add("謎", "谜");
        fanDic.Add("諞", "谝");
        fanDic.Add("謨", "谟");
        fanDic.Add("讜", "谠");
        fanDic.Add("謖", "谡");
        fanDic.Add("謝", "谢");
        fanDic.Add("謠", "谣");
        fanDic.Add("謗", "谤");
        fanDic.Add("謚", "谥");
        fanDic.Add("謙", "谦");
        fanDic.Add("謐", "谧");
        fanDic.Add("謹", "谨");
        fanDic.Add("謾", "谩");
        fanDic.Add("謫", "谪");
        fanDic.Add("謬", "谬");
        fanDic.Add("譚", "谭");
        fanDic.Add("譖", "谮");
        fanDic.Add("譙", "谯");
        fanDic.Add("讕", "谰");
        fanDic.Add("譜", "谱");
        fanDic.Add("譎", "谲");
        fanDic.Add("讞", "谳");
        fanDic.Add("譴", "谴");
        fanDic.Add("譫", "谵");
        fanDic.Add("讖", "谶");
        fanDic.Add("貝", "贝");
        fanDic.Add("貞", "贞");
        fanDic.Add("負", "负");
        fanDic.Add("貢", "贡");
        fanDic.Add("財", "财");
        fanDic.Add("責", "责");
        fanDic.Add("賢", "贤");
        fanDic.Add("敗", "败");
        fanDic.Add("賬", "账");
        fanDic.Add("貨", "货");
        fanDic.Add("質", "质");
        fanDic.Add("販", "贩");
        fanDic.Add("貪", "贪");
        fanDic.Add("貧", "贫");
        fanDic.Add("貶", "贬");
        fanDic.Add("購", "购");
        fanDic.Add("貯", "贮");
        fanDic.Add("貫", "贯");
        fanDic.Add("貳", "贰");
        fanDic.Add("賤", "贱");
        fanDic.Add("賁", "贲");
        fanDic.Add("貰", "贳");
        fanDic.Add("貼", "贴");
        fanDic.Add("貴", "贵");
        fanDic.Add("貺", "贶");
        fanDic.Add("貸", "贷");
        fanDic.Add("貿", "贸");
        fanDic.Add("費", "费");
        fanDic.Add("賀", "贺");
        fanDic.Add("貽", "贻");
        fanDic.Add("賊", "贼");
        fanDic.Add("贄", "贽");
        fanDic.Add("賈", "贾");
        fanDic.Add("賄", "贿");
        fanDic.Add("貲", "赀");
        fanDic.Add("賃", "赁");
        fanDic.Add("賂", "赂");
        fanDic.Add("贓", "赃");
        fanDic.Add("資", "资");
        fanDic.Add("賅", "赅");
        fanDic.Add("贐", "赆");
        fanDic.Add("賕", "赇");
        fanDic.Add("賑", "赈");
        fanDic.Add("賚", "赉");
        fanDic.Add("賒", "赊");
        fanDic.Add("賦", "赋");
        fanDic.Add("賭", "赌");
        fanDic.Add("贖", "赎");
        fanDic.Add("賞", "赏");
        fanDic.Add("賜", "赐");
        fanDic.Add("賡", "赓");
        fanDic.Add("賠", "赔");
        fanDic.Add("賧", "赕");
        fanDic.Add("賴", "赖");
        fanDic.Add("贅", "赘");
        fanDic.Add("賻", "赙");
        fanDic.Add("賺", "赚");
        fanDic.Add("賽", "赛");
        fanDic.Add("賾", "赜");
        fanDic.Add("贗", "赝");
        fanDic.Add("贊", "赞");
        fanDic.Add("贈", "赠");
        fanDic.Add("贍", "赡");
        fanDic.Add("贏", "赢");
        fanDic.Add("贛", "赣");
        fanDic.Add("趙", "赵");
        fanDic.Add("趕", "赶");
        fanDic.Add("趨", "趋");
        fanDic.Add("趲", "趱");
        fanDic.Add("躉", "趸");
        fanDic.Add("躍", "跃");
        fanDic.Add("蹌", "跄");
        fanDic.Add("躒", "跞");
        fanDic.Add("踐", "践");
        fanDic.Add("蹺", "跷");
        fanDic.Add("蹕", "跸");
        fanDic.Add("躚", "跹");
        fanDic.Add("躋", "跻");
        fanDic.Add("踴", "踊");
        fanDic.Add("躊", "踌");
        fanDic.Add("蹤", "踪");
        fanDic.Add("躓", "踬");
        fanDic.Add("躑", "踯");
        fanDic.Add("躡", "蹑");
        fanDic.Add("蹣", "蹒");
        fanDic.Add("躕", "蹰");
        fanDic.Add("躥", "蹿");
        fanDic.Add("躪", "躏");
        fanDic.Add("躦", "躜");
        fanDic.Add("軀", "躯");
        fanDic.Add("車", "车");
        fanDic.Add("軋", "轧");
        fanDic.Add("軌", "轨");
        fanDic.Add("軒", "轩");
        fanDic.Add("軔", "轫");
        fanDic.Add("轉", "转");
        fanDic.Add("軛", "轭");
        fanDic.Add("輪", "轮");
        fanDic.Add("軟", "软");
        fanDic.Add("轟", "轰");
        fanDic.Add("軻", "轲");
        fanDic.Add("轤", "轳");
        fanDic.Add("軸", "轴");
        fanDic.Add("軹", "轵");
        fanDic.Add("軼", "轶");
        fanDic.Add("軫", "轸");
        fanDic.Add("轢", "轹");
        fanDic.Add("軺", "轺");
        fanDic.Add("輕", "轻");
        fanDic.Add("軾", "轼");
        fanDic.Add("載", "载");
        fanDic.Add("輊", "轾");
        fanDic.Add("轎", "轿");
        fanDic.Add("輇", "辁");
        fanDic.Add("輅", "辂");
        fanDic.Add("較", "较");
        fanDic.Add("輒", "辄");
        fanDic.Add("輔", "辅");
        fanDic.Add("輛", "辆");
        fanDic.Add("輦", "辇");
        fanDic.Add("輩", "辈");
        fanDic.Add("輝", "辉");
        fanDic.Add("輥", "辊");
        fanDic.Add("輞", "辋");
        fanDic.Add("輟", "辍");
        fanDic.Add("輜", "辎");
        fanDic.Add("輳", "辏");
        fanDic.Add("輻", "辐");
        fanDic.Add("輯", "辑");
        fanDic.Add("輸", "输");
        fanDic.Add("轡", "辔");
        fanDic.Add("轅", "辕");
        fanDic.Add("轄", "辖");
        fanDic.Add("輾", "辗");
        fanDic.Add("轆", "辘");
        fanDic.Add("轍", "辙");
        fanDic.Add("轔", "辚");
        fanDic.Add("辭", "辞");
        fanDic.Add("辯", "辩");
        fanDic.Add("辮", "辫");
        fanDic.Add("邊", "边");
        fanDic.Add("遼", "辽");
        fanDic.Add("達", "达");
        fanDic.Add("遷", "迁");
        fanDic.Add("過", "过");
        fanDic.Add("邁", "迈");
        fanDic.Add("運", "运");
        fanDic.Add("還", "还");
        fanDic.Add("這", "这");
        fanDic.Add("進", "进");
        fanDic.Add("遠", "远");
        fanDic.Add("違", "违");
        fanDic.Add("連", "连");
        fanDic.Add("遲", "迟");
        fanDic.Add("邇", "迩");
        fanDic.Add("逕", "迳");
        fanDic.Add("跡", "迹");
        fanDic.Add("適", "适");
        fanDic.Add("選", "选");
        fanDic.Add("遜", "逊");
        fanDic.Add("遞", "递");
        fanDic.Add("邐", "逦");
        fanDic.Add("邏", "逻");
        fanDic.Add("遺", "遗");
        fanDic.Add("遙", "遥");
        fanDic.Add("鄧", "邓");
        fanDic.Add("鄺", "邝");
        fanDic.Add("鄔", "邬");
        fanDic.Add("郵", "邮");
        fanDic.Add("鄒", "邹");
        fanDic.Add("鄴", "邺");
        fanDic.Add("鄰", "邻");
        fanDic.Add("郟", "郏");
        fanDic.Add("鄶", "郐");
        fanDic.Add("鄭", "郑");
        fanDic.Add("鄆", "郓");
        fanDic.Add("酈", "郦");
        fanDic.Add("鄖", "郧");
        fanDic.Add("鄲", "郸");
        fanDic.Add("醞", "酝");
        fanDic.Add("醬", "酱");
        fanDic.Add("釅", "酽");
        fanDic.Add("釃", "酾");
        fanDic.Add("釀", "酿");
        fanDic.Add("釋", "释");
        fanDic.Add("鑒", "鉴");
        fanDic.Add("鑾", "銮");
        fanDic.Add("鏨", "錾");
        fanDic.Add("釓", "钆");
        fanDic.Add("釔", "钇");
        fanDic.Add("針", "针");
        fanDic.Add("釘", "钉");
        fanDic.Add("釗", "钊");
        fanDic.Add("釙", "钋");
        fanDic.Add("釕", "钌");
        fanDic.Add("釷", "钍");
        fanDic.Add("釬", "钎");
        fanDic.Add("釧", "钏");
        fanDic.Add("釤", "钐");
        fanDic.Add("釩", "钒");
        fanDic.Add("釣", "钓");
        fanDic.Add("鍆", "钔");
        fanDic.Add("釹", "钕");
        fanDic.Add("釵", "钗");
        fanDic.Add("鈣", "钙");
        fanDic.Add("鈦", "钛");
        fanDic.Add("鉅", "钜");
        fanDic.Add("鈍", "钝");
        fanDic.Add("鈔", "钞");
        fanDic.Add("鐘", "钟");
        fanDic.Add("鈉", "钠");
        fanDic.Add("鋇", "钡");
        fanDic.Add("鋼", "钢");
        fanDic.Add("鈑", "钣");
        fanDic.Add("鈐", "钤");
        fanDic.Add("鑰", "钥");
        fanDic.Add("欽", "钦");
        fanDic.Add("鈞", "钧");
        fanDic.Add("鎢", "钨");
        fanDic.Add("鉤", "钩");
        fanDic.Add("鈧", "钪");
        fanDic.Add("鈁", "钫");
        fanDic.Add("鈥", "钬");
        fanDic.Add("鈄", "钭");
        fanDic.Add("鈕", "钮");
        fanDic.Add("鈀", "钯");
        fanDic.Add("鈺", "钰");
        fanDic.Add("錢", "钱");
        fanDic.Add("鉦", "钲");
        fanDic.Add("鉗", "钳");
        fanDic.Add("鈷", "钴");
        fanDic.Add("缽", "钵");
        fanDic.Add("鈳", "钶");
        fanDic.Add("鈽", "钸");
        fanDic.Add("鈸", "钹");
        fanDic.Add("鉞", "钺");
        fanDic.Add("鉆", "钻");
        fanDic.Add("鉬", "钼");
        fanDic.Add("鉭", "钽");
        fanDic.Add("鉀", "钾");
        fanDic.Add("鈿", "钿");
        fanDic.Add("鈾", "铀");
        fanDic.Add("鐵", "铁");
        fanDic.Add("鉑", "铂");
        fanDic.Add("鈴", "铃");
        fanDic.Add("鑠", "铄");
        fanDic.Add("鉛", "铅");
        fanDic.Add("鉚", "铆");
        fanDic.Add("鈰", "铈");
        fanDic.Add("鉉", "铉");
        fanDic.Add("鉈", "铊");
        fanDic.Add("鉍", "铋");
        fanDic.Add("鈮", "铌");
        fanDic.Add("鈹", "铍");
        fanDic.Add("鐸", "铎");
        fanDic.Add("銬", "铐");
        fanDic.Add("銠", "铑");
        fanDic.Add("鉺", "铒");
        fanDic.Add("銪", "铕");
        fanDic.Add("鋮", "铖");
        fanDic.Add("鋏", "铗");
        fanDic.Add("鐃", "铙");
        fanDic.Add("鐺", "铛");
        fanDic.Add("銅", "铜");
        fanDic.Add("鋁", "铝");
        fanDic.Add("銦", "铟");
        fanDic.Add("鎧", "铠");
        fanDic.Add("鍘", "铡");
        fanDic.Add("銖", "铢");
        fanDic.Add("銑", "铣");
        fanDic.Add("鋌", "铤");
        fanDic.Add("銩", "铥");
        fanDic.Add("鏵", "铧");
        fanDic.Add("銓", "铨");
        fanDic.Add("鎩", "铩");
        fanDic.Add("鉿", "铪");
        fanDic.Add("銚", "铫");
        fanDic.Add("鉻", "铬");
        fanDic.Add("銘", "铭");
        fanDic.Add("錚", "铮");
        fanDic.Add("銫", "铯");
        fanDic.Add("鉸", "铰");
        fanDic.Add("銥", "铱");
        fanDic.Add("鏟", "铲");
        fanDic.Add("銃", "铳");
        fanDic.Add("鐋", "铴");
        fanDic.Add("銨", "铵");
        fanDic.Add("銀", "银");
        fanDic.Add("銣", "铷");
        fanDic.Add("鑄", "铸");
        fanDic.Add("鐒", "铹");
        fanDic.Add("鋪", "铺");
        fanDic.Add("錸", "铼");
        fanDic.Add("鋱", "铽");
        fanDic.Add("鏈", "链");
        fanDic.Add("鏗", "铿");
        fanDic.Add("銷", "销");
        fanDic.Add("鎖", "锁");
        fanDic.Add("鋰", "锂");
        fanDic.Add("鋤", "锄");
        fanDic.Add("鍋", "锅");
        fanDic.Add("鋯", "锆");
        fanDic.Add("鋨", "锇");
        fanDic.Add("銹", "锈");
        fanDic.Add("銼", "锉");
        fanDic.Add("鋝", "锊");
        fanDic.Add("鋒", "锋");
        fanDic.Add("鋅", "锌");
        fanDic.Add("銳", "锐");
        fanDic.Add("銻", "锑");
        fanDic.Add("鋃", "锒");
        fanDic.Add("鋟", "锓");
        fanDic.Add("鋦", "锔");
        fanDic.Add("錒", "锕");
        fanDic.Add("錆", "锖");
        fanDic.Add("鍺", "锗");
        fanDic.Add("錯", "错");
        fanDic.Add("錨", "锚");
        fanDic.Add("錛", "锛");
        fanDic.Add("錁", "锞");
        fanDic.Add("錕", "锟");
        fanDic.Add("錫", "锡");
        fanDic.Add("錮", "锢");
        fanDic.Add("鑼", "锣");
        fanDic.Add("錘", "锤");
        fanDic.Add("錐", "锥");
        fanDic.Add("錦", "锦");
        fanDic.Add("錈", "锩");
        fanDic.Add("錟", "锬");
        fanDic.Add("錠", "锭");
        fanDic.Add("鍵", "键");
        fanDic.Add("鋸", "锯");
        fanDic.Add("錳", "锰");
        fanDic.Add("錙", "锱");
        fanDic.Add("鍥", "锲");
        fanDic.Add("鍇", "锴");
        fanDic.Add("鏘", "锵");
        fanDic.Add("鍶", "锶");
        fanDic.Add("鍔", "锷");
        fanDic.Add("鍤", "锸");
        fanDic.Add("鍬", "锹");
        fanDic.Add("鍾", "锺");
        fanDic.Add("鍛", "锻");
        fanDic.Add("鎪", "锼");
        fanDic.Add("鍰", "锾");
        fanDic.Add("鍍", "镀");
        fanDic.Add("鎂", "镁");
        fanDic.Add("鏤", "镂");
        fanDic.Add("鐨", "镄");
        fanDic.Add("鏌", "镆");
        fanDic.Add("鎮", "镇");
        fanDic.Add("鎘", "镉");
        fanDic.Add("鑷", "镊");
        fanDic.Add("鐫", "镌");
        fanDic.Add("鎳", "镍");
        fanDic.Add("鎦", "镏");
        fanDic.Add("鎬", "镐");
        fanDic.Add("鎊", "镑");
        fanDic.Add("鎰", "镒");
        fanDic.Add("鎵", "镓");
        fanDic.Add("鑌", "镔");
        fanDic.Add("鏢", "镖");
        fanDic.Add("鏜", "镗");
        fanDic.Add("鏝", "镘");
        fanDic.Add("鏍", "镙");
        fanDic.Add("鏞", "镛");
        fanDic.Add("鏡", "镜");
        fanDic.Add("鏑", "镝");
        fanDic.Add("鏃", "镞");
        fanDic.Add("鏇", "镟");
        fanDic.Add("鐔", "镡");
        fanDic.Add("鐐", "镣");
        fanDic.Add("鏷", "镤");
        fanDic.Add("鐓", "镦");
        fanDic.Add("鑭", "镧");
        fanDic.Add("鐠", "镨");
        fanDic.Add("鏹", "镪");
        fanDic.Add("鐙", "镫");
        fanDic.Add("鑊", "镬");
        fanDic.Add("鐳", "镭");
        fanDic.Add("鐲", "镯");
        fanDic.Add("鐮", "镰");
        fanDic.Add("鐿", "镱");
        fanDic.Add("鑣", "镳");
        fanDic.Add("鑲", "镶");
        fanDic.Add("長", "长");
        fanDic.Add("門", "门");
        fanDic.Add("閂", "闩");
        fanDic.Add("閃", "闪");
        fanDic.Add("閆", "闫");
        fanDic.Add("閉", "闭");
        fanDic.Add("問", "问");
        fanDic.Add("闖", "闯");
        fanDic.Add("閏", "闰");
        fanDic.Add("闈", "闱");
        fanDic.Add("閑", "闲");
        fanDic.Add("閎", "闳");
        fanDic.Add("間", "间");
        fanDic.Add("閔", "闵");
        fanDic.Add("閌", "闶");
        fanDic.Add("悶", "闷");
        fanDic.Add("閘", "闸");
        fanDic.Add("鬧", "闹");
        fanDic.Add("閨", "闺");
        fanDic.Add("聞", "闻");
        fanDic.Add("闥", "闼");
        fanDic.Add("閩", "闽");
        fanDic.Add("閭", "闾");
        fanDic.Add("閥", "阀");
        fanDic.Add("閣", "阁");
        fanDic.Add("閡", "阂");
        fanDic.Add("閫", "阃");
        fanDic.Add("鬮", "阄");
        fanDic.Add("閱", "阅");
        fanDic.Add("閬", "阆");
        fanDic.Add("閾", "阈");
        fanDic.Add("閹", "阉");
        fanDic.Add("閶", "阊");
        fanDic.Add("鬩", "阋");
        fanDic.Add("閿", "阌");
        fanDic.Add("閽", "阍");
        fanDic.Add("閻", "阎");
        fanDic.Add("閼", "阏");
        fanDic.Add("闡", "阐");
        fanDic.Add("闌", "阑");
        fanDic.Add("闃", "阒");
        fanDic.Add("闊", "阔");
        fanDic.Add("闋", "阕");
        fanDic.Add("闔", "阖");
        fanDic.Add("闐", "阗");
        fanDic.Add("闕", "阙");
        fanDic.Add("闞", "阚");
        fanDic.Add("隊", "队");
        fanDic.Add("陽", "阳");
        fanDic.Add("陰", "阴");
        fanDic.Add("陣", "阵");
        fanDic.Add("階", "阶");
        fanDic.Add("際", "际");
        fanDic.Add("陸", "陆");
        fanDic.Add("隴", "陇");
        fanDic.Add("陳", "陈");
        fanDic.Add("陘", "陉");
        fanDic.Add("陜", "陕");
        fanDic.Add("隉", "陧");
        fanDic.Add("隕", "陨");
        fanDic.Add("險", "险");
        fanDic.Add("隨", "随");
        fanDic.Add("隱", "隐");
        fanDic.Add("隸", "隶");
        fanDic.Add("雋", "隽");
        fanDic.Add("難", "难");
        fanDic.Add("雛", "雏");
        fanDic.Add("讎", "雠");
        fanDic.Add("靂", "雳");
        fanDic.Add("霧", "雾");
        fanDic.Add("霽", "霁");
        fanDic.Add("靄", "霭");
        fanDic.Add("靚", "靓");
        fanDic.Add("靜", "静");
        fanDic.Add("靨", "靥");
        fanDic.Add("韃", "鞑");
        fanDic.Add("韉", "鞯");
        fanDic.Add("韋", "韦");
        fanDic.Add("韌", "韧");
        fanDic.Add("韓", "韩");
        fanDic.Add("韙", "韪");
        fanDic.Add("韞", "韫");
        fanDic.Add("韜", "韬");
        fanDic.Add("韻", "韵");
        fanDic.Add("頁", "页");
        fanDic.Add("頂", "顶");
        fanDic.Add("頃", "顷");
        fanDic.Add("頇", "顸");
        fanDic.Add("項", "项");
        fanDic.Add("順", "顺");
        fanDic.Add("須", "须");
        fanDic.Add("頊", "顼");
        fanDic.Add("頑", "顽");
        fanDic.Add("顧", "顾");
        fanDic.Add("頓", "顿");
        fanDic.Add("頎", "颀");
        fanDic.Add("頒", "颁");
        fanDic.Add("頌", "颂");
        fanDic.Add("頏", "颃");
        fanDic.Add("預", "预");
        fanDic.Add("顱", "颅");
        fanDic.Add("領", "领");
        fanDic.Add("頗", "颇");
        fanDic.Add("頸", "颈");
        fanDic.Add("頡", "颉");
        fanDic.Add("頰", "颊");
        fanDic.Add("頜", "颌");
        fanDic.Add("潁", "颍");
        fanDic.Add("頦", "颏");
        fanDic.Add("頤", "颐");
        fanDic.Add("頻", "频");
        fanDic.Add("頹", "颓");
        fanDic.Add("頷", "颔");
        fanDic.Add("穎", "颖");
        fanDic.Add("顆", "颗");
        fanDic.Add("題", "题");
        fanDic.Add("顎", "颚");
        fanDic.Add("顓", "颛");
        fanDic.Add("顏", "颜");
        fanDic.Add("額", "额");
        fanDic.Add("顳", "颞");
        fanDic.Add("顢", "颟");
        fanDic.Add("顛", "颠");
        fanDic.Add("顙", "颡");
        fanDic.Add("顥", "颢");
        fanDic.Add("顫", "颤");
        fanDic.Add("顰", "颦");
        fanDic.Add("顴", "颧");
        fanDic.Add("風", "风");
        fanDic.Add("颮", "飑");
        fanDic.Add("颯", "飒");
        fanDic.Add("颶", "飓");
        fanDic.Add("颼", "飕");
        fanDic.Add("飄", "飘");
        fanDic.Add("飆", "飙");
        fanDic.Add("飛", "飞");
        fanDic.Add("饗", "飨");
        fanDic.Add("饜", "餍");
        fanDic.Add("饑", "饥");
        fanDic.Add("餳", "饧");
        fanDic.Add("飩", "饨");
        fanDic.Add("餼", "饩");
        fanDic.Add("飪", "饪");
        fanDic.Add("飫", "饫");
        fanDic.Add("飭", "饬");
        fanDic.Add("飯", "饭");
        fanDic.Add("飲", "饮");
        fanDic.Add("餞", "饯");
        fanDic.Add("飾", "饰");
        fanDic.Add("飽", "饱");
        fanDic.Add("飼", "饲");
        fanDic.Add("飴", "饴");
        fanDic.Add("餌", "饵");
        fanDic.Add("饒", "饶");
        fanDic.Add("餉", "饷");
        fanDic.Add("餃", "饺");
        fanDic.Add("餅", "饼");
        fanDic.Add("餑", "饽");
        fanDic.Add("餓", "饿");
        fanDic.Add("餒", "馁");
        fanDic.Add("餛", "馄");
        fanDic.Add("餡", "馅");
        fanDic.Add("館", "馆");
        fanDic.Add("饋", "馈");
        fanDic.Add("餿", "馊");
        fanDic.Add("饞", "馋");
        fanDic.Add("饃", "馍");
        fanDic.Add("餾", "馏");
        fanDic.Add("饈", "馐");
        fanDic.Add("饉", "馑");
        fanDic.Add("饅", "馒");
        fanDic.Add("饌", "馔");
        fanDic.Add("馬", "马");
        fanDic.Add("馭", "驭");
        fanDic.Add("馱", "驮");
        fanDic.Add("馴", "驯");
        fanDic.Add("馳", "驰");
        fanDic.Add("驅", "驱");
        fanDic.Add("駁", "驳");
        fanDic.Add("驢", "驴");
        fanDic.Add("駔", "驵");
        fanDic.Add("駛", "驶");
        fanDic.Add("駟", "驷");
        fanDic.Add("駙", "驸");
        fanDic.Add("駒", "驹");
        fanDic.Add("騶", "驺");
        fanDic.Add("駐", "驻");
        fanDic.Add("駝", "驼");
        fanDic.Add("駑", "驽");
        fanDic.Add("駕", "驾");
        fanDic.Add("驛", "驿");
        fanDic.Add("駘", "骀");
        fanDic.Add("驍", "骁");
        fanDic.Add("罵", "骂");
        fanDic.Add("驕", "骄");
        fanDic.Add("驊", "骅");
        fanDic.Add("駱", "骆");
        fanDic.Add("駭", "骇");
        fanDic.Add("駢", "骈");
        fanDic.Add("驪", "骊");
        fanDic.Add("騁", "骋");
        fanDic.Add("驗", "验");
        fanDic.Add("駿", "骏");
        fanDic.Add("騏", "骐");
        fanDic.Add("騎", "骑");
        fanDic.Add("騍", "骒");
        fanDic.Add("騅", "骓");
        fanDic.Add("驂", "骖");
        fanDic.Add("騙", "骗");
        fanDic.Add("騭", "骘");
        fanDic.Add("騷", "骚");
        fanDic.Add("騖", "骛");
        fanDic.Add("驁", "骜");
        fanDic.Add("騮", "骝");
        fanDic.Add("騫", "骞");
        fanDic.Add("騸", "骟");
        fanDic.Add("驃", "骠");
        fanDic.Add("騾", "骡");
        fanDic.Add("驄", "骢");
        fanDic.Add("驏", "骣");
        fanDic.Add("驟", "骤");
        fanDic.Add("驥", "骥");
        fanDic.Add("驤", "骧");
        fanDic.Add("髏", "髅");
        fanDic.Add("髖", "髋");
        fanDic.Add("髕", "髌");
        fanDic.Add("鬢", "鬓");
        fanDic.Add("魘", "魇");
        fanDic.Add("魎", "魉");
        fanDic.Add("魚", "鱼");
        fanDic.Add("魷", "鱿");
        fanDic.Add("魯", "鲁");
        fanDic.Add("魴", "鲂");
        fanDic.Add("鱸", "鲈");
        fanDic.Add("鮒", "鲋");
        fanDic.Add("鮑", "鲍");
        fanDic.Add("鱟", "鲎");
        fanDic.Add("鮐", "鲐");
        fanDic.Add("鮭", "鲑");
        fanDic.Add("鮚", "鲒");
        fanDic.Add("鮪", "鲔");
        fanDic.Add("鮞", "鲕");
        fanDic.Add("鱭", "鲚");
        fanDic.Add("鮫", "鲛");
        fanDic.Add("鮮", "鲜");
        fanDic.Add("鱘", "鲟");
        fanDic.Add("鯁", "鲠");
        fanDic.Add("鱺", "鲡");
        fanDic.Add("鰱", "鲢");
        fanDic.Add("鰹", "鲣");
        fanDic.Add("鯉", "鲤");
        fanDic.Add("鰣", "鲥");
        fanDic.Add("鰷", "鲦");
        fanDic.Add("鯀", "鲧");
        fanDic.Add("鯊", "鲨");
        fanDic.Add("鯇", "鲩");
        fanDic.Add("鯽", "鲫");
        fanDic.Add("鯖", "鲭");
        fanDic.Add("鯪", "鲮");
        fanDic.Add("鯫", "鲰");
        fanDic.Add("鯡", "鲱");
        fanDic.Add("鯤", "鲲");
        fanDic.Add("鯧", "鲳");
        fanDic.Add("鯢", "鲵");
        fanDic.Add("鯰", "鲶");
        fanDic.Add("鯛", "鲷");
        fanDic.Add("鯨", "鲸");
        fanDic.Add("鯔", "鲻");
        fanDic.Add("鰈", "鲽");
        fanDic.Add("鰓", "鳃");
        fanDic.Add("鱷", "鳄");
        fanDic.Add("鰍", "鳅");
        fanDic.Add("鰒", "鳆");
        fanDic.Add("鰉", "鳇");
        fanDic.Add("鰲", "鳌");
        fanDic.Add("鰭", "鳍");
        fanDic.Add("鰨", "鳎");
        fanDic.Add("鰥", "鳏");
        fanDic.Add("鰩", "鳐");
        fanDic.Add("鰳", "鳓");
        fanDic.Add("鰾", "鳔");
        fanDic.Add("鱈", "鳕");
        fanDic.Add("鱉", "鳖");
        fanDic.Add("鰻", "鳗");
        fanDic.Add("鱖", "鳜");
        fanDic.Add("鱔", "鳝");
        fanDic.Add("鱗", "鳞");
        fanDic.Add("鱒", "鳟");
        fanDic.Add("鱧", "鳢");
        fanDic.Add("鳥", "鸟");
        fanDic.Add("鳩", "鸠");
        fanDic.Add("雞", "鸡");
        fanDic.Add("鳶", "鸢");
        fanDic.Add("鳴", "鸣");
        fanDic.Add("鷗", "鸥");
        fanDic.Add("鴉", "鸦");
        fanDic.Add("鴇", "鸨");
        fanDic.Add("鴆", "鸩");
        fanDic.Add("鴣", "鸪");
        fanDic.Add("鶇", "鸫");
        fanDic.Add("鸕", "鸬");
        fanDic.Add("鴨", "鸭");
        fanDic.Add("鴦", "鸯");
        fanDic.Add("鴟", "鸱");
        fanDic.Add("鴝", "鸲");
        fanDic.Add("鴛", "鸳");
        fanDic.Add("鴕", "鸵");
        fanDic.Add("鷥", "鸶");
        fanDic.Add("鷙", "鸷");
        fanDic.Add("鴯", "鸸");
        fanDic.Add("鴰", "鸹");
        fanDic.Add("鵂", "鸺");
        fanDic.Add("鴿", "鸽");
        fanDic.Add("鸞", "鸾");
        fanDic.Add("鴻", "鸿");
        fanDic.Add("鵓", "鹁");
        fanDic.Add("鸝", "鹂");
        fanDic.Add("鵑", "鹃");
        fanDic.Add("鵠", "鹄");
        fanDic.Add("鵝", "鹅");
        fanDic.Add("鵒", "鹆");
        fanDic.Add("鷴", "鹇");
        fanDic.Add("鵜", "鹈");
        fanDic.Add("鵡", "鹉");
        fanDic.Add("鵲", "鹊");
        fanDic.Add("鵪", "鹌");
        fanDic.Add("鵯", "鹎");
        fanDic.Add("鵬", "鹏");
        fanDic.Add("鶉", "鹑");
        fanDic.Add("鶘", "鹕");
        fanDic.Add("鶚", "鹗");
        fanDic.Add("鶻", "鹘");
        fanDic.Add("鶿", "鹚");
        fanDic.Add("鶩", "鹜");
        fanDic.Add("鷂", "鹞");
        fanDic.Add("鶼", "鹣");
        fanDic.Add("鶴", "鹤");
        fanDic.Add("鸚", "鹦");
        fanDic.Add("鷓", "鹧");
        fanDic.Add("鷚", "鹨");
        fanDic.Add("鷯", "鹩");
        fanDic.Add("鷦", "鹪");
        fanDic.Add("鷲", "鹫");
        fanDic.Add("鷸", "鹬");
        fanDic.Add("鷺", "鹭");
        fanDic.Add("鷹", "鹰");
        fanDic.Add("鸛", "鹳");
        fanDic.Add("鹺", "鹾");
        fanDic.Add("麥", "麦");
        fanDic.Add("麩", "麸");
        fanDic.Add("麼", "么");
        fanDic.Add("黃", "黄");
        fanDic.Add("黌", "黉");
        fanDic.Add("黷", "黩");
        fanDic.Add("黲", "黪");
        fanDic.Add("黽", "黾");
        fanDic.Add("黿", "鼋");
        fanDic.Add("鼉", "鼍");
        fanDic.Add("鼴", "鼹");
        fanDic.Add("齊", "齐");
        fanDic.Add("齏", "齑");
        fanDic.Add("齒", "齿");
        fanDic.Add("齔", "龀");
        fanDic.Add("齟", "龃");
        fanDic.Add("齡", "龄");
        fanDic.Add("齙", "龅");
        fanDic.Add("齠", "龆");
        fanDic.Add("齜", "龇");
        fanDic.Add("齦", "龈");
        fanDic.Add("齬", "龉");
        fanDic.Add("齪", "龊");
        fanDic.Add("齲", "龋");
        fanDic.Add("齷", "龌");
        fanDic.Add("龍", "龙");
        fanDic.Add("龔", "龚");
        fanDic.Add("龕", "龛");
        fanDic.Add("龜", "龟");
        #endregion
        #region 简转繁
        jianDic.Add("万", "萬");
        jianDic.Add("与", "與");
        jianDic.Add("专", "專");
        jianDic.Add("业", "業");
        jianDic.Add("丛", "叢");
        jianDic.Add("东", "東");
        jianDic.Add("丝", "絲");
        jianDic.Add("丢", "丟");
        jianDic.Add("两", "兩");
        jianDic.Add("严", "嚴");
        jianDic.Add("丧", "喪");
        jianDic.Add("个", "個");
        jianDic.Add("丰", "豐");
        jianDic.Add("临", "臨");
        jianDic.Add("为", "為");
        jianDic.Add("丽", "麗");
        jianDic.Add("举", "舉");
        jianDic.Add("义", "義");
        jianDic.Add("乌", "烏");
        jianDic.Add("乐", "樂");
        jianDic.Add("乔", "喬");
        jianDic.Add("习", "習");
        jianDic.Add("乡", "鄉");
        jianDic.Add("书", "書");
        jianDic.Add("买", "買");
        jianDic.Add("乱", "亂");
        jianDic.Add("争", "爭");
        jianDic.Add("亏", "虧");
        jianDic.Add("亘", "亙");
        jianDic.Add("亚", "亞");
        jianDic.Add("产", "產");
        jianDic.Add("亩", "畝");
        jianDic.Add("亲", "親");
        jianDic.Add("亵", "褻");
        jianDic.Add("亿", "億");
        jianDic.Add("仅", "僅");
        jianDic.Add("从", "從");
        jianDic.Add("仑", "侖");
        jianDic.Add("仓", "倉");
        jianDic.Add("仪", "儀");
        jianDic.Add("们", "們");
        jianDic.Add("价", "價");
        jianDic.Add("众", "眾");
        jianDic.Add("优", "優");
        jianDic.Add("会", "會");
        jianDic.Add("伛", "傴");
        jianDic.Add("伞", "傘");
        jianDic.Add("伟", "偉");
        jianDic.Add("传", "傳");
        jianDic.Add("伤", "傷");
        jianDic.Add("伥", "倀");
        jianDic.Add("伦", "倫");
        jianDic.Add("伧", "傖");
        jianDic.Add("伪", "偽");
        jianDic.Add("伫", "佇");
        jianDic.Add("体", "體");
        jianDic.Add("佣", "傭");
        jianDic.Add("佥", "僉");
        jianDic.Add("侠", "俠");
        jianDic.Add("侣", "侶");
        jianDic.Add("侥", "僥");
        jianDic.Add("侦", "偵");
        jianDic.Add("侧", "側");
        jianDic.Add("侨", "僑");
        jianDic.Add("侩", "儈");
        jianDic.Add("侪", "儕");
        jianDic.Add("侬", "儂");
        jianDic.Add("俣", "俁");
        jianDic.Add("俦", "儔");
        jianDic.Add("俨", "儼");
        jianDic.Add("俩", "倆");
        jianDic.Add("俪", "儷");
        jianDic.Add("俭", "儉");
        jianDic.Add("债", "債");
        jianDic.Add("倾", "傾");
        jianDic.Add("偬", "傯");
        jianDic.Add("偻", "僂");
        jianDic.Add("偾", "僨");
        jianDic.Add("偿", "償");
        jianDic.Add("傥", "儻");
        jianDic.Add("傧", "儐");
        jianDic.Add("储", "儲");
        jianDic.Add("傩", "儺");
        jianDic.Add("儿", "兒");
        jianDic.Add("兑", "兌");
        jianDic.Add("兖", "兗");
        jianDic.Add("党", "黨");
        jianDic.Add("兰", "蘭");
        jianDic.Add("关", "關");
        jianDic.Add("兴", "興");
        jianDic.Add("兹", "茲");
        jianDic.Add("养", "養");
        jianDic.Add("兽", "獸");
        jianDic.Add("冁", "囅");
        jianDic.Add("内", "內");
        jianDic.Add("冈", "岡");
        jianDic.Add("册", "冊");
        jianDic.Add("写", "寫");
        jianDic.Add("军", "軍");
        jianDic.Add("农", "農");
        jianDic.Add("冯", "馮");
        jianDic.Add("冲", "沖");
        jianDic.Add("决", "決");
        jianDic.Add("况", "況");
        jianDic.Add("冻", "凍");
        jianDic.Add("净", "凈");
        jianDic.Add("准", "準");
        jianDic.Add("凉", "涼");
        jianDic.Add("减", "減");
        jianDic.Add("凑", "湊");
        jianDic.Add("凛", "凜");
        jianDic.Add("几", "幾");
        jianDic.Add("凤", "鳳");
        jianDic.Add("凫", "鳧");
        jianDic.Add("凭", "憑");
        jianDic.Add("凯", "凱");
        jianDic.Add("凶", "兇");
        jianDic.Add("击", "擊");
        jianDic.Add("凿", "鑿");
        jianDic.Add("刍", "芻");
        jianDic.Add("划", "劃");
        jianDic.Add("刘", "劉");
        jianDic.Add("则", "則");
        jianDic.Add("刚", "剛");
        jianDic.Add("创", "創");
        jianDic.Add("删", "刪");
        jianDic.Add("别", "別");
        jianDic.Add("刭", "剄");
        jianDic.Add("刹", "剎");
        jianDic.Add("刽", "劊");
        jianDic.Add("刿", "劌");
        jianDic.Add("剀", "剴");
        jianDic.Add("剂", "劑");
        jianDic.Add("剐", "剮");
        jianDic.Add("剑", "劍");
        jianDic.Add("剥", "剝");
        jianDic.Add("剧", "劇");
        jianDic.Add("劝", "勸");
        jianDic.Add("办", "辦");
        jianDic.Add("务", "務");
        jianDic.Add("劢", "勱");
        jianDic.Add("动", "動");
        jianDic.Add("励", "勵");
        jianDic.Add("劲", "勁");
        jianDic.Add("劳", "勞");
        jianDic.Add("势", "勢");
        jianDic.Add("勋", "勛");
        jianDic.Add("匀", "勻");
        jianDic.Add("匦", "匭");
        jianDic.Add("匮", "匱");
        jianDic.Add("区", "區");
        jianDic.Add("医", "醫");
        jianDic.Add("华", "華");
        jianDic.Add("协", "協");
        jianDic.Add("单", "單");
        jianDic.Add("卖", "賣");
        jianDic.Add("卢", "盧");
        jianDic.Add("卤", "鹵");
        jianDic.Add("卧", "臥");
        jianDic.Add("卫", "衛");
        jianDic.Add("却", "卻");
        jianDic.Add("卺", "巹");
        jianDic.Add("厂", "廠");
        jianDic.Add("厅", "廳");
        jianDic.Add("历", "歷");
        jianDic.Add("厉", "厲");
        jianDic.Add("压", "壓");
        jianDic.Add("厌", "厭");
        jianDic.Add("厍", "厙");
        jianDic.Add("厕", "廁");
        jianDic.Add("厢", "廂");
        jianDic.Add("厣", "厴");
        jianDic.Add("厦", "廈");
        jianDic.Add("厨", "廚");
        jianDic.Add("厩", "廄");
        jianDic.Add("厮", "廝");
        jianDic.Add("县", "縣");
        jianDic.Add("参", "參");
        jianDic.Add("双", "雙");
        jianDic.Add("发", "發");
        jianDic.Add("变", "變");
        jianDic.Add("叙", "敘");
        jianDic.Add("叠", "疊");
        //jianDic.Add("台", "臺");
        jianDic.Add("叶", "葉");
        jianDic.Add("号", "號");
        jianDic.Add("叹", "嘆");
        jianDic.Add("叽", "嘰");
        jianDic.Add("吓", "嚇");
        jianDic.Add("吕", "呂");
        jianDic.Add("吗", "嗎");
        jianDic.Add("吨", "噸");
        jianDic.Add("听", "聽");
        jianDic.Add("启", "啟");
        jianDic.Add("吴", "吳");
        jianDic.Add("呐", "吶");
        jianDic.Add("呒", "嘸");
        jianDic.Add("呓", "囈");
        jianDic.Add("呕", "嘔");
        jianDic.Add("呖", "嚦");
        jianDic.Add("呗", "唄");
        jianDic.Add("员", "員");
        jianDic.Add("呙", "咼");
        jianDic.Add("呛", "嗆");
        jianDic.Add("呜", "嗚");
        jianDic.Add("咏", "詠");
        jianDic.Add("咙", "嚨");
        jianDic.Add("咛", "嚀");
        jianDic.Add("响", "響");
        jianDic.Add("哑", "啞");
        jianDic.Add("哒", "噠");
        jianDic.Add("哓", "嘵");
        jianDic.Add("哔", "嗶");
        jianDic.Add("哕", "噦");
        jianDic.Add("哗", "嘩");
        jianDic.Add("哙", "噲");
        jianDic.Add("哜", "嚌");
        jianDic.Add("哝", "噥");
        jianDic.Add("哟", "喲");
        jianDic.Add("唛", "嘜");
        jianDic.Add("唠", "嘮");
        jianDic.Add("唢", "嗩");
        jianDic.Add("唤", "喚");
        jianDic.Add("啧", "嘖");
        jianDic.Add("啬", "嗇");
        jianDic.Add("啭", "囀");
        jianDic.Add("啮", "嚙");
        jianDic.Add("啸", "嘯");
        jianDic.Add("喷", "噴");
        jianDic.Add("喽", "嘍");
        jianDic.Add("喾", "嚳");
        jianDic.Add("嗫", "囁");
        jianDic.Add("嗳", "噯");
        jianDic.Add("嘘", "噓");
        jianDic.Add("嘤", "嚶");
        jianDic.Add("嘱", "囑");
        jianDic.Add("噜", "嚕");
        jianDic.Add("嚣", "囂");
        jianDic.Add("团", "團");
        jianDic.Add("园", "園");
        jianDic.Add("囱", "囪");
        jianDic.Add("围", "圍");
        jianDic.Add("囵", "圇");
        jianDic.Add("国", "國");
        jianDic.Add("图", "圖");
        jianDic.Add("圆", "圓");
        jianDic.Add("圹", "壙");
        jianDic.Add("场", "場");
        jianDic.Add("坏", "壞");
        jianDic.Add("块", "塊");
        jianDic.Add("坚", "堅");
        jianDic.Add("坛", "壇");
        jianDic.Add("坜", "壢");
        jianDic.Add("坝", "壩");
        jianDic.Add("坞", "塢");
        jianDic.Add("坟", "墳");
        jianDic.Add("坠", "墜");
        jianDic.Add("垄", "壟");
        jianDic.Add("垆", "壚");
        jianDic.Add("垒", "壘");
        jianDic.Add("垦", "墾");
        jianDic.Add("垧", "坰");
        jianDic.Add("垩", "堊");
        jianDic.Add("垫", "墊");
        jianDic.Add("垭", "埡");
        jianDic.Add("垲", "塏");
        jianDic.Add("埘", "塒");
        jianDic.Add("埙", "塤");
        jianDic.Add("埚", "堝");
        jianDic.Add("堑", "塹");
        jianDic.Add("堕", "墮");
        jianDic.Add("墒", "墑");
        jianDic.Add("墙", "墻");
        jianDic.Add("壮", "壯");
        jianDic.Add("声", "聲");
        jianDic.Add("壳", "殼");
        jianDic.Add("壶", "壺");
        jianDic.Add("处", "處");
        jianDic.Add("备", "備");
        jianDic.Add("复", "復");
        jianDic.Add("够", "夠");
        jianDic.Add("头", "頭");
        jianDic.Add("夹", "夾");
        jianDic.Add("夺", "奪");
        jianDic.Add("奁", "奩");
        jianDic.Add("奂", "奐");
        jianDic.Add("奋", "奮");
        jianDic.Add("奖", "獎");
        jianDic.Add("奥", "奧");
        jianDic.Add("妆", "妝");
        jianDic.Add("妇", "婦");
        jianDic.Add("妈", "媽");
        jianDic.Add("妩", "嫵");
        jianDic.Add("妪", "嫗");
        jianDic.Add("妫", "媯");
        jianDic.Add("姗", "姍");
        jianDic.Add("娄", "婁");
        jianDic.Add("娅", "婭");
        jianDic.Add("娆", "嬈");
        jianDic.Add("娇", "嬌");
        jianDic.Add("娈", "孌");
        jianDic.Add("娱", "娛");
        jianDic.Add("娲", "媧");
        jianDic.Add("娴", "嫻");
        jianDic.Add("婴", "嬰");
        jianDic.Add("婵", "嬋");
        jianDic.Add("婶", "嬸");
        jianDic.Add("媪", "媼");
        jianDic.Add("嫒", "嬡");
        jianDic.Add("嫔", "嬪");
        jianDic.Add("嫱", "嬙");
        jianDic.Add("嬷", "嬤");
        jianDic.Add("孙", "孫");
        jianDic.Add("学", "學");
        jianDic.Add("孪", "孿");
        jianDic.Add("宁", "寧");
        jianDic.Add("宝", "寶");
        jianDic.Add("实", "實");
        jianDic.Add("宠", "寵");
        jianDic.Add("审", "審");
        jianDic.Add("宪", "憲");
        jianDic.Add("宫", "宮");
        jianDic.Add("宽", "寬");
        jianDic.Add("宾", "賓");
        jianDic.Add("寝", "寢");
        jianDic.Add("对", "對");
        jianDic.Add("寻", "尋");
        jianDic.Add("导", "導");
        jianDic.Add("寿", "壽");
        jianDic.Add("将", "將");
        jianDic.Add("尔", "爾");
        jianDic.Add("尘", "塵");
        jianDic.Add("尝", "嘗");
        jianDic.Add("尧", "堯");
        jianDic.Add("尴", "尷");
        jianDic.Add("尽", "盡");
        jianDic.Add("层", "層");
        jianDic.Add("屉", "屜");
        jianDic.Add("届", "屆");
        jianDic.Add("属", "屬");
        jianDic.Add("屡", "屢");
        jianDic.Add("屦", "屨");
        jianDic.Add("屿", "嶼");
        jianDic.Add("岁", "歲");
        jianDic.Add("岂", "豈");
        jianDic.Add("岖", "嶇");
        jianDic.Add("岗", "崗");
        jianDic.Add("岘", "峴");
        jianDic.Add("岚", "嵐");
        jianDic.Add("岛", "島");
        jianDic.Add("岩", "巖");
        jianDic.Add("岭", "嶺");
        jianDic.Add("岽", "崠");
        jianDic.Add("岿", "巋");
        jianDic.Add("峄", "嶧");
        jianDic.Add("峡", "峽");
        jianDic.Add("峤", "嶠");
        jianDic.Add("峥", "崢");
        jianDic.Add("峦", "巒");
        jianDic.Add("崂", "嶗");
        jianDic.Add("崃", "崍");
        jianDic.Add("崭", "嶄");
        jianDic.Add("嵘", "嶸");
        jianDic.Add("嵛", "崳");
        jianDic.Add("嵝", "嶁");
        jianDic.Add("巅", "巔");
        jianDic.Add("巩", "鞏");
        jianDic.Add("巯", "巰");
        jianDic.Add("币", "幣");
        jianDic.Add("帅", "帥");
        jianDic.Add("师", "師");
        jianDic.Add("帏", "幃");
        jianDic.Add("帐", "帳");
        jianDic.Add("帘", "簾");
        jianDic.Add("帜", "幟");
        jianDic.Add("带", "帶");
        jianDic.Add("帧", "幀");
        jianDic.Add("帮", "幫");
        jianDic.Add("帱", "幬");
        jianDic.Add("帻", "幘");
        jianDic.Add("帼", "幗");
        jianDic.Add("幂", "冪");
        jianDic.Add("广", "廣");
        jianDic.Add("庄", "莊");
        jianDic.Add("庆", "慶");
        jianDic.Add("庐", "廬");
        jianDic.Add("庑", "廡");
        jianDic.Add("库", "庫");
        jianDic.Add("应", "應");
        jianDic.Add("庙", "廟");
        jianDic.Add("庞", "龐");
        jianDic.Add("废", "廢");
        jianDic.Add("廪", "廩");
        jianDic.Add("开", "開");
        jianDic.Add("异", "異");
        jianDic.Add("弃", "棄");
        jianDic.Add("弑", "弒");
        jianDic.Add("张", "張");
        jianDic.Add("弥", "彌");
        jianDic.Add("弪", "弳");
        jianDic.Add("弯", "彎");
        jianDic.Add("弹", "彈");
        jianDic.Add("强", "強");
        jianDic.Add("归", "歸");
        jianDic.Add("当", "當");
        jianDic.Add("录", "錄");
        jianDic.Add("彦", "彥");
        jianDic.Add("彻", "徹");
        jianDic.Add("径", "徑");
        jianDic.Add("徕", "徠");
        jianDic.Add("忆", "憶");
        jianDic.Add("忏", "懺");
        jianDic.Add("忧", "憂");
        jianDic.Add("忾", "愾");
        jianDic.Add("怀", "懷");
        jianDic.Add("态", "態");
        jianDic.Add("怂", "慫");
        jianDic.Add("怃", "憮");
        jianDic.Add("怄", "慪");
        jianDic.Add("怅", "悵");
        jianDic.Add("怆", "愴");
        jianDic.Add("怜", "憐");
        jianDic.Add("总", "總");
        jianDic.Add("怼", "懟");
        jianDic.Add("怿", "懌");
        jianDic.Add("恋", "戀");
        jianDic.Add("恳", "懇");
        jianDic.Add("恶", "惡");
        jianDic.Add("恸", "慟");
        jianDic.Add("恹", "懨");
        jianDic.Add("恺", "愷");
        jianDic.Add("恻", "惻");
        jianDic.Add("恼", "惱");
        jianDic.Add("恽", "惲");
        jianDic.Add("悦", "悅");
        jianDic.Add("悫", "愨");
        jianDic.Add("悬", "懸");
        jianDic.Add("悭", "慳");
        jianDic.Add("悯", "憫");
        jianDic.Add("惊", "驚");
        jianDic.Add("惧", "懼");
        jianDic.Add("惨", "慘");
        jianDic.Add("惩", "懲");
        jianDic.Add("惫", "憊");
        jianDic.Add("惬", "愜");
        jianDic.Add("惭", "慚");
        jianDic.Add("惮", "憚");
        jianDic.Add("惯", "慣");
        jianDic.Add("愠", "慍");
        jianDic.Add("愤", "憤");
        jianDic.Add("愦", "憒");
        jianDic.Add("慑", "懾");
        jianDic.Add("懑", "懣");
        jianDic.Add("懒", "懶");
        jianDic.Add("戆", "戇");
        jianDic.Add("戋", "戔");
        jianDic.Add("戏", "戲");
        jianDic.Add("戗", "戧");
        jianDic.Add("战", "戰");
        jianDic.Add("戬", "戩");
        jianDic.Add("户", "戶");
        jianDic.Add("扑", "撲");
        jianDic.Add("扞", "捍");
        jianDic.Add("执", "執");
        jianDic.Add("扩", "擴");
        jianDic.Add("扪", "捫");
        jianDic.Add("扫", "掃");
        jianDic.Add("扬", "揚");
        jianDic.Add("扰", "擾");
        jianDic.Add("抚", "撫");
        jianDic.Add("抛", "拋");
        jianDic.Add("抟", "摶");
        jianDic.Add("抠", "摳");
        jianDic.Add("抡", "掄");
        jianDic.Add("抢", "搶");
        jianDic.Add("护", "護");
        jianDic.Add("报", "報");
        jianDic.Add("担", "擔");
        jianDic.Add("拟", "擬");
        jianDic.Add("拢", "攏");
        jianDic.Add("拣", "揀");
        jianDic.Add("拥", "擁");
        jianDic.Add("拦", "攔");
        jianDic.Add("拧", "擰");
        jianDic.Add("拨", "撥");
        jianDic.Add("择", "擇");
        jianDic.Add("挂", "掛");
        jianDic.Add("挚", "摯");
        jianDic.Add("挛", "攣");
        jianDic.Add("挝", "撾");
        jianDic.Add("挞", "撻");
        jianDic.Add("挟", "挾");
        jianDic.Add("挠", "撓");
        jianDic.Add("挡", "擋");
        jianDic.Add("挢", "撟");
        jianDic.Add("挣", "掙");
        jianDic.Add("挤", "擠");
        jianDic.Add("挥", "揮");
        jianDic.Add("捞", "撈");
        jianDic.Add("损", "損");
        jianDic.Add("捡", "撿");
        jianDic.Add("换", "換");
        jianDic.Add("捣", "搗");
        jianDic.Add("据", "據");
        jianDic.Add("掳", "擄");
        jianDic.Add("掴", "摑");
        jianDic.Add("掷", "擲");
        jianDic.Add("掸", "撣");
        jianDic.Add("掺", "摻");
        jianDic.Add("掼", "摜");
        jianDic.Add("揽", "攬");
        jianDic.Add("揿", "撳");
        jianDic.Add("搀", "攙");
        jianDic.Add("搁", "擱");
        jianDic.Add("搂", "摟");
        jianDic.Add("搅", "攪");
        jianDic.Add("携", "攜");
        jianDic.Add("摄", "攝");
        jianDic.Add("摅", "攄");
        jianDic.Add("摆", "擺");
        jianDic.Add("摇", "搖");
        jianDic.Add("摈", "擯");
        jianDic.Add("摊", "攤");
        jianDic.Add("撄", "攖");
        jianDic.Add("撑", "撐");
        jianDic.Add("撵", "攆");
        jianDic.Add("撷", "擷");
        jianDic.Add("撸", "擼");
        jianDic.Add("撺", "攛");
        jianDic.Add("擀", "搟");
        jianDic.Add("擞", "擻");
        jianDic.Add("攒", "攢");
        jianDic.Add("敌", "敵");
        jianDic.Add("敛", "斂");
        jianDic.Add("数", "數");
        jianDic.Add("斋", "齋");
        jianDic.Add("斓", "斕");
        jianDic.Add("斩", "斬");
        jianDic.Add("断", "斷");
        jianDic.Add("无", "無");
        jianDic.Add("旧", "舊");
        jianDic.Add("时", "時");
        jianDic.Add("旷", "曠");
        jianDic.Add("昙", "曇");
        jianDic.Add("昼", "晝");
        jianDic.Add("显", "顯");
        jianDic.Add("晋", "晉");
        jianDic.Add("晒", "曬");
        jianDic.Add("晓", "曉");
        jianDic.Add("晔", "曄");
        jianDic.Add("晕", "暈");
        jianDic.Add("晖", "暉");
        jianDic.Add("暂", "暫");
        jianDic.Add("暧", "曖");
        jianDic.Add("术", "術");
        jianDic.Add("朴", "樸");
        jianDic.Add("机", "機");
        jianDic.Add("杀", "殺");
        jianDic.Add("杂", "雜");
        jianDic.Add("权", "權");
        jianDic.Add("杆", "桿");
        jianDic.Add("条", "條");
        jianDic.Add("来", "來");
        jianDic.Add("杨", "楊");
        jianDic.Add("杩", "榪");
        jianDic.Add("极", "極");
        jianDic.Add("构", "構");
        jianDic.Add("枞", "樅");
        jianDic.Add("枢", "樞");
        jianDic.Add("枣", "棗");
        jianDic.Add("枥", "櫪");
        jianDic.Add("枨", "棖");
        jianDic.Add("枪", "槍");
        jianDic.Add("枫", "楓");
        jianDic.Add("枭", "梟");
        jianDic.Add("柠", "檸");
        jianDic.Add("柽", "檉");
        jianDic.Add("栀", "梔");
        jianDic.Add("栅", "柵");
        jianDic.Add("标", "標");
        jianDic.Add("栈", "棧");
        jianDic.Add("栉", "櫛");
        jianDic.Add("栊", "櫳");
        jianDic.Add("栋", "棟");
        jianDic.Add("栌", "櫨");
        jianDic.Add("栎", "櫟");
        jianDic.Add("栏", "欄");
        jianDic.Add("树", "樹");
        jianDic.Add("栖", "棲");
        jianDic.Add("样", "樣");
        jianDic.Add("栾", "欒");
        jianDic.Add("桠", "椏");
        jianDic.Add("桡", "橈");
        jianDic.Add("桢", "楨");
        jianDic.Add("档", "檔");
        jianDic.Add("桤", "榿");
        jianDic.Add("桥", "橋");
        jianDic.Add("桦", "樺");
        jianDic.Add("桧", "檜");
        jianDic.Add("桨", "槳");
        jianDic.Add("桩", "樁");
        jianDic.Add("梦", "夢");
        jianDic.Add("检", "檢");
        jianDic.Add("棂", "欞");
        jianDic.Add("椁", "槨");
        jianDic.Add("椟", "櫝");
        jianDic.Add("椠", "槧");
        jianDic.Add("椤", "欏");
        jianDic.Add("椭", "橢");
        jianDic.Add("楼", "樓");
        jianDic.Add("榄", "欖");
        jianDic.Add("榇", "櫬");
        jianDic.Add("榈", "櫚");
        jianDic.Add("榉", "櫸");
        jianDic.Add("槛", "檻");
        jianDic.Add("槟", "檳");
        jianDic.Add("槠", "櫧");
        jianDic.Add("横", "橫");
        jianDic.Add("樯", "檣");
        jianDic.Add("樱", "櫻");
        jianDic.Add("橱", "櫥");
        jianDic.Add("橹", "櫓");
        jianDic.Add("橼", "櫞");
        jianDic.Add("檩", "檁");
        jianDic.Add("欢", "歡");
        jianDic.Add("欤", "歟");
        jianDic.Add("欧", "歐");
        jianDic.Add("歼", "殲");
        jianDic.Add("殁", "歿");
        jianDic.Add("殇", "殤");
        jianDic.Add("残", "殘");
        jianDic.Add("殒", "殞");
        jianDic.Add("殓", "殮");
        jianDic.Add("殚", "殫");
        jianDic.Add("殡", "殯");
        jianDic.Add("殴", "毆");
        jianDic.Add("毁", "毀");
        jianDic.Add("毂", "轂");
        jianDic.Add("毕", "畢");
        jianDic.Add("毙", "斃");
        jianDic.Add("毡", "氈");
        jianDic.Add("毵", "毿");
        jianDic.Add("氇", "氌");
        jianDic.Add("气", "氣");
        jianDic.Add("氢", "氫");
        jianDic.Add("氩", "氬");
        jianDic.Add("氲", "氳");
        jianDic.Add("汇", "匯");
        jianDic.Add("汉", "漢");
        jianDic.Add("汤", "湯");
        jianDic.Add("汹", "洶");
        jianDic.Add("沟", "溝");
        jianDic.Add("没", "沒");
        jianDic.Add("沣", "灃");
        jianDic.Add("沤", "漚");
        jianDic.Add("沥", "瀝");
        jianDic.Add("沦", "淪");
        jianDic.Add("沧", "滄");
        jianDic.Add("沩", "溈");
        jianDic.Add("沪", "滬");
        jianDic.Add("泞", "濘");
        jianDic.Add("泪", "淚");
        jianDic.Add("泶", "澩");
        jianDic.Add("泷", "瀧");
        jianDic.Add("泸", "瀘");
        jianDic.Add("泺", "濼");
        jianDic.Add("泻", "瀉");
        jianDic.Add("泼", "潑");
        jianDic.Add("泽", "澤");
        jianDic.Add("泾", "涇");
        jianDic.Add("洁", "潔");
        jianDic.Add("洒", "灑");
        jianDic.Add("浃", "浹");
        jianDic.Add("浅", "淺");
        jianDic.Add("浆", "漿");
        jianDic.Add("浇", "澆");
        jianDic.Add("浈", "湞");
        jianDic.Add("浊", "濁");
        jianDic.Add("测", "測");
        jianDic.Add("浍", "澮");
        jianDic.Add("济", "濟");
        jianDic.Add("浏", "瀏");
        jianDic.Add("浑", "渾");
        jianDic.Add("浒", "滸");
        jianDic.Add("浓", "濃");
        jianDic.Add("浔", "潯");
        jianDic.Add("涛", "濤");
        jianDic.Add("涝", "澇");
        jianDic.Add("涞", "淶");
        jianDic.Add("涟", "漣");
        jianDic.Add("涠", "潿");
        jianDic.Add("涡", "渦");
        jianDic.Add("涣", "渙");
        jianDic.Add("涤", "滌");
        jianDic.Add("润", "潤");
        jianDic.Add("涧", "澗");
        jianDic.Add("涨", "漲");
        jianDic.Add("涩", "澀");
        jianDic.Add("渊", "淵");
        jianDic.Add("渌", "淥");
        jianDic.Add("渍", "漬");
        jianDic.Add("渎", "瀆");
        jianDic.Add("渐", "漸");
        jianDic.Add("渑", "澠");
        jianDic.Add("渔", "漁");
        jianDic.Add("渖", "瀋");
        jianDic.Add("渗", "滲");
        jianDic.Add("温", "溫");
        jianDic.Add("湾", "灣");
        jianDic.Add("湿", "濕");
        jianDic.Add("溃", "潰");
        jianDic.Add("溅", "濺");
        jianDic.Add("滗", "潷");
        jianDic.Add("滚", "滾");
        jianDic.Add("滞", "滯");
        jianDic.Add("滠", "灄");
        jianDic.Add("满", "滿");
        jianDic.Add("滢", "瀅");
        jianDic.Add("滤", "濾");
        jianDic.Add("滥", "濫");
        jianDic.Add("滦", "灤");
        jianDic.Add("滨", "濱");
        jianDic.Add("滩", "灘");
        jianDic.Add("潆", "瀠");
        jianDic.Add("潇", "瀟");
        jianDic.Add("潋", "瀲");
        jianDic.Add("潍", "濰");
        jianDic.Add("潜", "潛");
        jianDic.Add("澜", "瀾");
        jianDic.Add("濑", "瀨");
        jianDic.Add("濒", "瀕");
        jianDic.Add("灏", "灝");
        jianDic.Add("灭", "滅");
        jianDic.Add("灯", "燈");
        jianDic.Add("灵", "靈");
        jianDic.Add("灾", "災");
        jianDic.Add("灿", "燦");
        jianDic.Add("炀", "煬");
        jianDic.Add("炉", "爐");
        jianDic.Add("炖", "燉");
        jianDic.Add("炜", "煒");
        jianDic.Add("炝", "熗");
        jianDic.Add("点", "點");
        jianDic.Add("炼", "煉");
        jianDic.Add("炽", "熾");
        jianDic.Add("烁", "爍");
        jianDic.Add("烂", "爛");
        jianDic.Add("烃", "烴");
        jianDic.Add("烛", "燭");
        jianDic.Add("烟", "煙");
        jianDic.Add("烦", "煩");
        jianDic.Add("烧", "燒");
        jianDic.Add("烨", "燁");
        jianDic.Add("烩", "燴");
        jianDic.Add("烫", "燙");
        jianDic.Add("烬", "燼");
        jianDic.Add("热", "熱");
        jianDic.Add("焕", "煥");
        jianDic.Add("焖", "燜");
        jianDic.Add("焘", "燾");
        jianDic.Add("爱", "愛");
        jianDic.Add("爷", "爺");
        jianDic.Add("牍", "牘");
        jianDic.Add("牵", "牽");
        jianDic.Add("牺", "犧");
        jianDic.Add("犊", "犢");
        jianDic.Add("状", "狀");
        jianDic.Add("犷", "獷");
        jianDic.Add("犹", "猶");
        jianDic.Add("狈", "狽");
        jianDic.Add("狞", "獰");
        jianDic.Add("独", "獨");
        jianDic.Add("狭", "狹");
        jianDic.Add("狮", "獅");
        jianDic.Add("狯", "獪");
        jianDic.Add("狰", "猙");
        jianDic.Add("狱", "獄");
        jianDic.Add("狲", "猻");
        jianDic.Add("狸", "貍");
        jianDic.Add("猃", "獫");
        jianDic.Add("猎", "獵");
        jianDic.Add("猕", "獼");
        jianDic.Add("猡", "玀");
        jianDic.Add("猪", "豬");
        jianDic.Add("猫", "貓");
        jianDic.Add("献", "獻");
        jianDic.Add("獭", "獺");
        jianDic.Add("玑", "璣");
        jianDic.Add("玛", "瑪");
        jianDic.Add("玮", "瑋");
        jianDic.Add("环", "環");
        jianDic.Add("现", "現");
        jianDic.Add("玺", "璽");
        jianDic.Add("珏", "玨");
        jianDic.Add("珐", "琺");
        jianDic.Add("珑", "瓏");
        jianDic.Add("珲", "琿");
        jianDic.Add("琅", "瑯");
        jianDic.Add("琏", "璉");
        jianDic.Add("琐", "瑣");
        jianDic.Add("琼", "瓊");
        jianDic.Add("瑶", "瑤");
        jianDic.Add("瑷", "璦");
        jianDic.Add("璎", "瓔");
        jianDic.Add("瓒", "瓚");
        jianDic.Add("瓮", "甕");
        jianDic.Add("瓯", "甌");
        jianDic.Add("电", "電");
        jianDic.Add("画", "畫");
        jianDic.Add("畅", "暢");
        jianDic.Add("畲", "畬");
        jianDic.Add("畴", "疇");
        jianDic.Add("疖", "癤");
        jianDic.Add("疗", "療");
        jianDic.Add("疟", "瘧");
        jianDic.Add("疠", "癘");
        jianDic.Add("疡", "瘍");
        jianDic.Add("疮", "瘡");
        jianDic.Add("疯", "瘋");
        jianDic.Add("疱", "皰");
        jianDic.Add("症", "癥");
        jianDic.Add("痈", "癰");
        jianDic.Add("痉", "痙");
        jianDic.Add("痒", "癢");
        jianDic.Add("痨", "癆");
        jianDic.Add("痪", "瘓");
        jianDic.Add("痫", "癇");
        jianDic.Add("痴", "癡");
        jianDic.Add("瘅", "癉");
        jianDic.Add("瘗", "瘞");
        jianDic.Add("瘘", "瘺");
        jianDic.Add("瘪", "癟");
        jianDic.Add("瘫", "癱");
        jianDic.Add("瘾", "癮");
        jianDic.Add("瘿", "癭");
        jianDic.Add("癞", "癩");
        jianDic.Add("癣", "癬");
        jianDic.Add("癫", "癲");
        jianDic.Add("皑", "皚");
        jianDic.Add("皱", "皺");
        jianDic.Add("皲", "皸");
        jianDic.Add("盏", "盞");
        jianDic.Add("盐", "鹽");
        jianDic.Add("监", "監");
        jianDic.Add("盖", "蓋");
        jianDic.Add("盗", "盜");
        jianDic.Add("盘", "盤");
        jianDic.Add("眦", "眥");
        jianDic.Add("眯", "瞇");
        jianDic.Add("着", "著");
        jianDic.Add("睁", "睜");
        jianDic.Add("睃", "脧");
        jianDic.Add("睐", "睞");
        jianDic.Add("睑", "瞼");
        jianDic.Add("睾", "睪");
        jianDic.Add("瞒", "瞞");
        jianDic.Add("瞩", "矚");
        jianDic.Add("矫", "矯");
        jianDic.Add("矶", "磯");
        jianDic.Add("矾", "礬");
        jianDic.Add("矿", "礦");
        jianDic.Add("砀", "碭");
        jianDic.Add("码", "碼");
        jianDic.Add("砖", "磚");
        jianDic.Add("砗", "硨");
        jianDic.Add("砚", "硯");
        jianDic.Add("砺", "礪");
        jianDic.Add("砻", "礱");
        jianDic.Add("砾", "礫");
        jianDic.Add("础", "礎");
        jianDic.Add("硕", "碩");
        jianDic.Add("硖", "硤");
        jianDic.Add("硗", "磽");
        jianDic.Add("确", "確");
        jianDic.Add("硷", "鹼");
        jianDic.Add("碍", "礙");
        jianDic.Add("碛", "磧");
        jianDic.Add("碜", "磣");
        jianDic.Add("碱", "堿");
        jianDic.Add("礼", "禮");
        jianDic.Add("祢", "禰");
        jianDic.Add("祯", "禎");
        jianDic.Add("祷", "禱");
        jianDic.Add("祸", "禍");
        jianDic.Add("禀", "稟");
        jianDic.Add("禄", "祿");
        jianDic.Add("禅", "禪");
        jianDic.Add("离", "離");
        jianDic.Add("秃", "禿");
        jianDic.Add("秆", "稈");
        jianDic.Add("种", "種");
        jianDic.Add("积", "積");
        jianDic.Add("称", "稱");
        jianDic.Add("秽", "穢");
        jianDic.Add("税", "稅");
        jianDic.Add("稣", "穌");
        jianDic.Add("稳", "穩");
        jianDic.Add("穑", "穡");
        jianDic.Add("穷", "窮");
        jianDic.Add("窃", "竊");
        jianDic.Add("窍", "竅");
        jianDic.Add("窑", "窯");
        jianDic.Add("窜", "竄");
        jianDic.Add("窝", "窩");
        jianDic.Add("窥", "窺");
        jianDic.Add("窦", "竇");
        jianDic.Add("窭", "窶");
        jianDic.Add("竖", "豎");
        jianDic.Add("竞", "競");
        jianDic.Add("笃", "篤");
        jianDic.Add("笋", "筍");
        jianDic.Add("笔", "筆");
        jianDic.Add("笕", "筧");
        jianDic.Add("笺", "箋");
        jianDic.Add("笼", "籠");
        jianDic.Add("笾", "籩");
        jianDic.Add("筚", "篳");
        jianDic.Add("筛", "篩");
        jianDic.Add("筝", "箏");
        jianDic.Add("筹", "籌");
        jianDic.Add("签", "簽");
        jianDic.Add("简", "簡");
        jianDic.Add("箦", "簀");
        jianDic.Add("箧", "篋");
        jianDic.Add("箨", "籜");
        jianDic.Add("箩", "籮");
        jianDic.Add("箪", "簞");
        jianDic.Add("箫", "簫");
        jianDic.Add("篑", "簣");
        jianDic.Add("篓", "簍");
        jianDic.Add("篮", "籃");
        jianDic.Add("篱", "籬");
        jianDic.Add("簖", "籪");
        jianDic.Add("籁", "籟");
        jianDic.Add("籴", "糴");
        jianDic.Add("类", "類");
        jianDic.Add("籼", "秈");
        jianDic.Add("粜", "糶");
        jianDic.Add("粝", "糲");
        jianDic.Add("粤", "粵");
        jianDic.Add("粪", "糞");
        jianDic.Add("粮", "糧");
        jianDic.Add("糁", "糝");
        jianDic.Add("紧", "緊");
        jianDic.Add("絷", "縶");
        jianDic.Add("纠", "糾");
        jianDic.Add("纡", "紆");
        jianDic.Add("红", "紅");
        jianDic.Add("纣", "紂");
        jianDic.Add("纤", "纖");
        jianDic.Add("纥", "紇");
        jianDic.Add("约", "約");
        jianDic.Add("级", "級");
        jianDic.Add("纨", "紈");
        jianDic.Add("纩", "纊");
        jianDic.Add("纪", "紀");
        jianDic.Add("纫", "紉");
        jianDic.Add("纬", "緯");
        jianDic.Add("纭", "紜");
        jianDic.Add("纯", "純");
        jianDic.Add("纰", "紕");
        jianDic.Add("纱", "紗");
        jianDic.Add("纲", "綱");
        jianDic.Add("纳", "納");
        jianDic.Add("纵", "縱");
        jianDic.Add("纶", "綸");
        jianDic.Add("纷", "紛");
        jianDic.Add("纸", "紙");
        jianDic.Add("纹", "紋");
        jianDic.Add("纺", "紡");
        jianDic.Add("纽", "紐");
        jianDic.Add("纾", "紓");
        jianDic.Add("线", "線");
        jianDic.Add("绀", "紺");
        jianDic.Add("绁", "紲");
        jianDic.Add("绂", "紱");
        jianDic.Add("练", "練");
        jianDic.Add("组", "組");
        jianDic.Add("绅", "紳");
        jianDic.Add("细", "細");
        jianDic.Add("织", "織");
        jianDic.Add("终", "終");
        jianDic.Add("绉", "縐");
        jianDic.Add("绊", "絆");
        jianDic.Add("绋", "紼");
        jianDic.Add("绌", "絀");
        jianDic.Add("绍", "紹");
        jianDic.Add("绎", "繹");
        jianDic.Add("经", "經");
        jianDic.Add("绐", "紿");
        jianDic.Add("绑", "綁");
        jianDic.Add("绒", "絨");
        jianDic.Add("结", "結");
        jianDic.Add("绕", "繞");
        jianDic.Add("绗", "絎");
        jianDic.Add("绘", "繪");
        jianDic.Add("给", "給");
        jianDic.Add("绚", "絢");
        jianDic.Add("绛", "絳");
        jianDic.Add("络", "絡");
        jianDic.Add("绝", "絕");
        jianDic.Add("绞", "絞");
        jianDic.Add("统", "統");
        jianDic.Add("绠", "綆");
        jianDic.Add("绡", "綃");
        jianDic.Add("绢", "絹");
        jianDic.Add("绣", "繡");
        jianDic.Add("绥", "綏");
        jianDic.Add("继", "繼");
        jianDic.Add("绨", "綈");
        jianDic.Add("绩", "績");
        jianDic.Add("绪", "緒");
        jianDic.Add("绫", "綾");
        jianDic.Add("续", "續");
        jianDic.Add("绮", "綺");
        jianDic.Add("绯", "緋");
        jianDic.Add("绰", "綽");
        jianDic.Add("绲", "緄");
        jianDic.Add("绳", "繩");
        jianDic.Add("维", "維");
        jianDic.Add("绵", "綿");
        jianDic.Add("绶", "綬");
        jianDic.Add("绷", "繃");
        jianDic.Add("绸", "綢");
        jianDic.Add("绺", "綹");
        jianDic.Add("绻", "綣");
        jianDic.Add("综", "綜");
        jianDic.Add("绽", "綻");
        jianDic.Add("绾", "綰");
        jianDic.Add("绿", "綠");
        jianDic.Add("缀", "綴");
        jianDic.Add("缁", "緇");
        jianDic.Add("缂", "緙");
        jianDic.Add("缃", "緗");
        jianDic.Add("缄", "緘");
        jianDic.Add("缅", "緬");
        jianDic.Add("缆", "纜");
        jianDic.Add("缇", "緹");
        jianDic.Add("缈", "緲");
        jianDic.Add("缉", "緝");
        jianDic.Add("缋", "繢");
        jianDic.Add("缌", "緦");
        jianDic.Add("缍", "綞");
        jianDic.Add("缎", "緞");
        jianDic.Add("缏", "緶");
        jianDic.Add("缑", "緱");
        jianDic.Add("缒", "縋");
        jianDic.Add("缓", "緩");
        jianDic.Add("缔", "締");
        jianDic.Add("缕", "縷");
        jianDic.Add("编", "編");
        jianDic.Add("缗", "緡");
        jianDic.Add("缘", "緣");
        jianDic.Add("缙", "縉");
        jianDic.Add("缚", "縛");
        jianDic.Add("缛", "縟");
        jianDic.Add("缜", "縝");
        jianDic.Add("缝", "縫");
        jianDic.Add("缟", "縞");
        jianDic.Add("缠", "纏");
        jianDic.Add("缡", "縭");
        jianDic.Add("缢", "縊");
        jianDic.Add("缣", "縑");
        jianDic.Add("缤", "繽");
        jianDic.Add("缥", "縹");
        jianDic.Add("缦", "縵");
        jianDic.Add("缧", "縲");
        jianDic.Add("缨", "纓");
        jianDic.Add("缩", "縮");
        jianDic.Add("缪", "繆");
        jianDic.Add("缫", "繅");
        jianDic.Add("缬", "纈");
        jianDic.Add("缭", "繚");
        jianDic.Add("缮", "繕");
        jianDic.Add("缯", "繒");
        jianDic.Add("缰", "韁");
        jianDic.Add("缱", "繾");
        jianDic.Add("缲", "繰");
        jianDic.Add("缳", "繯");
        jianDic.Add("缴", "繳");
        jianDic.Add("缵", "纘");
        jianDic.Add("罂", "罌");
        jianDic.Add("网", "網");
        jianDic.Add("罗", "羅");
        jianDic.Add("罚", "罰");
        jianDic.Add("罢", "罷");
        jianDic.Add("罴", "羆");
        jianDic.Add("羁", "羈");
        jianDic.Add("羟", "羥");
        jianDic.Add("羡", "羨");
        jianDic.Add("翘", "翹");
        jianDic.Add("耧", "耬");
        jianDic.Add("耸", "聳");
        jianDic.Add("耻", "恥");
        jianDic.Add("聂", "聶");
        jianDic.Add("聋", "聾");
        jianDic.Add("职", "職");
        jianDic.Add("聍", "聹");
        jianDic.Add("联", "聯");
        jianDic.Add("聩", "聵");
        jianDic.Add("聪", "聰");
        jianDic.Add("肃", "肅");
        jianDic.Add("肠", "腸");
        jianDic.Add("肤", "膚");
        jianDic.Add("肮", "骯");
        jianDic.Add("肾", "腎");
        jianDic.Add("肿", "腫");
        jianDic.Add("胀", "脹");
        jianDic.Add("胁", "脅");
        jianDic.Add("胆", "膽");
        jianDic.Add("胜", "勝");
        jianDic.Add("胧", "朧");
        jianDic.Add("胪", "臚");
        jianDic.Add("胫", "脛");
        jianDic.Add("胶", "膠");
        jianDic.Add("脉", "脈");
        jianDic.Add("脍", "膾");
        jianDic.Add("脏", "臟");
        jianDic.Add("脐", "臍");
        jianDic.Add("脑", "腦");
        jianDic.Add("脓", "膿");
        jianDic.Add("脔", "臠");
        jianDic.Add("脚", "腳");
        jianDic.Add("脱", "脫");
        jianDic.Add("脶", "腡");
        jianDic.Add("脸", "臉");
        jianDic.Add("腊", "臘");
        jianDic.Add("腻", "膩");
        jianDic.Add("腼", "靦");
        jianDic.Add("腽", "膃");
        jianDic.Add("腾", "騰");
        jianDic.Add("膑", "臏");
        jianDic.Add("舆", "輿");
        jianDic.Add("舣", "艤");
        jianDic.Add("舰", "艦");
        jianDic.Add("舱", "艙");
        jianDic.Add("舻", "艫");
        jianDic.Add("艰", "艱");
        jianDic.Add("艳", "艷");
        jianDic.Add("艹", "艸");
        jianDic.Add("艺", "藝");
        jianDic.Add("节", "節");
        jianDic.Add("芈", "羋");
        jianDic.Add("芗", "薌");
        jianDic.Add("芜", "蕪");
        jianDic.Add("芦", "蘆");
        jianDic.Add("芸", "蕓");
        jianDic.Add("苁", "蓯");
        jianDic.Add("苄", "芐");
        jianDic.Add("苇", "葦");
        jianDic.Add("苈", "藶");
        jianDic.Add("苋", "莧");
        jianDic.Add("苌", "萇");
        jianDic.Add("苍", "蒼");
        jianDic.Add("苎", "苧");
        jianDic.Add("苏", "蘇");
        jianDic.Add("苟", "茍");
        jianDic.Add("苹", "蘋");
        jianDic.Add("茎", "莖");
        jianDic.Add("茏", "蘢");
        jianDic.Add("茑", "蔦");
        jianDic.Add("茔", "塋");
        jianDic.Add("茕", "煢");
        jianDic.Add("茧", "繭");
        jianDic.Add("荆", "荊");
        jianDic.Add("荐", "薦");
        jianDic.Add("荚", "莢");
        jianDic.Add("荛", "蕘");
        jianDic.Add("荜", "蓽");
        jianDic.Add("荞", "蕎");
        jianDic.Add("荟", "薈");
        jianDic.Add("荠", "薺");
        jianDic.Add("荡", "蕩");
        jianDic.Add("荣", "榮");
        jianDic.Add("荤", "葷");
        jianDic.Add("荥", "滎");
        jianDic.Add("荦", "犖");
        jianDic.Add("荧", "熒");
        jianDic.Add("荨", "蕁");
        jianDic.Add("荩", "藎");
        jianDic.Add("荪", "蓀");
        jianDic.Add("荫", "蔭");
        jianDic.Add("荭", "葒");
        jianDic.Add("药", "藥");
        jianDic.Add("莅", "蒞");
        jianDic.Add("莱", "萊");
        jianDic.Add("莲", "蓮");
        jianDic.Add("莳", "蒔");
        jianDic.Add("莴", "萵");
        jianDic.Add("莶", "薟");
        jianDic.Add("获", "獲");
        jianDic.Add("莸", "蕕");
        jianDic.Add("莹", "瑩");
        jianDic.Add("莺", "鶯");
        jianDic.Add("萝", "蘿");
        jianDic.Add("萤", "螢");
        jianDic.Add("营", "營");
        jianDic.Add("萦", "縈");
        jianDic.Add("萧", "蕭");
        jianDic.Add("萨", "薩");
        jianDic.Add("葱", "蔥");
        jianDic.Add("蒇", "蕆");
        jianDic.Add("蒉", "蕢");
        jianDic.Add("蒋", "蔣");
        jianDic.Add("蒌", "蔞");
        jianDic.Add("蓝", "藍");
        jianDic.Add("蓟", "薊");
        jianDic.Add("蓠", "蘺");
        jianDic.Add("蓣", "蕷");
        jianDic.Add("蓥", "鎣");
        jianDic.Add("蓦", "驀");
        jianDic.Add("蔷", "薔");
        jianDic.Add("蔹", "蘞");
        jianDic.Add("蔺", "藺");
        jianDic.Add("蔼", "藹");
        jianDic.Add("蕲", "蘄");
        jianDic.Add("蕴", "蘊");
        jianDic.Add("薮", "藪");
        jianDic.Add("藓", "蘚");
        jianDic.Add("蘖", "蘗");
        jianDic.Add("虏", "虜");
        jianDic.Add("虑", "慮");
        jianDic.Add("虚", "虛");
        jianDic.Add("虫", "蟲");
        jianDic.Add("虮", "蟣");
        jianDic.Add("虽", "雖");
        jianDic.Add("虾", "蝦");
        jianDic.Add("虿", "蠆");
        jianDic.Add("蚀", "蝕");
        jianDic.Add("蚁", "蟻");
        jianDic.Add("蚂", "螞");
        jianDic.Add("蚕", "蠶");
        jianDic.Add("蚝", "蠔");
        jianDic.Add("蚬", "蜆");
        jianDic.Add("蛊", "蠱");
        jianDic.Add("蛎", "蠣");
        jianDic.Add("蛏", "蟶");
        jianDic.Add("蛮", "蠻");
        jianDic.Add("蛰", "蟄");
        jianDic.Add("蛱", "蛺");
        jianDic.Add("蛲", "蟯");
        jianDic.Add("蛳", "螄");
        jianDic.Add("蛴", "蠐");
        jianDic.Add("蜕", "蛻");
        jianDic.Add("蜗", "蝸");
        jianDic.Add("蜡", "蠟");
        jianDic.Add("蝇", "蠅");
        jianDic.Add("蝈", "蟈");
        jianDic.Add("蝉", "蟬");
        jianDic.Add("蝼", "螻");
        jianDic.Add("蝾", "蠑");
        jianDic.Add("衅", "釁");
        jianDic.Add("衔", "銜");
        jianDic.Add("补", "補");
        jianDic.Add("衬", "襯");
        jianDic.Add("衮", "袞");
        jianDic.Add("袄", "襖");
        jianDic.Add("袅", "裊");
        jianDic.Add("袜", "襪");
        jianDic.Add("袭", "襲");
        jianDic.Add("装", "裝");
        jianDic.Add("裆", "襠");
        jianDic.Add("裢", "褳");
        jianDic.Add("裣", "襝");
        jianDic.Add("裤", "褲");
        jianDic.Add("褛", "褸");
        jianDic.Add("褴", "襤");
        jianDic.Add("见", "見");
        jianDic.Add("观", "觀");
        jianDic.Add("规", "規");
        jianDic.Add("觅", "覓");
        jianDic.Add("视", "視");
        jianDic.Add("觇", "覘");
        jianDic.Add("览", "覽");
        jianDic.Add("觉", "覺");
        jianDic.Add("觊", "覬");
        jianDic.Add("觋", "覡");
        jianDic.Add("觌", "覿");
        jianDic.Add("觎", "覦");
        jianDic.Add("觏", "覯");
        jianDic.Add("觐", "覲");
        jianDic.Add("觑", "覷");
        jianDic.Add("觞", "觴");
        jianDic.Add("触", "觸");
        jianDic.Add("觯", "觶");
        jianDic.Add("誉", "譽");
        jianDic.Add("誊", "謄");
        jianDic.Add("计", "計");
        jianDic.Add("订", "訂");
        jianDic.Add("讣", "訃");
        jianDic.Add("认", "認");
        jianDic.Add("讥", "譏");
        jianDic.Add("讦", "訐");
        jianDic.Add("讧", "訌");
        jianDic.Add("讨", "討");
        jianDic.Add("让", "讓");
        jianDic.Add("讪", "訕");
        jianDic.Add("讫", "訖");
        jianDic.Add("训", "訓");
        jianDic.Add("议", "議");
        jianDic.Add("讯", "訊");
        jianDic.Add("记", "記");
        jianDic.Add("讲", "講");
        jianDic.Add("讳", "諱");
        jianDic.Add("讴", "謳");
        jianDic.Add("讵", "詎");
        jianDic.Add("讶", "訝");
        jianDic.Add("讷", "訥");
        jianDic.Add("许", "許");
        jianDic.Add("讹", "訛");
        jianDic.Add("论", "論");
        jianDic.Add("讼", "訟");
        jianDic.Add("讽", "諷");
        jianDic.Add("设", "設");
        jianDic.Add("访", "訪");
        jianDic.Add("诀", "訣");
        jianDic.Add("证", "證");
        jianDic.Add("诂", "詁");
        jianDic.Add("诃", "訶");
        jianDic.Add("评", "評");
        jianDic.Add("诅", "詛");
        jianDic.Add("识", "識");
        jianDic.Add("诈", "詐");
        jianDic.Add("诉", "訴");
        jianDic.Add("诊", "診");
        jianDic.Add("诋", "詆");
        jianDic.Add("诌", "謅");
        jianDic.Add("词", "詞");
        jianDic.Add("诎", "詘");
        jianDic.Add("诏", "詔");
        jianDic.Add("译", "譯");
        jianDic.Add("诒", "詒");
        jianDic.Add("诓", "誆");
        jianDic.Add("诔", "誄");
        jianDic.Add("试", "試");
        jianDic.Add("诖", "詿");
        jianDic.Add("诗", "詩");
        jianDic.Add("诘", "詰");
        jianDic.Add("诙", "詼");
        jianDic.Add("诚", "誠");
        jianDic.Add("诛", "誅");
        jianDic.Add("诜", "詵");
        jianDic.Add("话", "話");
        jianDic.Add("诞", "誕");
        jianDic.Add("诟", "詬");
        jianDic.Add("诠", "詮");
        jianDic.Add("诡", "詭");
        jianDic.Add("询", "詢");
        jianDic.Add("诣", "詣");
        jianDic.Add("诤", "諍");
        jianDic.Add("该", "該");
        jianDic.Add("详", "詳");
        jianDic.Add("诧", "詫");
        jianDic.Add("诨", "諢");
        jianDic.Add("诩", "詡");
        jianDic.Add("诫", "誡");
        jianDic.Add("诬", "誣");
        jianDic.Add("语", "語");
        jianDic.Add("诮", "誚");
        jianDic.Add("误", "誤");
        jianDic.Add("诰", "誥");
        jianDic.Add("诱", "誘");
        jianDic.Add("诲", "誨");
        jianDic.Add("诳", "誑");
        jianDic.Add("说", "說");
        jianDic.Add("诵", "誦");
        jianDic.Add("诶", "誒");
        jianDic.Add("请", "請");
        jianDic.Add("诸", "諸");
        jianDic.Add("诹", "諏");
        jianDic.Add("诺", "諾");
        jianDic.Add("读", "讀");
        jianDic.Add("诼", "諑");
        jianDic.Add("诽", "誹");
        jianDic.Add("课", "課");
        jianDic.Add("诿", "諉");
        jianDic.Add("谀", "諛");
        jianDic.Add("谁", "誰");
        jianDic.Add("谂", "諗");
        jianDic.Add("调", "調");
        jianDic.Add("谄", "諂");
        jianDic.Add("谅", "諒");
        jianDic.Add("谆", "諄");
        jianDic.Add("谇", "誶");
        jianDic.Add("谈", "談");
        jianDic.Add("谊", "誼");
        jianDic.Add("谋", "謀");
        jianDic.Add("谌", "諶");
        jianDic.Add("谍", "諜");
        jianDic.Add("谎", "謊");
        jianDic.Add("谏", "諫");
        jianDic.Add("谐", "諧");
        jianDic.Add("谑", "謔");
        jianDic.Add("谒", "謁");
        jianDic.Add("谓", "謂");
        jianDic.Add("谔", "諤");
        jianDic.Add("谕", "諭");
        jianDic.Add("谖", "諼");
        jianDic.Add("谗", "讒");
        jianDic.Add("谘", "諮");
        jianDic.Add("谙", "諳");
        jianDic.Add("谚", "諺");
        jianDic.Add("谛", "諦");
        jianDic.Add("谜", "謎");
        jianDic.Add("谝", "諞");
        jianDic.Add("谟", "謨");
        jianDic.Add("谠", "讜");
        jianDic.Add("谡", "謖");
        jianDic.Add("谢", "謝");
        jianDic.Add("谣", "謠");
        jianDic.Add("谤", "謗");
        jianDic.Add("谥", "謚");
        jianDic.Add("谦", "謙");
        jianDic.Add("谧", "謐");
        jianDic.Add("谨", "謹");
        jianDic.Add("谩", "謾");
        jianDic.Add("谪", "謫");
        jianDic.Add("谬", "謬");
        jianDic.Add("谭", "譚");
        jianDic.Add("谮", "譖");
        jianDic.Add("谯", "譙");
        jianDic.Add("谰", "讕");
        jianDic.Add("谱", "譜");
        jianDic.Add("谲", "譎");
        jianDic.Add("谳", "讞");
        jianDic.Add("谴", "譴");
        jianDic.Add("谵", "譫");
        jianDic.Add("谶", "讖");
        jianDic.Add("贝", "貝");
        jianDic.Add("贞", "貞");
        jianDic.Add("负", "負");
        jianDic.Add("贡", "貢");
        jianDic.Add("财", "財");
        jianDic.Add("责", "責");
        jianDic.Add("贤", "賢");
        jianDic.Add("败", "敗");
        jianDic.Add("账", "賬");
        jianDic.Add("货", "貨");
        jianDic.Add("质", "質");
        jianDic.Add("贩", "販");
        jianDic.Add("贪", "貪");
        jianDic.Add("贫", "貧");
        jianDic.Add("贬", "貶");
        jianDic.Add("购", "購");
        jianDic.Add("贮", "貯");
        jianDic.Add("贯", "貫");
        jianDic.Add("贰", "貳");
        jianDic.Add("贱", "賤");
        jianDic.Add("贲", "賁");
        jianDic.Add("贳", "貰");
        jianDic.Add("贴", "貼");
        jianDic.Add("贵", "貴");
        jianDic.Add("贶", "貺");
        jianDic.Add("贷", "貸");
        jianDic.Add("贸", "貿");
        jianDic.Add("费", "費");
        jianDic.Add("贺", "賀");
        jianDic.Add("贻", "貽");
        jianDic.Add("贼", "賊");
        jianDic.Add("贽", "贄");
        jianDic.Add("贾", "賈");
        jianDic.Add("贿", "賄");
        jianDic.Add("赀", "貲");
        jianDic.Add("赁", "賃");
        jianDic.Add("赂", "賂");
        jianDic.Add("赃", "贓");
        jianDic.Add("资", "資");
        jianDic.Add("赅", "賅");
        jianDic.Add("赆", "贐");
        jianDic.Add("赇", "賕");
        jianDic.Add("赈", "賑");
        jianDic.Add("赉", "賚");
        jianDic.Add("赊", "賒");
        jianDic.Add("赋", "賦");
        jianDic.Add("赌", "賭");
        jianDic.Add("赎", "贖");
        jianDic.Add("赏", "賞");
        jianDic.Add("赐", "賜");
        jianDic.Add("赓", "賡");
        jianDic.Add("赔", "賠");
        jianDic.Add("赕", "賧");
        jianDic.Add("赖", "賴");
        jianDic.Add("赘", "贅");
        jianDic.Add("赙", "賻");
        jianDic.Add("赚", "賺");
        jianDic.Add("赛", "賽");
        jianDic.Add("赜", "賾");
        jianDic.Add("赝", "贗");
        jianDic.Add("赞", "贊");
        jianDic.Add("赠", "贈");
        jianDic.Add("赡", "贍");
        jianDic.Add("赢", "贏");
        jianDic.Add("赣", "贛");
        jianDic.Add("赵", "趙");
        jianDic.Add("赶", "趕");
        jianDic.Add("趋", "趨");
        jianDic.Add("趱", "趲");
        jianDic.Add("趸", "躉");
        jianDic.Add("跃", "躍");
        jianDic.Add("跄", "蹌");
        jianDic.Add("跞", "躒");
        jianDic.Add("践", "踐");
        jianDic.Add("跷", "蹺");
        jianDic.Add("跸", "蹕");
        jianDic.Add("跹", "躚");
        jianDic.Add("跻", "躋");
        jianDic.Add("踊", "踴");
        jianDic.Add("踌", "躊");
        jianDic.Add("踪", "蹤");
        jianDic.Add("踬", "躓");
        jianDic.Add("踯", "躑");
        jianDic.Add("蹑", "躡");
        jianDic.Add("蹒", "蹣");
        jianDic.Add("蹰", "躕");
        jianDic.Add("蹿", "躥");
        jianDic.Add("躏", "躪");
        jianDic.Add("躜", "躦");
        jianDic.Add("躯", "軀");
        jianDic.Add("车", "車");
        jianDic.Add("轧", "軋");
        jianDic.Add("轨", "軌");
        jianDic.Add("轩", "軒");
        jianDic.Add("轫", "軔");
        jianDic.Add("转", "轉");
        jianDic.Add("轭", "軛");
        jianDic.Add("轮", "輪");
        jianDic.Add("软", "軟");
        jianDic.Add("轰", "轟");
        jianDic.Add("轲", "軻");
        jianDic.Add("轳", "轤");
        jianDic.Add("轴", "軸");
        jianDic.Add("轵", "軹");
        jianDic.Add("轶", "軼");
        jianDic.Add("轸", "軫");
        jianDic.Add("轹", "轢");
        jianDic.Add("轺", "軺");
        jianDic.Add("轻", "輕");
        jianDic.Add("轼", "軾");
        jianDic.Add("载", "載");
        jianDic.Add("轾", "輊");
        jianDic.Add("轿", "轎");
        jianDic.Add("辁", "輇");
        jianDic.Add("辂", "輅");
        jianDic.Add("较", "較");
        jianDic.Add("辄", "輒");
        jianDic.Add("辅", "輔");
        jianDic.Add("辆", "輛");
        jianDic.Add("辇", "輦");
        jianDic.Add("辈", "輩");
        jianDic.Add("辉", "輝");
        jianDic.Add("辊", "輥");
        jianDic.Add("辋", "輞");
        jianDic.Add("辍", "輟");
        jianDic.Add("辎", "輜");
        jianDic.Add("辏", "輳");
        jianDic.Add("辐", "輻");
        jianDic.Add("辑", "輯");
        jianDic.Add("输", "輸");
        jianDic.Add("辔", "轡");
        jianDic.Add("辕", "轅");
        jianDic.Add("辖", "轄");
        jianDic.Add("辗", "輾");
        jianDic.Add("辘", "轆");
        jianDic.Add("辙", "轍");
        jianDic.Add("辚", "轔");
        jianDic.Add("辞", "辭");
        jianDic.Add("辩", "辯");
        jianDic.Add("辫", "辮");
        jianDic.Add("边", "邊");
        jianDic.Add("辽", "遼");
        jianDic.Add("达", "達");
        jianDic.Add("迁", "遷");
        jianDic.Add("过", "過");
        jianDic.Add("迈", "邁");
        jianDic.Add("运", "運");
        jianDic.Add("还", "還");
        jianDic.Add("这", "這");
        jianDic.Add("进", "進");
        jianDic.Add("远", "遠");
        jianDic.Add("违", "違");
        jianDic.Add("连", "連");
        jianDic.Add("迟", "遲");
        jianDic.Add("迩", "邇");
        jianDic.Add("迳", "逕");
        jianDic.Add("迹", "跡");
        jianDic.Add("适", "適");
        jianDic.Add("选", "選");
        jianDic.Add("逊", "遜");
        jianDic.Add("递", "遞");
        jianDic.Add("逦", "邐");
        jianDic.Add("逻", "邏");
        jianDic.Add("遗", "遺");
        jianDic.Add("遥", "遙");
        jianDic.Add("邓", "鄧");
        jianDic.Add("邝", "鄺");
        jianDic.Add("邬", "鄔");
        jianDic.Add("邮", "郵");
        jianDic.Add("邹", "鄒");
        jianDic.Add("邺", "鄴");
        jianDic.Add("邻", "鄰");
        jianDic.Add("郏", "郟");
        jianDic.Add("郐", "鄶");
        jianDic.Add("郑", "鄭");
        jianDic.Add("郓", "鄆");
        jianDic.Add("郦", "酈");
        jianDic.Add("郧", "鄖");
        jianDic.Add("郸", "鄲");
        jianDic.Add("酝", "醞");
        jianDic.Add("酱", "醬");
        jianDic.Add("酽", "釅");
        jianDic.Add("酾", "釃");
        jianDic.Add("酿", "釀");
        jianDic.Add("释", "釋");
        jianDic.Add("鉴", "鑒");
        jianDic.Add("銮", "鑾");
        jianDic.Add("錾", "鏨");
        jianDic.Add("钆", "釓");
        jianDic.Add("钇", "釔");
        jianDic.Add("针", "針");
        jianDic.Add("钉", "釘");
        jianDic.Add("钊", "釗");
        jianDic.Add("钋", "釙");
        jianDic.Add("钌", "釕");
        jianDic.Add("钍", "釷");
        jianDic.Add("钎", "釬");
        jianDic.Add("钏", "釧");
        jianDic.Add("钐", "釤");
        jianDic.Add("钒", "釩");
        jianDic.Add("钓", "釣");
        jianDic.Add("钔", "鍆");
        jianDic.Add("钕", "釹");
        jianDic.Add("钗", "釵");
        jianDic.Add("钙", "鈣");
        jianDic.Add("钛", "鈦");
        jianDic.Add("钜", "鉅");
        jianDic.Add("钝", "鈍");
        jianDic.Add("钞", "鈔");
        jianDic.Add("钟", "鐘");
        jianDic.Add("钠", "鈉");
        jianDic.Add("钡", "鋇");
        jianDic.Add("钢", "鋼");
        jianDic.Add("钣", "鈑");
        jianDic.Add("钤", "鈐");
        jianDic.Add("钥", "鑰");
        jianDic.Add("钦", "欽");
        jianDic.Add("钧", "鈞");
        jianDic.Add("钨", "鎢");
        jianDic.Add("钩", "鉤");
        jianDic.Add("钪", "鈧");
        jianDic.Add("钫", "鈁");
        jianDic.Add("钬", "鈥");
        jianDic.Add("钭", "鈄");
        jianDic.Add("钮", "鈕");
        jianDic.Add("钯", "鈀");
        jianDic.Add("钰", "鈺");
        jianDic.Add("钱", "錢");
        jianDic.Add("钲", "鉦");
        jianDic.Add("钳", "鉗");
        jianDic.Add("钴", "鈷");
        jianDic.Add("钵", "缽");
        jianDic.Add("钶", "鈳");
        jianDic.Add("钸", "鈽");
        jianDic.Add("钹", "鈸");
        jianDic.Add("钺", "鉞");
        jianDic.Add("钻", "鉆");
        jianDic.Add("钼", "鉬");
        jianDic.Add("钽", "鉭");
        jianDic.Add("钾", "鉀");
        jianDic.Add("钿", "鈿");
        jianDic.Add("铀", "鈾");
        jianDic.Add("铁", "鐵");
        jianDic.Add("铂", "鉑");
        jianDic.Add("铃", "鈴");
        jianDic.Add("铄", "鑠");
        jianDic.Add("铅", "鉛");
        jianDic.Add("铆", "鉚");
        jianDic.Add("铈", "鈰");
        jianDic.Add("铉", "鉉");
        jianDic.Add("铊", "鉈");
        jianDic.Add("铋", "鉍");
        jianDic.Add("铌", "鈮");
        jianDic.Add("铍", "鈹");
        jianDic.Add("铎", "鐸");
        jianDic.Add("铐", "銬");
        jianDic.Add("铑", "銠");
        jianDic.Add("铒", "鉺");
        jianDic.Add("铕", "銪");
        jianDic.Add("铖", "鋮");
        jianDic.Add("铗", "鋏");
        jianDic.Add("铙", "鐃");
        jianDic.Add("铛", "鐺");
        jianDic.Add("铜", "銅");
        jianDic.Add("铝", "鋁");
        jianDic.Add("铟", "銦");
        jianDic.Add("铠", "鎧");
        jianDic.Add("铡", "鍘");
        jianDic.Add("铢", "銖");
        jianDic.Add("铣", "銑");
        jianDic.Add("铤", "鋌");
        jianDic.Add("铥", "銩");
        jianDic.Add("铧", "鏵");
        jianDic.Add("铨", "銓");
        jianDic.Add("铩", "鎩");
        jianDic.Add("铪", "鉿");
        jianDic.Add("铫", "銚");
        jianDic.Add("铬", "鉻");
        jianDic.Add("铭", "銘");
        jianDic.Add("铮", "錚");
        jianDic.Add("铯", "銫");
        jianDic.Add("铰", "鉸");
        jianDic.Add("铱", "銥");
        jianDic.Add("铲", "鏟");
        jianDic.Add("铳", "銃");
        jianDic.Add("铴", "鐋");
        jianDic.Add("铵", "銨");
        jianDic.Add("银", "銀");
        jianDic.Add("铷", "銣");
        jianDic.Add("铸", "鑄");
        jianDic.Add("铹", "鐒");
        jianDic.Add("铺", "鋪");
        jianDic.Add("铼", "錸");
        jianDic.Add("铽", "鋱");
        jianDic.Add("链", "鏈");
        jianDic.Add("铿", "鏗");
        jianDic.Add("销", "銷");
        jianDic.Add("锁", "鎖");
        jianDic.Add("锂", "鋰");
        jianDic.Add("锄", "鋤");
        jianDic.Add("锅", "鍋");
        jianDic.Add("锆", "鋯");
        jianDic.Add("锇", "鋨");
        jianDic.Add("锈", "銹");
        jianDic.Add("锉", "銼");
        jianDic.Add("锊", "鋝");
        jianDic.Add("锋", "鋒");
        jianDic.Add("锌", "鋅");
        jianDic.Add("锐", "銳");
        jianDic.Add("锑", "銻");
        jianDic.Add("锒", "鋃");
        jianDic.Add("锓", "鋟");
        jianDic.Add("锔", "鋦");
        jianDic.Add("锕", "錒");
        jianDic.Add("锖", "錆");
        jianDic.Add("锗", "鍺");
        jianDic.Add("错", "錯");
        jianDic.Add("锚", "錨");
        jianDic.Add("锛", "錛");
        jianDic.Add("锞", "錁");
        jianDic.Add("锟", "錕");
        jianDic.Add("锡", "錫");
        jianDic.Add("锢", "錮");
        jianDic.Add("锣", "鑼");
        jianDic.Add("锤", "錘");
        jianDic.Add("锥", "錐");
        jianDic.Add("锦", "錦");
        jianDic.Add("锩", "錈");
        jianDic.Add("锬", "錟");
        jianDic.Add("锭", "錠");
        jianDic.Add("键", "鍵");
        jianDic.Add("锯", "鋸");
        jianDic.Add("锰", "錳");
        jianDic.Add("锱", "錙");
        jianDic.Add("锲", "鍥");
        jianDic.Add("锴", "鍇");
        jianDic.Add("锵", "鏘");
        jianDic.Add("锶", "鍶");
        jianDic.Add("锷", "鍔");
        jianDic.Add("锸", "鍤");
        jianDic.Add("锹", "鍬");
        jianDic.Add("锺", "鍾");
        jianDic.Add("锻", "鍛");
        jianDic.Add("锼", "鎪");
        jianDic.Add("锾", "鍰");
        jianDic.Add("镀", "鍍");
        jianDic.Add("镁", "鎂");
        jianDic.Add("镂", "鏤");
        jianDic.Add("镄", "鐨");
        jianDic.Add("镆", "鏌");
        jianDic.Add("镇", "鎮");
        jianDic.Add("镉", "鎘");
        jianDic.Add("镊", "鑷");
        jianDic.Add("镌", "鐫");
        jianDic.Add("镍", "鎳");
        jianDic.Add("镏", "鎦");
        jianDic.Add("镐", "鎬");
        jianDic.Add("镑", "鎊");
        jianDic.Add("镒", "鎰");
        jianDic.Add("镓", "鎵");
        jianDic.Add("镔", "鑌");
        jianDic.Add("镖", "鏢");
        jianDic.Add("镗", "鏜");
        jianDic.Add("镘", "鏝");
        jianDic.Add("镙", "鏍");
        jianDic.Add("镛", "鏞");
        jianDic.Add("镜", "鏡");
        jianDic.Add("镝", "鏑");
        jianDic.Add("镞", "鏃");
        jianDic.Add("镟", "鏇");
        jianDic.Add("镡", "鐔");
        jianDic.Add("镣", "鐐");
        jianDic.Add("镤", "鏷");
        jianDic.Add("镦", "鐓");
        jianDic.Add("镧", "鑭");
        jianDic.Add("镨", "鐠");
        jianDic.Add("镪", "鏹");
        jianDic.Add("镫", "鐙");
        jianDic.Add("镬", "鑊");
        jianDic.Add("镭", "鐳");
        jianDic.Add("镯", "鐲");
        jianDic.Add("镰", "鐮");
        jianDic.Add("镱", "鐿");
        jianDic.Add("镳", "鑣");
        jianDic.Add("镶", "鑲");
        jianDic.Add("长", "長");
        jianDic.Add("门", "門");
        jianDic.Add("闩", "閂");
        jianDic.Add("闪", "閃");
        jianDic.Add("闫", "閆");
        jianDic.Add("闭", "閉");
        jianDic.Add("问", "問");
        jianDic.Add("闯", "闖");
        jianDic.Add("闰", "閏");
        jianDic.Add("闱", "闈");
        jianDic.Add("闲", "閑");
        jianDic.Add("闳", "閎");
        jianDic.Add("间", "間");
        jianDic.Add("闵", "閔");
        jianDic.Add("闶", "閌");
        jianDic.Add("闷", "悶");
        jianDic.Add("闸", "閘");
        jianDic.Add("闹", "鬧");
        jianDic.Add("闺", "閨");
        jianDic.Add("闻", "聞");
        jianDic.Add("闼", "闥");
        jianDic.Add("闽", "閩");
        jianDic.Add("闾", "閭");
        jianDic.Add("阀", "閥");
        jianDic.Add("阁", "閣");
        jianDic.Add("阂", "閡");
        jianDic.Add("阃", "閫");
        jianDic.Add("阄", "鬮");
        jianDic.Add("阅", "閱");
        jianDic.Add("阆", "閬");
        jianDic.Add("阈", "閾");
        jianDic.Add("阉", "閹");
        jianDic.Add("阊", "閶");
        jianDic.Add("阋", "鬩");
        jianDic.Add("阌", "閿");
        jianDic.Add("阍", "閽");
        jianDic.Add("阎", "閻");
        jianDic.Add("阏", "閼");
        jianDic.Add("阐", "闡");
        jianDic.Add("阑", "闌");
        jianDic.Add("阒", "闃");
        jianDic.Add("阔", "闊");
        jianDic.Add("阕", "闋");
        jianDic.Add("阖", "闔");
        jianDic.Add("阗", "闐");
        jianDic.Add("阙", "闕");
        jianDic.Add("阚", "闞");
        jianDic.Add("队", "隊");
        jianDic.Add("阳", "陽");
        jianDic.Add("阴", "陰");
        jianDic.Add("阵", "陣");
        jianDic.Add("阶", "階");
        jianDic.Add("际", "際");
        jianDic.Add("陆", "陸");
        jianDic.Add("陇", "隴");
        jianDic.Add("陈", "陳");
        jianDic.Add("陉", "陘");
        jianDic.Add("陕", "陜");
        jianDic.Add("陧", "隉");
        jianDic.Add("陨", "隕");
        jianDic.Add("险", "險");
        jianDic.Add("随", "隨");
        jianDic.Add("隐", "隱");
        jianDic.Add("隶", "隸");
        jianDic.Add("隽", "雋");
        jianDic.Add("难", "難");
        jianDic.Add("雏", "雛");
        jianDic.Add("雠", "讎");
        jianDic.Add("雳", "靂");
        jianDic.Add("雾", "霧");
        jianDic.Add("霁", "霽");
        jianDic.Add("霭", "靄");
        jianDic.Add("靓", "靚");
        jianDic.Add("静", "靜");
        jianDic.Add("靥", "靨");
        jianDic.Add("鞑", "韃");
        jianDic.Add("鞯", "韉");
        jianDic.Add("韦", "韋");
        jianDic.Add("韧", "韌");
        jianDic.Add("韩", "韓");
        jianDic.Add("韪", "韙");
        jianDic.Add("韫", "韞");
        jianDic.Add("韬", "韜");
        jianDic.Add("韵", "韻");
        jianDic.Add("页", "頁");
        jianDic.Add("顶", "頂");
        jianDic.Add("顷", "頃");
        jianDic.Add("顸", "頇");
        jianDic.Add("项", "項");
        jianDic.Add("顺", "順");
        jianDic.Add("须", "須");
        jianDic.Add("顼", "頊");
        jianDic.Add("顽", "頑");
        jianDic.Add("顾", "顧");
        jianDic.Add("顿", "頓");
        jianDic.Add("颀", "頎");
        jianDic.Add("颁", "頒");
        jianDic.Add("颂", "頌");
        jianDic.Add("颃", "頏");
        jianDic.Add("预", "預");
        jianDic.Add("颅", "顱");
        jianDic.Add("领", "領");
        jianDic.Add("颇", "頗");
        jianDic.Add("颈", "頸");
        jianDic.Add("颉", "頡");
        jianDic.Add("颊", "頰");
        jianDic.Add("颌", "頜");
        jianDic.Add("颍", "潁");
        jianDic.Add("颏", "頦");
        jianDic.Add("颐", "頤");
        jianDic.Add("频", "頻");
        jianDic.Add("颓", "頹");
        jianDic.Add("颔", "頷");
        jianDic.Add("颖", "穎");
        jianDic.Add("颗", "顆");
        jianDic.Add("题", "題");
        jianDic.Add("颚", "顎");
        jianDic.Add("颛", "顓");
        jianDic.Add("颜", "顏");
        jianDic.Add("额", "額");
        jianDic.Add("颞", "顳");
        jianDic.Add("颟", "顢");
        jianDic.Add("颠", "顛");
        jianDic.Add("颡", "顙");
        jianDic.Add("颢", "顥");
        jianDic.Add("颤", "顫");
        jianDic.Add("颦", "顰");
        jianDic.Add("颧", "顴");
        jianDic.Add("风", "風");
        jianDic.Add("飑", "颮");
        jianDic.Add("飒", "颯");
        jianDic.Add("飓", "颶");
        jianDic.Add("飕", "颼");
        jianDic.Add("飘", "飄");
        jianDic.Add("飙", "飆");
        jianDic.Add("飞", "飛");
        jianDic.Add("飨", "饗");
        jianDic.Add("餍", "饜");
        jianDic.Add("饥", "饑");
        jianDic.Add("饧", "餳");
        jianDic.Add("饨", "飩");
        jianDic.Add("饩", "餼");
        jianDic.Add("饪", "飪");
        jianDic.Add("饫", "飫");
        jianDic.Add("饬", "飭");
        jianDic.Add("饭", "飯");
        jianDic.Add("饮", "飲");
        jianDic.Add("饯", "餞");
        jianDic.Add("饰", "飾");
        jianDic.Add("饱", "飽");
        jianDic.Add("饲", "飼");
        jianDic.Add("饴", "飴");
        jianDic.Add("饵", "餌");
        jianDic.Add("饶", "饒");
        jianDic.Add("饷", "餉");
        jianDic.Add("饺", "餃");
        jianDic.Add("饼", "餅");
        jianDic.Add("饽", "餑");
        jianDic.Add("饿", "餓");
        jianDic.Add("馁", "餒");
        jianDic.Add("馄", "餛");
        jianDic.Add("馅", "餡");
        jianDic.Add("馆", "館");
        jianDic.Add("馈", "饋");
        jianDic.Add("馊", "餿");
        jianDic.Add("馋", "饞");
        jianDic.Add("馍", "饃");
        jianDic.Add("馏", "餾");
        jianDic.Add("馐", "饈");
        jianDic.Add("馑", "饉");
        jianDic.Add("馒", "饅");
        jianDic.Add("馔", "饌");
        jianDic.Add("马", "馬");
        jianDic.Add("驭", "馭");
        jianDic.Add("驮", "馱");
        jianDic.Add("驯", "馴");
        jianDic.Add("驰", "馳");
        jianDic.Add("驱", "驅");
        jianDic.Add("驳", "駁");
        jianDic.Add("驴", "驢");
        jianDic.Add("驵", "駔");
        jianDic.Add("驶", "駛");
        jianDic.Add("驷", "駟");
        jianDic.Add("驸", "駙");
        jianDic.Add("驹", "駒");
        jianDic.Add("驺", "騶");
        jianDic.Add("驻", "駐");
        jianDic.Add("驼", "駝");
        jianDic.Add("驽", "駑");
        jianDic.Add("驾", "駕");
        jianDic.Add("驿", "驛");
        jianDic.Add("骀", "駘");
        jianDic.Add("骁", "驍");
        jianDic.Add("骂", "罵");
        jianDic.Add("骄", "驕");
        jianDic.Add("骅", "驊");
        jianDic.Add("骆", "駱");
        jianDic.Add("骇", "駭");
        jianDic.Add("骈", "駢");
        jianDic.Add("骊", "驪");
        jianDic.Add("骋", "騁");
        jianDic.Add("验", "驗");
        jianDic.Add("骏", "駿");
        jianDic.Add("骐", "騏");
        jianDic.Add("骑", "騎");
        jianDic.Add("骒", "騍");
        jianDic.Add("骓", "騅");
        jianDic.Add("骖", "驂");
        jianDic.Add("骗", "騙");
        jianDic.Add("骘", "騭");
        jianDic.Add("骚", "騷");
        jianDic.Add("骛", "騖");
        jianDic.Add("骜", "驁");
        jianDic.Add("骝", "騮");
        jianDic.Add("骞", "騫");
        jianDic.Add("骟", "騸");
        jianDic.Add("骠", "驃");
        jianDic.Add("骡", "騾");
        jianDic.Add("骢", "驄");
        jianDic.Add("骣", "驏");
        jianDic.Add("骤", "驟");
        jianDic.Add("骥", "驥");
        jianDic.Add("骧", "驤");
        jianDic.Add("髅", "髏");
        jianDic.Add("髋", "髖");
        jianDic.Add("髌", "髕");
        jianDic.Add("鬓", "鬢");
        jianDic.Add("魇", "魘");
        jianDic.Add("魉", "魎");
        jianDic.Add("鱼", "魚");
        jianDic.Add("鱿", "魷");
        jianDic.Add("鲁", "魯");
        jianDic.Add("鲂", "魴");
        jianDic.Add("鲈", "鱸");
        jianDic.Add("鲋", "鮒");
        jianDic.Add("鲍", "鮑");
        jianDic.Add("鲎", "鱟");
        jianDic.Add("鲐", "鮐");
        jianDic.Add("鲑", "鮭");
        jianDic.Add("鲒", "鮚");
        jianDic.Add("鲔", "鮪");
        jianDic.Add("鲕", "鮞");
        jianDic.Add("鲚", "鱭");
        jianDic.Add("鲛", "鮫");
        jianDic.Add("鲜", "鮮");
        jianDic.Add("鲟", "鱘");
        jianDic.Add("鲠", "鯁");
        jianDic.Add("鲡", "鱺");
        jianDic.Add("鲢", "鰱");
        jianDic.Add("鲣", "鰹");
        jianDic.Add("鲤", "鯉");
        jianDic.Add("鲥", "鰣");
        jianDic.Add("鲦", "鰷");
        jianDic.Add("鲧", "鯀");
        jianDic.Add("鲨", "鯊");
        jianDic.Add("鲩", "鯇");
        jianDic.Add("鲫", "鯽");
        jianDic.Add("鲭", "鯖");
        jianDic.Add("鲮", "鯪");
        jianDic.Add("鲰", "鯫");
        jianDic.Add("鲱", "鯡");
        jianDic.Add("鲲", "鯤");
        jianDic.Add("鲳", "鯧");
        jianDic.Add("鲵", "鯢");
        jianDic.Add("鲶", "鯰");
        jianDic.Add("鲷", "鯛");
        jianDic.Add("鲸", "鯨");
        jianDic.Add("鲻", "鯔");
        jianDic.Add("鲽", "鰈");
        jianDic.Add("鳃", "鰓");
        jianDic.Add("鳄", "鱷");
        jianDic.Add("鳅", "鰍");
        jianDic.Add("鳆", "鰒");
        jianDic.Add("鳇", "鰉");
        jianDic.Add("鳌", "鰲");
        jianDic.Add("鳍", "鰭");
        jianDic.Add("鳎", "鰨");
        jianDic.Add("鳏", "鰥");
        jianDic.Add("鳐", "鰩");
        jianDic.Add("鳓", "鰳");
        jianDic.Add("鳔", "鰾");
        jianDic.Add("鳕", "鱈");
        jianDic.Add("鳖", "鱉");
        jianDic.Add("鳗", "鰻");
        jianDic.Add("鳜", "鱖");
        jianDic.Add("鳝", "鱔");
        jianDic.Add("鳞", "鱗");
        jianDic.Add("鳟", "鱒");
        jianDic.Add("鳢", "鱧");
        jianDic.Add("鸟", "鳥");
        jianDic.Add("鸠", "鳩");
        jianDic.Add("鸡", "雞");
        jianDic.Add("鸢", "鳶");
        jianDic.Add("鸣", "鳴");
        jianDic.Add("鸥", "鷗");
        jianDic.Add("鸦", "鴉");
        jianDic.Add("鸨", "鴇");
        jianDic.Add("鸩", "鴆");
        jianDic.Add("鸪", "鴣");
        jianDic.Add("鸫", "鶇");
        jianDic.Add("鸬", "鸕");
        jianDic.Add("鸭", "鴨");
        jianDic.Add("鸯", "鴦");
        jianDic.Add("鸱", "鴟");
        jianDic.Add("鸲", "鴝");
        jianDic.Add("鸳", "鴛");
        jianDic.Add("鸵", "鴕");
        jianDic.Add("鸶", "鷥");
        jianDic.Add("鸷", "鷙");
        jianDic.Add("鸸", "鴯");
        jianDic.Add("鸹", "鴰");
        jianDic.Add("鸺", "鵂");
        jianDic.Add("鸽", "鴿");
        jianDic.Add("鸾", "鸞");
        jianDic.Add("鸿", "鴻");
        jianDic.Add("鹁", "鵓");
        jianDic.Add("鹂", "鸝");
        jianDic.Add("鹃", "鵑");
        jianDic.Add("鹄", "鵠");
        jianDic.Add("鹅", "鵝");
        jianDic.Add("鹆", "鵒");
        jianDic.Add("鹇", "鷴");
        jianDic.Add("鹈", "鵜");
        jianDic.Add("鹉", "鵡");
        jianDic.Add("鹊", "鵲");
        jianDic.Add("鹌", "鵪");
        jianDic.Add("鹎", "鵯");
        jianDic.Add("鹏", "鵬");
        jianDic.Add("鹑", "鶉");
        jianDic.Add("鹕", "鶘");
        jianDic.Add("鹗", "鶚");
        jianDic.Add("鹘", "鶻");
        jianDic.Add("鹚", "鶿");
        jianDic.Add("鹜", "鶩");
        jianDic.Add("鹞", "鷂");
        jianDic.Add("鹣", "鶼");
        jianDic.Add("鹤", "鶴");
        jianDic.Add("鹦", "鸚");
        jianDic.Add("鹧", "鷓");
        jianDic.Add("鹨", "鷚");
        jianDic.Add("鹩", "鷯");
        jianDic.Add("鹪", "鷦");
        jianDic.Add("鹫", "鷲");
        jianDic.Add("鹬", "鷸");
        jianDic.Add("鹭", "鷺");
        jianDic.Add("鹰", "鷹");
        jianDic.Add("鹳", "鸛");
        jianDic.Add("鹾", "鹺");
        jianDic.Add("麦", "麥");
        jianDic.Add("麸", "麩");
        jianDic.Add("么", "麼");
        jianDic.Add("黄", "黃");
        jianDic.Add("黉", "黌");
        jianDic.Add("黩", "黷");
        jianDic.Add("黪", "黲");
        jianDic.Add("黾", "黽");
        jianDic.Add("鼋", "黿");
        jianDic.Add("鼍", "鼉");
        jianDic.Add("鼹", "鼴");
        jianDic.Add("齐", "齊");
        jianDic.Add("齑", "齏");
        jianDic.Add("齿", "齒");
        jianDic.Add("龀", "齔");
        jianDic.Add("龃", "齟");
        jianDic.Add("龄", "齡");
        jianDic.Add("龅", "齙");
        jianDic.Add("龆", "齠");
        jianDic.Add("龇", "齜");
        jianDic.Add("龈", "齦");
        jianDic.Add("龉", "齬");
        jianDic.Add("龊", "齪");
        jianDic.Add("龋", "齲");
        jianDic.Add("龌", "齷");
        jianDic.Add("龙", "龍");
        jianDic.Add("龚", "龔");
        jianDic.Add("龛", "龕");
        jianDic.Add("龟", "龜");
        #endregion
    }
    #region 简繁互转
    /// <summary>
    /// 繁转简
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string T2S(string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;
        StringBuilder sb = new StringBuilder();
        foreach (char i in s)
        {
            if (fanDic.ContainsKey(i.ToString()))
            {
                sb.Append(fanDic[i.ToString()]);
            }
            else
            {
                sb.Append(i.ToString());
            }
        }
        return sb.ToString();
    }
    /// <summary>
    /// 简转繁
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string S2T(string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;
        StringBuilder sb = new StringBuilder();
        foreach (char i in s)
        {
            if (jianDic.ContainsKey(i.ToString()))
            {
                sb.Append(jianDic[i.ToString()]);
            }
            else
            {
                sb.Append(i.ToString());
            }
        }
        return sb.ToString();
    }
    #endregion
}