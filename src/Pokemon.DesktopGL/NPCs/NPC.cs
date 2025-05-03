using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.World;

namespace Pokemon.DesktopGL.NPCs;

public sealed class NPC : WorldEntity
{
    public NPCData Data { get; }

    private bool _isInteracting;
    private readonly NPCController _controller;

    public NPC(NPCData data, Character character) : base(character)
    {
        Data = data;
        PokemonGame.Instance.DialogueManager.DialogueEnded += () => {
            if (_isInteracting)
                _isInteracting = false;
        };

        _controller = new NPCController(character);
    }

    public override void Update(float dt)
    {
        if (!_isInteracting)
            _controller.Update(dt);

        base.Update(dt);
    }

    public override void Interact()
    {
        _isInteracting = true;
        PokemonGame.Instance.DialogueManager.StartDialogue(Data.Dialogues);
    }
}