using System;

namespace Pokemon.DesktopGL.Core.Extensions;

public static class DotTiledExtensions
{
    public static bool TryGetString(this DotTiled.Object obj, string name, out string value)
    {
        value = null;

        bool found = obj.TryGetProperty(name, out DotTiled.StringProperty prop);
        if (found)
            value = prop.Value;

        return found;
    }

    public static float GetFloatRequired(this DotTiled.Object obj, string name)
    {
        if (obj.TryGetProperty<DotTiled.FloatProperty>(name, out var property))
            return property.Value;

        throw new InvalidOperationException($"The property '{name}' is required but was not found");
    }

    public static string GetStringRequired(this DotTiled.Object obj, string name)
    {
        if (obj.TryGetProperty<DotTiled.StringProperty>(name, out var property))
            return property.Value;

        throw new InvalidOperationException($"The property '{name}' is required but was not found");
    }

    public static string GetStringOrDefault(this DotTiled.Object obj, string name, string defaultValue = null)
    {
        string result = defaultValue;
        if (obj.TryGetProperty<DotTiled.StringProperty>(name, out var property))
            result = property.Value;

        return result;
    }

    public static string GetStringOrEmpty(this DotTiled.Object obj, string name)
    {
        string result = string.Empty;
        if (obj.TryGetProperty<DotTiled.StringProperty>(name, out var property))
            result = property.Value;

        return result;
    }
}