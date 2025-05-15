using System;
using Microsoft.Xna.Framework;
using PokeSharp.Core;
using PokeSharp.Core.Graphics;
using PokeSharp.Core.Renderers;

namespace Pokemon.DesktopGL.Characters;

public class CharacterRenderer
{
    public Vector2 Offset { get; set; }

    private readonly Character _character;
    private readonly AnimationPack _runAnim;
    private readonly AnimationPack _idleAnim;
    private readonly AnimationPlayer _animPlayer;

    private float _jumpTimer = 0.0f;
    private bool _isJumping = false;

    private bool _wasMoving;
    private Direction _lastDirection;

    private static readonly float JumpDuration = 0.2f;
    private static readonly float JumpHeight = GameConstants.TileSize * 0.2f;
    private static readonly Vector2 BaseOffset = new(0, GameConstants.TileSize * -0.35f);

    public CharacterRenderer(Character character)
    {
        _character = character;
        _character.Rotated += OnCharacterRotation;

        _runAnim = character.Data.RunAnimations;
        _idleAnim = character.Data.IdleAnimations;

        _animPlayer = new AnimationPlayer();
        _animPlayer.Play(_idleAnim[_character.Direction.ToString()]);
        _lastDirection = _character.Direction;

        Offset = BaseOffset;
    }

    private void OnCharacterRotation(object sender, EventArgs e)
    {
        _jumpTimer = 0f;
        _isJumping = true;
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

        if (_isJumping)
        {
            _jumpTimer += dt;
            float t = _jumpTimer / JumpDuration;

            if (t >= 1f)
            {
                _isJumping = false;
                Offset = BaseOffset;
            }
            else
            {
                float jumpY = (float)Math.Sin(t * Math.PI) * JumpHeight;
                Offset = BaseOffset + new Vector2(0, -jumpY);
            }
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