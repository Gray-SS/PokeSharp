using Pokemon.DesktopGL.Characters;

namespace Pokemon.DesktopGL.NPCs;

public sealed class NPC : CharacterEntity
{
    private readonly NPCController _controller;

    public NPC(Character character) : base(character)
    {
        _controller = new NPCController(character);
    }

    public override void Update(float dt)
    {
        _controller.Update(dt);
        base.Update(dt);
    }
}