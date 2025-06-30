using System.Reflection;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets;

public sealed class ImportParameter
{
    public string DisplayName { get; }
    public string? Description { get; }

    public IAssetImporter Importer => _target;
    public Type ParameterType => _property.PropertyType;

    private readonly IAssetImporter _target;
    private readonly PropertyInfo _property;

    public ImportParameter(IAssetImporter target, PropertyInfo property, ImportParameterAttribute attr)
    {
        DisplayName = attr.DisplayName;
        Description = attr.Description;

        _target = target;
        _property = property;
    }

    public object? GetValue()
    {
        return _property.GetValue(_target);
    }

    public void SetValue(object? value)
    {
        _property.SetValue(_target, value);
    }
}