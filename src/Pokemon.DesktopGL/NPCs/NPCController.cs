using System;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core;

namespace Pokemon.DesktopGL.NPCs;

public sealed class NPCController : CharacterController
{
    private float _timer = 0.0f;

    public NPCController(Character character) : base(character)
    {
    }

    public override void Update(float dt)
    {
        _timer += dt;
        if (_timer >= 1.0f)
        {
            _timer = 0.0f;

            var dir = (Direction)Random.Shared.Next(0, 4);
            Character.Move(dir);
        }
    }
}
