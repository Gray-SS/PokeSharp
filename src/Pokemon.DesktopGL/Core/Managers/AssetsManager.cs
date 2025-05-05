using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.Core.Graphics;

namespace Pokemon.DesktopGL.Core.Managers;

public class AssetsManager
{
    public Sprite Sprite_Blank { get; private set; }

    public Sprite Sprite_Battle_Forest_Bg { get; private set; }
    public Sprite Sprite_Battle_Grass_Base0 { get; private set; }
    public Sprite Sprite_Battle_Grass_Base1 { get; private set; }
    public Sprite Sprite_Battle_Databox { get; private set; }
    public Sprite Sprite_Battle_Databox_Foe { get; private set; }
    public Sprite Sprite_Dialogue_Overlay { get; private set; }
    public Sprite Sprite_Battle_Exp_Overlay { get; private set; }

    public Sprite Sprite_Fade_Transition { get; private set; }

    public SpriteSheet Sheet_Battle_Actions { get; private set; }
    public SpriteSheet Sheet_Battle_Hp_Overlay { get; private set; }

    public FontSystem Font_PowerGreen { get; private set; }
    public FontSystem Font_PowerClearBold { get; private set; }

    private readonly ContentManager _content;
    private readonly GraphicsDevice _graphicsDevice;

    public AssetsManager(GraphicsDevice graphics, ContentManager contentManager)
    {
        _content = contentManager;
        _graphicsDevice = graphics;
    }

    public void LoadGlobalAssets()
    {
        var blankTexture = new Texture2D(_graphicsDevice, 1, 1);
        blankTexture.SetData([ Color.White ]);

        Sprite_Blank = new Sprite(blankTexture);

        Sprite_Battle_Forest_Bg = LoadSprite("Textures/Battlepacks/forest_bg");
        Sprite_Battle_Grass_Base0 = LoadSprite("Textures/Battlepacks/grass_base0");
        Sprite_Battle_Grass_Base1 = LoadSprite("Textures/Battlepacks/grass_base1");

        Sprite_Battle_Databox = LoadSprite("Textures/UI/Battle/databox_normal");
        Sprite_Battle_Databox_Foe = LoadSprite("Textures/UI/Battle/databox_normal_foe");
        Sprite_Battle_Exp_Overlay = LoadSprite("Textures/UI/Battle/overlay_exp");

        Sheet_Battle_Actions = new SpriteSheet(LoadSprite("Textures/UI/Battle/cursor_command"), 2, 10, null, null);
        Sheet_Battle_Hp_Overlay = new SpriteSheet(LoadSprite("Textures/UI/Battle/overlay_hp"), 1, 3, null, null);

        Sprite_Dialogue_Overlay = LoadSprite("Textures/UI/Battle/overlay_message");

        Font_PowerGreen = new FontSystem();
        Font_PowerGreen.AddFont(File.ReadAllBytes("Content/Fonts/power_green.ttf"));

        Font_PowerClearBold = new FontSystem();
        Font_PowerClearBold.AddFont(File.ReadAllBytes("Content/Fonts/power_clear_bold.ttf"));
    }

    public Sprite LoadSprite(string assetName)
    {
        var texture = _content.Load<Texture2D>(assetName);
        return new Sprite(texture, null);
    }
}