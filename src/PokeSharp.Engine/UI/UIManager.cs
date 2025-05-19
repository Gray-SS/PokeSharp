using FontStashSharp;
using PokeSharp.Engine.Assets;
using PokeSharp.Engine.Graphics;

namespace PokeSharp.Engine.UI;

public sealed class UIManager
{
    public Sprite FrameBackground { get; set; } = null!;
    public FontSystem DefaultFont { get; set; } = null!;

    public static UIManager Instance => PokesharpEngine.Instance.UIManager;

    public void Initialize(AssetsManager assetsManager)
    {
        ArgumentNullException.ThrowIfNull(assetsManager, nameof(assetsManager));

        FrameBackground = assetsManager.Sprite_Button;
        DefaultFont = assetsManager.Font_PowerGreen;
    }
}