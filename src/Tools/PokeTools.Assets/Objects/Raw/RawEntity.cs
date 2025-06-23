namespace PokeTools.Assets.Objects.Raw;

public sealed record RawEntity(
    string Id,
    string Tag,
    string TypeName,
    Dictionary<string, object> Properties
);