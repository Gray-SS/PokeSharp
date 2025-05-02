using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.DesktopGL.Core.Managers;
using Pokemon.DesktopGL.Core.Renderers;

namespace Pokemon.DesktopGL.Core.Entities;

public enum PlayerDirection
{
    Left,
    Right,
    Up,
    Down
}

public sealed class Player
{
    public const float Speed = 300.0f;

    public Vector2 Position { get; set; }
    public Vector2 Offset { get; set; }
    public Vector2 Size { get; set; }
    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

    private bool _isMoving;
    private bool _premoved;
    private Vector2 _targetPos;
    private Vector2 _nextTargetPos;
    private PlayerDirection _dir;
    private PlayerDirection _nextDir;

    private readonly InputManager _inputManager;
    private readonly AssetsManager _assetsManager;
    private readonly AnimationPlayer _animPlayer;

    public Player(InputManager inputManager, AssetsManager assetsManager)
    {
        _inputManager = inputManager;
        _assetsManager = assetsManager;

        Position = Vector2.Zero;
        Size = new Vector2(50, 75);

        _targetPos = Position;
        _nextTargetPos = Position;
        _dir = PlayerDirection.Down;

        _animPlayer = new AnimationPlayer();
        _animPlayer.Play(assetsManager.Anim_BoyIdle["Down"]);
    }

    public void Update(float dt)
    {
        bool wasMoving = _isMoving;
        PlayerDirection previousDir = _dir;

        if (!_isMoving)
        {
            if (_inputManager.IsKeyDown(Keys.D) || _inputManager.IsKeyDown(Keys.Right))
            {
                _targetPos = Position + new Vector2(GameConstants.TileSize, 0f);
                _dir = PlayerDirection.Right;
                _isMoving = true;
            }
            else if (_inputManager.IsKeyDown(Keys.A) || _inputManager.IsKeyDown(Keys.Left))
            {
                _targetPos = Position - new Vector2(GameConstants.TileSize, 0f);
                _dir = PlayerDirection.Left;
                _isMoving = true;
            }
            else if (_inputManager.IsKeyDown(Keys.W) || _inputManager.IsKeyDown(Keys.Up))
            {
                _targetPos = Position - new Vector2(0f, GameConstants.TileSize);
                _dir = PlayerDirection.Up;
                _isMoving = true;
            }
            else if (_inputManager.IsKeyDown(Keys.S) || _inputManager.IsKeyDown(Keys.Down))
            {
                _targetPos = Position + new Vector2(0f, GameConstants.TileSize);
                _dir = PlayerDirection.Down;
                _isMoving = true;
            }
        }
        else if (_isMoving)
        {
            float distanceSquared = Vector2.DistanceSquared(Position, _targetPos);

            if (distanceSquared <= 8f)
            {
                if (_inputManager.IsKeyDown(Keys.D) || _inputManager.IsKeyDown(Keys.Right))
                {
                    _premoved = true;
                    _nextTargetPos = _targetPos + new Vector2(GameConstants.TileSize, 0.0f);
                    _nextDir = PlayerDirection.Right;
                }
                else if (_inputManager.IsKeyDown(Keys.A) || _inputManager.IsKeyDown(Keys.Left))
                {
                    _premoved = true;
                    _nextTargetPos = _targetPos + new Vector2(-GameConstants.TileSize, 0.0f);
                    _nextDir = PlayerDirection.Left;
                }
                else if (_inputManager.IsKeyDown(Keys.W) || _inputManager.IsKeyDown(Keys.Up))
                {
                    _premoved = true;
                    _nextTargetPos = _targetPos + new Vector2(0.0f, -GameConstants.TileSize);
                    _nextDir = PlayerDirection.Up;
                }
                else if (_inputManager.IsKeyDown(Keys.S) || _inputManager.IsKeyDown(Keys.Down))
                {
                    _premoved = true;
                    _nextTargetPos = _targetPos + new Vector2(0.0f, GameConstants.TileSize);
                    _nextDir = PlayerDirection.Down;
                }
            }

            if (distanceSquared >= 2f)
            {
                var dir = _targetPos - Position;
                dir.Normalize();

                Position += dir * Speed * dt;
            }
            else
            {
                Position = _targetPos;


                if (_premoved)
                {
                    _targetPos = _nextTargetPos;
                    _dir = _nextDir;
                    _premoved = false;
                }
                else
                {
                    _isMoving = false;
                }
            }
        }

        // Update animation based on movement state and direction
        UpdateAnimation(wasMoving, previousDir);

        _animPlayer.Update(dt);
    }

    private void UpdateAnimation(bool wasMoving, PlayerDirection previousDir)
    {
        string directionString = _dir.ToString();

        if (_isMoving != wasMoving || _dir != previousDir)
        {
            if (_isMoving)
            {
                _animPlayer.Play(_assetsManager.Anim_BoyRun[directionString]);
            }
            else
            {
                _animPlayer.Play(_assetsManager.Anim_BoyIdle[directionString]);
            }
        }
    }

    public void Draw(GameRenderer renderer)
    {
        renderer.Draw(_animPlayer.Sprite, Position + Offset, Size, Color.White);
    }
}