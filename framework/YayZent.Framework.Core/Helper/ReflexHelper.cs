namespace YayZent.Framework.Core.Helper;

public static class ReflexHelper
{
    public static string? GetModelValue(string fieldName, object? obj)
    {
        if (obj == null || string.IsNullOrWhiteSpace(fieldName))
            return null;

        var prop = obj.GetType().GetProperty(fieldName);
        if (prop == null)
            return null;

        var value = prop.GetValue(obj);
        return value?.ToString();
    }

    public static bool SetModelValue(string fieldName, object value, object? obj)
    {
        if (obj == null || string.IsNullOrWhiteSpace(fieldName))
            return false;

        var prop = obj.GetType().GetProperty(fieldName);
        if (prop == null || !prop.CanWrite)
            return false;

        prop.SetValue(obj, value);
        return true;
    }

}