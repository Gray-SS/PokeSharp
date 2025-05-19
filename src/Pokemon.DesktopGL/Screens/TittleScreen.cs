using System;
using Microsoft.Xna.Framework;
using NativeFileDialogSharp;
using PokeSharp.Engine;
using PokeSharp.Engine.Graphics;
using PokeSharp.Engine.Renderers;
using PokeSharp.Engine.UI;
using PokeSharp.ROM;

namespace Pokemon.DesktopGL.Screens;

public sealed class TitleScreen : Screen
{
    private UICanvas _canvas;
    private Sprite _background;
    private UIRenderer _uiRenderer;

    private UILabel _romInfoLabel;

    protected override void Initialize()
    {
        PokemonRom rom = RomManager.Rom;

        _romInfoLabel = new UILabel
        {
            Anchor = UIAnchorPoint.Center,
            Text = RomManager.IsRomLoaded ? $"{rom.Info}" : "No ROM loaded",
            FontSize = 30f,
            TextColor = Color.White,
            Margin = new UIThickness(0, 15, 0, 0)
        };

        _canvas = new UICanvas(ResolutionManager, new UIContainer
        {
            Anchor = UIAnchorPoint.Center,
            Padding = new UIThickness(30),
            Children = [
                new UILabel
                {
                    Anchor = UIAnchorPoint.Center,
                    Text = "PokeSharp - v0.0.1",
                    FontSize = 50f,
                    TextColor = Color.White,
                },
                _romInfoLabel,
                new UIContainer
                {
                    Anchor = UIAnchorPoint.Center,
                    Margin = new UIThickness(0, 50, 0, 0),
                    Children = [
                        new UIButton
                        {
                            Width = UISize.Pixels(400),
                            Height = UISize.Pixels(80),
                            Anchor = UIAnchorPoint.Center,
                            Text = RomManager.IsRomLoaded ? "Play" : "Load ROM",
                            FontSize = 30f,
                            Margin = new UIThickness(0, 10, 0, 0),
                            OnClicked = OnPlayClicked
                        },
                        new UIButton
                        {
                            Width = UISize.Pixels(400),
                            Height = UISize.Pixels(80),
                            Anchor = UIAnchorPoint.Center,
                            Text = "Exit",
                            FontSize = 30f,
                            Margin = new UIThickness(0, 20, 0, 0),
                            OnClicked = OnExitClicked
                        }
                    ]
                }
            ]
        });

        _background = Engine.AssetsManager.Sprite_Background;
        _uiRenderer = new UIRenderer(GraphicsDevice);

        base.Initialize();
    }

    private void OnPlayClicked(object sender, EventArgs e)
    {
        if (RomManager.IsRomLoaded)
        {
            Engine.ScreenManager.Push(new OverworldScreen());
        }
        else
        {
            DialogResult result = Dialog.FileOpen("gba");
            if (!result.IsOk)
                return;

            string romPath = result.Path;
            if (!RomManager.LoadRom(romPath))
                return;

            UIButton button = (UIButton)sender;
            button.Text = "Play";
            _romInfoLabel.Text = $"{RomManager.Rom.Info}";
        }
    }

    private void OnExitClicked(object sender, EventArgs e)
    {
        Engine.Exit();
    }

    protected override void Update(GameTime gameTime)
    {
        _canvas.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(20, 20, 20));

        _uiRenderer.Begin();
        _uiRenderer.Draw(_background, WindowManager.Rect, Color.White);
        _canvas.Draw(_uiRenderer);
        _uiRenderer.End();
    }
}