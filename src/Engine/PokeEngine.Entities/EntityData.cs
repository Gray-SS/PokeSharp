namespace PokeEngine.Entities;

public struct EntityData
{
    public string Id { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

    public EntityData()
    {
    }
}