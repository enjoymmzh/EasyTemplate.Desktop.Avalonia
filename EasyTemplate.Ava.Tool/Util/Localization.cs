using System.Reflection;
using System.Resources;
using System.Text;

namespace EasyTemplate.Ava.Tool.Util;

public class Localization
{
    public const string SimplifiedChinese = "zh-CN";
    public const string TraditionalChinese = "zh-TW";
    public const string EnglishUS = "en-US";

    private static readonly Dictionary<string, Dictionary<string, string>> _cache = new();
    private static string _currentLang = SimplifiedChinese;
    private static readonly ResourceManager ResourceManager = new("EasyTemplate.Ava.Tool.Properties.Resources", Assembly.GetExecutingAssembly());

    public static string CurrentLanguage
    {
        get => _currentLang;
        set
        {
            if (_currentLang != value && SupportedLanguages.Contains(value))
            {
                _currentLang = value;
                LoadLanguage(_currentLang);
            }
        }
    }

    public static IReadOnlyList<string> SupportedLanguages { get; } = new[]
    {
        SimplifiedChinese,
        TraditionalChinese,
        EnglishUS
    };

    static Localization()
    {
        foreach (var lang in SupportedLanguages)
        {
            LoadLanguage(lang);
        }
    }

    private static void LoadLanguage(string langCode)
    {
        var dict = new Dictionary<string, string>();
        // 资源名如: zh-CN.csv
        var resourceName = $"{langCode}.csv";
        var stream = ResourceManager.GetObject(langCode) as byte[];
        string csvContent = Encoding.UTF8.GetString(stream);
        if (!string.IsNullOrEmpty(csvContent))
        {
            foreach (var line in csvContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split(',', 2);
                if (parts.Length == 2)
                {
                    dict[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }
        _cache[langCode] = dict;
    }

    public static string Get(string key)
    {
        if (_cache.TryGetValue(_currentLang, out var dict) && dict.TryGetValue(key, out var value))
        {
            return value;
        }
        return key;
    }
}
