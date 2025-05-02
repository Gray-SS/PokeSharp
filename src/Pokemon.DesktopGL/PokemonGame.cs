using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.DesktopGL.Core;
using Pokemon.DesktopGL.Core.Entities;
using Pokemon.DesktopGL.Core.Managers;
using Pokemon.DesktopGL.Core.Screens;

namespace Pokemon.DesktopGL;

public class PokemonGame : Game
{
    // Services / Managers
    public InputManager InputManager { get; private set; }
    public ScreenManager ScreenManager { get; private set; }
    public AssetsManager AssetsManager { get; private set; }
    public WindowManager WindowManager { get; private set; }

    // Game Properties
    public Player Player { get; private set; }

    // MonoGame Properties
    public GraphicsDeviceManager Graphics { get; }
    public static PokemonGame Instance { get; private set; }

    public PokemonGame()
    {
        if (Instance != null)
            throw new InvalidOperationException("Impossible to create multiple instances of the game");

        Instance = this;
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        InputManager = new InputManager();
        ScreenManager = new ScreenManager();
        AssetsManager = new AssetsManager(Content);
        WindowManager = new WindowManager(Window, Graphics);

        WindowManager.SetWindowTitle("Pokemon");
        WindowManager.SetResolution("1280x720");

        base.Initialize();
    }

    protected override void LoadContent()
    {
        AssetsManager.LoadGlobalAssets();

        Player = new Player(InputManager, AssetsManager);
        Player.Offset = new Vector2(0, GameConstants.TileSize * -0.35f);

        ScreenManager.Push(new BattleScreen(this));
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.Update();

        if (InputManager.IsKeyPressed(Keys.Escape))
            Exit();

        if (InputManager.IsKeyPressed(Keys.F11))
            WindowManager.ToggleFullscreen();

        ScreenManager.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        ScreenManager.Draw(gameTime);
    }

    // private void DrawGUI(GameTime gameTime)
    // {
    //     _imGuiRenderer.BeginLayout(gameTime);

    //     if (ImGui.Begin("Tilesets Manager"))
    //     {
    //         if (_tilemap != null)
    //         {
    //             ImGui.Text($"Map Size: {_tilemap.MapWidth}x{_tilemap.MapHeight}");
    //             ImGui.Text($"Total Tiles: {_tilemap.Tiles.Length}");

    //             if (ImGui.BeginChild("TilesetViewer", new System.Numerics.Vector2(0, 300)))
    //             {
    //                 float tileDisplaySize = 64.0f;
    //                 float padding = 5.0f;
    //                 float windowWidth = ImGui.GetWindowWidth();
    //                 int tilesPerRow = Math.Max(1, (int)((windowWidth - padding) / (tileDisplaySize + padding)));

    //                 for (int i = 0; i < _tilemap.Tiles.Length; i++)
    //                 {
    //                     int column = i % tilesPerRow;

    //                     if (column > 0)
    //                         ImGui.SameLine();

    //                     ImGui.BeginGroup();

    //                     Sprite tile = _tilemap.Tiles[i];
    //                     IntPtr textureId = _imGuiRenderer.BindTexture(tile.Texture);

    //                     System.Numerics.Vector2 uv0 = new System.Numerics.Vector2(
    //                         (float)tile.Bounds.X / tile.Texture.Width,
    //                         (float)tile.Bounds.Y / tile.Texture.Height);

    //                     System.Numerics.Vector2 uv1 = new System.Numerics.Vector2(
    //                         (float)(tile.Bounds.X + tile.Bounds.Width) / tile.Texture.Width,
    //                         (float)(tile.Bounds.Y + tile.Bounds.Height) / tile.Texture.Height);

    //                     ImGui.Image(textureId,
    //                                 new System.Numerics.Vector2(tileDisplaySize, tileDisplaySize),
    //                                 uv0, uv1);

    //                     string label = $"ID: {i}";
    //                     float textWidth = ImGui.CalcTextSize(label).X;
    //                     float centerPos = (tileDisplaySize - textWidth) * 0.5f;
    //                     ImGui.SetCursorPosX(ImGui.GetCursorPosX() + centerPos);
    //                     ImGui.Text(label);

    //                     ImGui.EndGroup();

    //                     if (ImGui.IsItemHovered())
    //                     {
    //                         ImGui.BeginTooltip();
    //                         ImGui.Text($"Tile ID: {i}");
    //                         ImGui.Text($"Source: {tile.Bounds.X},{tile.Bounds.Y},{tile.Bounds.Width},{tile.Bounds.Height}");
    //                         ImGui.EndTooltip();
    //                     }
    //                 }
    //             }
    //             ImGui.EndChild();

    //             if (ImGui.CollapsingHeader("Tile Usage"))
    //             {
    //                 var tileUsage = new Dictionary<int, int>();
    //                 for (int i = 0; i < _tilemap.Data.Length; i++)
    //                 {
    //                     int tileId = _tilemap.Data[i];
    //                     if (!tileUsage.ContainsKey(tileId))
    //                         tileUsage[tileId] = 0;
    //                     tileUsage[tileId]++;
    //                 }

    //                 if (ImGui.BeginTable("TileUsage", 2))
    //                 {
    //                     ImGui.TableSetupColumn("Tile ID");
    //                     ImGui.TableSetupColumn("Usage Count");
    //                     ImGui.TableHeadersRow();

    //                     foreach (var pair in tileUsage.OrderBy(p => p.Key))
    //                     {
    //                         ImGui.TableNextRow();

    //                         ImGui.TableSetColumnIndex(0);
    //                         ImGui.Text($"{pair.Key}");

    //                         ImGui.TableSetColumnIndex(1);
    //                         ImGui.Text($"{pair.Value}");
    //                     }

    //                     ImGui.EndTable();
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             ImGui.Text("No tilemap loaded.");
    //         }

    //         ImGui.End();
    //     }

    //     _imGuiRenderer.EndLayout();
    // }
}
