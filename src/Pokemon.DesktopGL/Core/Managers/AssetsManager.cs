using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pokemon.DesktopGL.Core.Managers;

public class AssetsManager
{
    public Sprite Sprite_Battle_Forest_Bg { get; private set; }
    public Sprite Sprite_Battle_Grass_Base0 { get; private set; }
    public Sprite Sprite_Battle_Grass_Base1 { get; private set; }
    public Sprite Sprite_Battle_Databox { get; private set; }
    public Sprite Sprite_Battle_Databox_Foe { get; private set; }

    public SpriteSheet Sheet_Tileset_Outside { get; private set; }
    public AnimationPack Anim_Character_Boy_Run { get; private set; }
    public AnimationPack Anim_Character_Boy_Idle { get; private set; }

    private ContentManager _content;

    public AssetsManager(ContentManager contentManager)
    {
        _content = contentManager;
    }

    public void LoadGlobalAssets()
    {
        Sprite_Battle_Forest_Bg = LoadSprite("Textures/Battlepacks/forest_bg");
        Sprite_Battle_Grass_Base0 = LoadSprite("Textures/Battlepacks/grass_base0");
        Sprite_Battle_Grass_Base1 = LoadSprite("Textures/Battlepacks/grass_base1");

        Sprite_Battle_Databox = LoadSprite("Textures/UI/Battle/databox_normal");
        Sprite_Battle_Databox_Foe = LoadSprite("Textures/UI/Battle/databox_normal_foe");

        Sheet_Tileset_Outside = SpriteSheet.FromDimension(LoadSprite("Tilesets/Outside"), 64, 64);

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
    }

    private Sprite LoadSprite(string assetName)
    {
        var texture = _content.Load<Texture2D>(assetName);
        return new Sprite(texture, null);
    }
}