using System.Collections;

namespace nng.Helpers;

public static class EnvironmentHelper
{
    private static readonly Dictionary<string, string?> CachedEnvironmentVariables = new();
    private static bool _initialised;

    private static Dictionary<string, string?> EnvironmentVariables
    {
        get
        {
            if (_initialised) return CachedEnvironmentVariables;
            CachedEnvironmentVariables.Clear();
            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
                CachedEnvironmentVariables.Add((string) entry.Key, (string?) entry.Value);
            _initialised = true;
            return CachedEnvironmentVariables;
        }
    }

    public static long GetLong(string key)
    {
        var value = GetValue(key);
        return long.Parse(value ?? throw new NullReferenceException());
    }

    public static long GetLong(string key, long defaultValue)
    {
        if (!TryGetValue(key, out var value)) return defaultValue;
        return long.TryParse(value, out var result) ? result : defaultValue;
    }

    public static int GetInt(string key)
    {
        var value = GetValue(key);
        return int.Parse(value ?? throw new NullReferenceException());
    }

    public static int GetInt(string key, int defaultValue)
    {
        if (!TryGetValue(key, out var value)) return defaultValue;
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    public static string GetString(string key)
    {
        var value = GetValue(key);
        return value ?? throw new NullReferenceException();
    }

    public static string GetString(string key, string defaultValue)
    {
        return TryGetValue(key, out var value) ? value : defaultValue;
    }

    public static bool GetBoolean(string key)
    {
        var value = GetValue(key);
        if (!bool.TryParse(value, out var output)) throw new NullReferenceException();
        return output;
    }

    public static bool GetBoolean(string key, bool defaultValue)
    {
        if (!TryGetValue(key, out var value)) return defaultValue;
        return bool.TryParse(value, out var output) ? output : defaultValue;
    }

    private static bool TryGetValue(string key, out string value)
    {
        try
        {
            var res = GetValue(key);
            if (res is null)
            {
                value = string.Empty;
                return false;
            }

            value = res;
            return true;
        }
        catch (Exception)
        {
            value = string.Empty;
            return false;
        }
    }

    private static string? GetValue(string key)
    {
        return EnvironmentVariables[key];
    }
}
