using System.Collections.Generic;
using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pokemon.DesktopGL.Core.Managers;

public class AssetsManager
{
    public Sprite Sprite_Blank { get; private set; }

    public Sprite Sprite_Battle_Forest_Bg { get; private set; }
    public Sprite Sprite_Battle_Grass_Base0 { get; private set; }
    public Sprite Sprite_Battle_Grass_Base1 { get; private set; }
    public Sprite Sprite_Battle_Databox { get; private set; }
    public Sprite Sprite_Battle_Databox_Foe { get; private set; }

    public SpriteSheet Sheet_Tileset_Outside { get; private set; }
    public SpriteSheet Sheet_Battle_Actions { get; private set; }

    public AnimationPack Anim_Character_Boy_Run { get; private set; }
    public AnimationPack Anim_Character_Boy_Idle { get; private set; }

    public AnimationPack Anim_Character_NPC_Run { get; private set; }
    public AnimationPack Anim_Character_NPC_Idle { get; private set; }

    public FontSystem Font_PowerGreen { get; private set; }

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

        Sheet_Tileset_Outside = SpriteSheet.FromDimension(LoadSprite("Tilesets/Outside"), 32, 32);
        Sheet_Battle_Actions = new SpriteSheet(LoadSprite("Textures/UI/Battle/cursor_command"), 2, 10);

        var sheet_boy_run = new SpriteSheet(LoadSprite("Textures/boy_run"), 4, 4);
        Anim_Character_Boy_Idle = new AnimationPack(new Dictionary<string, Animation>()
        {
            { "Down", Animation.FromSpriteSheet(sheet_boy_run, 0) },
            { "Left", Animation.FromSpriteSheet(sheet_boy_run, 4) },
            { "Right", Animation.FromSpriteSheet(sheet_boy_run, 8) },
            { "Up", Animation.FromSpriteSheet(sheet_boy_run, 12) },
        });

        Anim_Character_Boy_Run = new AnimationPack(new Dictionary<string, Animation>()
        {
            { "Down", Animation.FromSpriteSheet(sheet_boy_run, 0, 3) },
            { "Left", Animation.FromSpriteSheet(sheet_boy_run, 4, 7) },
            { "Right", Animation.FromSpriteSheet(sheet_boy_run, 8, 11) },
            { "Up", Animation.FromSpriteSheet(sheet_boy_run, 12, 15) },
        });

        var sheet_npc_run = new SpriteSheet(LoadSprite("Textures/Characters/NPC 12"), 4, 4);
        Anim_Character_NPC_Idle = new AnimationPack(new Dictionary<string, Animation>()
        {
            { "Down", Animation.FromSpriteSheet(sheet_npc_run, 0) },
            { "Left", Animation.FromSpriteSheet(sheet_npc_run, 4) },
            { "Right", Animation.FromSpriteSheet(sheet_npc_run, 8) },
            { "Up", Animation.FromSpriteSheet(sheet_npc_run, 12) },
        });

        Anim_Character_NPC_Run = new AnimationPack(new Dictionary<string, Animation>()
        {
            { "Down", Animation.FromSpriteSheet(sheet_npc_run, 0, 3) },
            { "Left", Animation.FromSpriteSheet(sheet_npc_run, 4, 7) },
            { "Right", Animation.FromSpriteSheet(sheet_npc_run, 8, 11) },
            { "Up", Animation.FromSpriteSheet(sheet_npc_run, 12, 15) },
        });

        Font_PowerGreen = new FontSystem();
        Font_PowerGreen.AddFont(File.ReadAllBytes("Content/Fonts/power_green.ttf"));
    }

    public Sprite LoadSprite(string assetName)
    {
        var texture = _content.Load<Texture2D>(assetName);
        return new Sprite(texture, null);
    }
}