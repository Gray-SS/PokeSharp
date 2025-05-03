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
        if (_timer >= 3.0f)
        {
            _timer = 0.0f;

            var actionId = Random.Shared.Next(0, 2);
            if (actionId == 0)
            {
                var dir = (Direction)Random.Shared.Next(0, 4);
                Character.Move(dir);
            }
            else if (actionId == 1)
            {
                var dir = (Direction)Random.Shared.Next(0, 4);
                Character.Rotate(dir);
            }
        }
    }
}
