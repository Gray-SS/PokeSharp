using System.Linq;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.World;

namespace Pokemon.DesktopGL.NPCs;

public sealed class NPC : WorldEntity
{
    public NPCData Data { get; }

    private readonly NPCController _controller;

    public NPC(NPCData data, Character character) : base(character)
    {
        Data = data;
        _controller = new NPCController(character);
    }

    public override void Update(float dt)
    {
        _controller.Update(dt);
        base.Update(dt);
    }

    public override void Interact()
    {
        string dialogue = Data.Dialogues.FirstOrDefault() ?? "No dialogue found";
        System.Console.WriteLine($"{Data.Name} said '{dialogue}'.");
    }
}