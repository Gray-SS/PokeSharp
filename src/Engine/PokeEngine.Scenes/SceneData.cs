using PokeEngine.Entities;

namespace PokeEngine.Scenes;

public struct SceneData
{
    public string Name { get; set; }

    public List<EntityData> Entities { get; set; }
}