using System.Collections;
using System.Reflection;
using System.Text.Json;
using PokeSharp.Assets;
using PokeSharp.Core.Logging;
using PokeSharp.Entities;

namespace PokeSharp.Scenes;

public sealed class SceneProcessor : AssetProcessor<SceneData, Scene>
{
    private readonly Logger _logger;

    public SceneProcessor(Logger logger)
    {
        _logger = logger;
    }

    public override Scene Process(SceneData rawAsset)
    {
        var scene = new Scene(rawAsset.Name);

        foreach (var entityData in rawAsset.Entities)
        {
            Type? type = Type.GetType(entityData.TypeName);
            if (type == null)
            {
                _logger.Warn(
                    $"Could not resolve type '{entityData.TypeName}' for entity '{entityData.Id}'. " +
                    "This might happen if the type was renamed, moved to another assembly, or deleted.");
                continue;
            }

            Entity? entity = InstantiateEntity(entityData, type);
            if (entity == null)
                continue;

            entity.Id = entityData.Id;
            entity.Tag = entityData.Tag;

            AssignProperties(type, entity, entityData.Properties);

            _logger.Debug($"Loaded entity '{entity.Id}'");
            scene.Entities.Add(entity);
        }

        return scene;
    }

    private Entity? InstantiateEntity(EntityData data, Type type)
    {
        try
        {
            if (Activator.CreateInstance(type) is not Entity entity)
            {
                _logger.Warn(
                    $"Resolved type '{type.FullName}' from entity '{data.Id}' " +
                    "does not inherit from Entity. Make sure custom types derive from the Entity base class.");
                return null;
            }

            return entity;
        }
        catch (Exception ex)
        {
            _logger.Warn(
                $"Failed to instantiate entity '{data.Id}' of type '{type.FullName}': {ex.Message}");
            return null;
        }
    }

    private void AssignProperties(Type entityType, Entity instance, Dictionary<string, object> properties)
    {
        foreach (var property in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!property.CanWrite || !properties.TryGetValue(property.Name, out var rawValue))
                continue;

            try
            {
                _logger.Debug($"Raw value: {rawValue}, Type: {rawValue.GetType().Name}");
                object normalized = NormalizeYamlObject(rawValue);
                object? convertedValue = ConvertToPropertyType(normalized, property.PropertyType);
                if (convertedValue == null)
                {
                    _logger.Warn($"Failed to assign property '{property.Name}' to entity '{instance.GetType().Name}'.");
                    continue;
                }

                _logger.Debug($"Converted value: {convertedValue}, Type: {convertedValue.GetType().Name}");

                property.SetValue(instance, convertedValue);
            }
            catch (Exception ex)
            {
                _logger.Warn($"Failed to assign property '{property.Name}' to entity '{instance.GetType().Name}': {ex.Message}");
            }
        }
    }

    private object NormalizeYamlObject(object? input)
    {
        if (input is Dictionary<object, object> dict)
        {
            var result = new Dictionary<string, object>();
            foreach (var kvp in dict)
            {
                string key = kvp.Key.ToString() ?? string.Empty;
                result[key] = NormalizeYamlObject(kvp.Value);
            }
            return result;
        }
        else if (input is IList list)
        {
            return list.Cast<object>().Select(NormalizeYamlObject).ToList();
        }
        else if (input is string s && int.TryParse(s, out var n))
        {
            return n;
        }
        else
        {
            return input!;
        }
    }


    private object? ConvertToPropertyType(object rawValue, Type targetType)
    {
        if (rawValue == null)
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

        if (targetType.IsAssignableFrom(rawValue.GetType()))
            return rawValue;

        Type nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (nonNullableType.IsEnum)
            return Enum.Parse(nonNullableType, rawValue.ToString()!, ignoreCase: true);

        if (nonNullableType.IsPrimitive || nonNullableType == typeof(string) || nonNullableType == typeof(decimal))
            return Convert.ChangeType(rawValue, nonNullableType);

        string json = JsonSerializer.Serialize(rawValue);
        _logger.Trace($"Serialized to intermediate json: {json}");

        _logger.Trace($"Deserializing intermediate json to: {targetType.FullName}");
        return JsonSerializer.Deserialize(json, targetType);
    }

}