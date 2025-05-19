using Microsoft.Xna.Framework;
using PokeSharp.Engine.Renderers;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.World;
using System;
using System.Collections;
using PokeSharp.Engine.Managers;
using PokeSharp.Engine.Tweening;
using Pokemon.DesktopGL.Creatures;
using PokeSharp.Engine;
using Pokemon.DesktopGL.Dialogues;

namespace Pokemon.DesktopGL.Screens;

public sealed class OverworldScreen : Screen
{
    public new PokemonGame Engine => GetEngine<PokemonGame>();

    private Camera _camera;
    private UIRenderer _uiRenderer;
    private GameRenderer _gameRenderer;

    private DialogueBoxRenderer _dialogueRenderer;

    private GameMap _map;
    private Overworld _world;

    protected override void Initialize()
    {
        _camera = new Camera(ResolutionManager);
        _uiRenderer = new UIRenderer(GraphicsDevice);
        _gameRenderer = new GameRenderer(GraphicsDevice);
        _dialogueRenderer = new DialogueBoxRenderer(Engine.DialogueManager);

        _map = GameMap.Load("Content/Data/Maps/test_map.tmx");

        _world = new Overworld(_map);
        Engine.ActiveWorld = _world;

        _world.Load();
        _world.Player.Character.Moved += OnPlayerMove;
    }

    protected override void Load()
    {
        _camera.Zoom = 1.0f;
        _world.Player.Character.MovementEnabled = true;
        _world.Player.Character.RotationEnabled = true;

        CoroutineManager.Start(FadeOut());
    }

    private void OnPlayerMove(object sender, EventArgs e)
    {
        if (_world.IsInLeaf(_world.Player.Character) && Engine.PlayerData.CanFight)
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

        CreatureZone zone = _world.GetCurrentZone();
        Creature creature = zone.SpawnWildCreature();

        Engine.ScreenManager.Push(new BattleScreen(creature));
    }

    protected override void Unload()
    {
        Engine.ActiveWorld = null;
        _world = null;
    }

    protected override void Update(GameTime gameTime)
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

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _gameRenderer.Begin(_camera);
        _world.Draw(_gameRenderer);
        _gameRenderer.End();

        _uiRenderer.Begin();
        _dialogueRenderer.Draw(_uiRenderer);
        _uiRenderer.End();
    }
}