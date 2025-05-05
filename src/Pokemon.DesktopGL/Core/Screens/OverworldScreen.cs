using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core.Renderers;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.World;
using Pokemon.DesktopGL.Dialogues;
using System;
using System.Collections;
using Pokemon.DesktopGL.Core.Coroutines;
using Pokemon.DesktopGL.Core.Managers;
using Pokemon.DesktopGL.Core.Tweening;

namespace Pokemon.DesktopGL.Core.Screens;

public sealed class OverworldScreen : Screen
{
    private Camera _camera;
    private UIRenderer _uiRenderer;
    private GameRenderer _gameRenderer;

    private DialogueBoxRenderer _dialogueRenderer;

    private GameMap _map;
    private Overworld _world;

    public override void Initialize()
    {
        _camera = new Camera(Game.WindowManager);
        _uiRenderer = new UIRenderer(GraphicsDevice);
        _gameRenderer = new GameRenderer(GraphicsDevice);
        _dialogueRenderer = new DialogueBoxRenderer(Game.DialogueManager);

        _map = GameMap.Load("Content/Data/Maps/test_map.tmx");

        _world = new Overworld(_map);
        _world.LoadEntities();

        _world.Player.Character.Moved += OnPlayerMove;

        Game.ActiveWorld = _world;

        base.Initialize();
    }

    public override void Load()
    {
        _camera.Zoom = 1.0f;
        _world.Player.Character.MovementEnabled = true;
        _world.Player.Character.RotationEnabled = true;

        CoroutineManager.Start(FadeOut());
    }

    private void OnPlayerMove(object sender, EventArgs e)
    {
        if (_world.IsInLeaf(_world.Player.Character) && Game.PlayerData.CanFight)
        {
            var prob = Random.Shared.NextSingle();
            if (prob <= 0.1f)
                CoroutineManager.Start(PlayBattleIntro());
        }
    }

    private IEnumerator PlayBattleIntro()
    {
        _world.Player.Character.MovementEnabled = false;
        _world.Player.Character.RotationEnabled = false;
        _world.Player.Character.Rotate(Direction.Down, force: true);

        CoroutineManager.Start(Tween.To((v) => _camera.Zoom = v, () => _camera.Zoom, 3f, 1f, Easing.InOutQuad));
        yield return FadeIn();

        var zone = _world.GetCurrentZone();
        var creature = zone.Spawn();

        Game.ScreenManager.Push(new BattleScreen(creature));
    }

    public override void Unload()
    {
        _world = null;
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _world.Update(dt);
        _dialogueRenderer.Update(dt);

        var target = _world.Player.Character.Position;

        var viewportSize = _camera.Viewport.Bounds.Size.ToVector2();
        var halfView = viewportSize * 0.5f;

        int mapPixelWidth  = (int)(_map.TiledMap.Width * GameConstants.TileSize);
        int mapPixelHeight = (int)(_map.TiledMap.Height * GameConstants.TileSize);

        float clampedX = MathHelper.Clamp(target.X, halfView.X + 20, mapPixelWidth  - halfView.X - 40);
        float clampedY = MathHelper.Clamp(target.Y, halfView.Y + 20, mapPixelHeight - halfView.Y - 40);

        Vector2 clampedTarget = new(clampedX, clampedY);
        _camera.Position = Vector2.Lerp(_camera.Position, clampedTarget, 0.05f);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _gameRenderer.Begin(_camera);
        _world.Draw(_gameRenderer);
        _gameRenderer.End();

        _uiRenderer.Begin();
        _dialogueRenderer.Draw(_uiRenderer);
        _uiRenderer.End();

        base.Draw(gameTime);
    }
}