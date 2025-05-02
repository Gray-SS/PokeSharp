using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core;
using Pokemon.DesktopGL.Core.Renderers;

public class CharacterRenderer
{
    public Vector2 Offset { get; set; } = new Vector2(0, GameConstants.TileSize * -0.35f);

    private readonly Character _character;
    private readonly AnimationPack _runAnim;
    private readonly AnimationPack _idleAnim;
    private readonly AnimationPlayer _animPlayer;

    private bool _wasMoving;
    private Direction _lastDirection;

    public CharacterRenderer(Character character)
    {
        _character = character;
        _runAnim = character.Data.RunAnimations;
        _idleAnim = character.Data.IdleAnimations;

        _animPlayer = new AnimationPlayer();
        _animPlayer.Play(_idleAnim[_character.Direction.ToString()]);
        _lastDirection = _character.Direction;
    }

    public void Update(float dt)
    {
        var dir = _character.Direction;
        bool isMoving = _character.IsMoving;

        if (_wasMoving != isMoving || _lastDirection != dir)
        {
            var anim = isMoving ? _runAnim : _idleAnim;
            _animPlayer.Play(anim[dir.ToString()]);
        }

        _animPlayer.Update(dt);
        _wasMoving = isMoving;
        _lastDirection = dir;
    }

    public void Draw(GameRenderer renderer)
    {
        var pos = _character.Position + Offset;
        renderer.Draw(_animPlayer.Sprite, pos, _character.Size, Color.White);
    }
}