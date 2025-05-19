using Microsoft.Xna.Framework;
using PokeSharp.Engine.Renderers;

namespace PokeSharp.Engine.UI;

public enum UIStackDirection
{
    Horizontal,
    Vertical
}

public enum UIFlexJustify
{
    Start, Center, End, SpaceBetween, SpaceAround, SpaceEvenly
}

public enum UIFlexAlign
{
    Start, Center, End, Stretch
}

public class UIStackLayout : UIContainer
{
    public int ItemSpacing { get; set; } = 0;
    public UIFlexAlign AlignItems { get; set; } = UIFlexAlign.Stretch;
    public UIFlexJustify JustifyContent { get; set; } = UIFlexJustify.Start;
    public UIStackDirection Direction { get; set; } = UIStackDirection.Vertical;

    public UIStackLayout()
    {
        BackgroundColor = Color.Transparent;
    }

    public override void Arrange(Rectangle bounds)
    {
        if (!IsVisible) return;

        base.Arrange(bounds);
    }

    public override void Draw(UIRenderer renderer)
    {
        if (!IsVisible) return;

        base.Draw(renderer);

        foreach (var child in Children)
        {
            if (child.IsVisible)
            {
                child.Draw(renderer);
            }
        }
    }
}