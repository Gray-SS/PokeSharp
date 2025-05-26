using PokeSharp.Entities;

namespace PokeSharp.Scenes;

public struct SceneData
{
    public string Name { get; set; }

    public List<EntityData> Entities { get; set; }
}