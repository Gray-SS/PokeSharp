using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Patrol;
using Pokemon.DesktopGL.World;
using PokeSharp.Engine;

namespace Pokemon.DesktopGL.NPCs;

public sealed class NPC : Entity
{
    public string Name { get; }
    public string[] Dialogues { get; }
    public PatrolPath PatrolPath { get; }

    private bool _isInteracting;
    private readonly NPCController _controller;

    public NPC(Character character, string name, string[] dialogues, PatrolPath patrolPath) : base(character)
    {
        Name = name;
        Dialogues = dialogues;
        PatrolPath = patrolPath;

        PokemonGame.Instance.DialogueManager.DialogueEnded += () => {
            if (_isInteracting)
                _isInteracting = false;
        };

        _controller = new NPCController(this);
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
        PokemonGame.Instance.DialogueManager.StartDialogue(Dialogues);
    }
}