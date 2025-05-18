using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Engine.Assets.Importers;
using PokeSharp.Engine.Assets.Processors;
using PokeSharp.Engine.Graphics;
using PokeSharp.Engine.Managers;

namespace PokeSharp.Engine.Assets;

public class AssetsManager
{
    public Sprite Sprite_Blank { get; private set; } = null!;

    public Sprite Sprite_Battle_Forest_Bg { get; private set; } = null!;
    public Sprite Sprite_Battle_Grass_Base0 { get; private set; } = null!;
    public Sprite Sprite_Battle_Grass_Base1 { get; private set; } = null!;
    public Sprite Sprite_Battle_Databox { get; private set; } = null!;
    public Sprite Sprite_Battle_Databox_Foe { get; private set; } = null!;
    public Sprite Sprite_Dialogue_Overlay { get; private set; } = null!;
    public Sprite Sprite_Battle_Exp_Overlay { get; private set; } = null!;
    public Sprite Sprite_Battle_Overlay_Fight { get; private set; } = null!;

    public Sprite Sprite_Fade_Transition { get; private set; } = null!;

    public SpriteSheet Sheet_Battle_Cursor_Action { get; private set; } = null!;
    public SpriteSheet Sheet_Battle_Cursor_Fight { get; private set; } = null!;
    public SpriteSheet Sheet_Battle_Hp_Overlay { get; private set; } = null!;

    public FontSystem Font_PowerGreen { get; private set; } = null!;
    public FontSystem Font_PowerClearBold { get; private set; } = null!;

    private readonly AssetPipeline _pipeline;

    public AssetsManager(GraphicsDevice graphicsDevice, ContentManager content, RomManager romManager)
    {
        _pipeline = new AssetPipeline();
        _pipeline.RegisterImporter(new ContentTextureImporter(content, graphicsDevice));
        _pipeline.RegisterImporter(new TextureImporter(romManager, graphicsDevice));

        _pipeline.RegisterProcessor(new SpriteProcessor());
    }

    public void LoadGlobalAssets(GraphicsDevice device)
    {
        var blankTexture = new Texture2D(device, 1, 1);
        blankTexture.SetData([Color.White]);

        Sprite_Blank = new Sprite(blankTexture);

        Sprite_Battle_Forest_Bg = Load<Sprite>(AssetSource.Content, "Textures/Battlepacks/forest_bg");
        Sprite_Battle_Grass_Base0 = Load<Sprite>(AssetSource.Content, "Textures/Battlepacks/grass_base0");
        Sprite_Battle_Grass_Base1 = Load<Sprite>(AssetSource.Content, "Textures/Battlepacks/grass_base1");

        Sprite_Battle_Databox = Load<Sprite>(AssetSource.Content, "Textures/UI/Battle/databox_normal");
        Sprite_Battle_Databox_Foe = Load<Sprite>(AssetSource.Content, "Textures/UI/Battle/databox_normal_foe");
        Sprite_Battle_Exp_Overlay = Load<Sprite>(AssetSource.Content, "Textures/UI/Battle/overlay_exp");
        Sprite_Battle_Overlay_Fight = Load<Sprite>(AssetSource.Content, "Textures/UI/Battle/overlay_fight");

        Sheet_Battle_Cursor_Action = new SpriteSheet(Load<Sprite>(AssetSource.Content, "Textures/UI/Battle/cursor_command"), 2, 10, null, null);
        Sheet_Battle_Cursor_Fight = new SpriteSheet(Load<Sprite>(AssetSource.Content, "Textures/UI/Battle/cursor_fight"), 2, 19, null, null);
        Sheet_Battle_Hp_Overlay = new SpriteSheet(Load<Sprite>(AssetSource.Content, "Textures/UI/Battle/overlay_hp"), 1, 3, null, null);

        Sprite_Dialogue_Overlay = Load<Sprite>(AssetSource.Content, "Textures/UI/Battle/overlay_message");

        Font_PowerGreen = new FontSystem();
        Font_PowerGreen.AddFont(File.ReadAllBytes("Content/Fonts/power_green.ttf"));

        Font_PowerClearBold = new FontSystem();
        Font_PowerClearBold.AddFont(File.ReadAllBytes("Content/Fonts/power_clear_bold.ttf"));
    }

    public T Load<T>(AssetSource source, object payload) where T : class
    {
        var assetRef = new AssetReference(source, payload);
        return _pipeline.LoadAsset<T>(assetRef);
    }
}