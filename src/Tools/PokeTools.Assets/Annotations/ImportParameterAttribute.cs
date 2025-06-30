namespace PokeTools.Assets.Annotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ImportParameterAttribute : Attribute
{
    public string DisplayName { get; }
    public string? Description { get; init; }

    public ImportParameterAttribute(string displayName)
    {
        DisplayName = displayName ?? GetType().Name;
    }
}