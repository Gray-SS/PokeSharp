using FontStashSharp;
using Microsoft.Xna.Framework;
using PokeSharp.Engine.Renderers;

namespace PokeSharp.Engine.UI;

public enum TextAlignment
{
    Left, Center, Right
}

public enum TextVerticalAlignment
{
    Top, Center, Bottom
}

public sealed class UILabel : UINode
{
    public string Text { get; set; }
    public float FontSize { get; set; }
    public FontSystem Font { get; set; }
    public Color TextColor { get; set; } = Color.Black;
    public TextAlignment Alignment { get; set; } = TextAlignment.Left;
    public TextVerticalAlignment VerticalAlignment { get; set; } = TextVerticalAlignment.Top;

    private Vector2 _textPos;

    public UILabel()
    {
        Font = UIManager.Instance.DefaultFont;
        FontSize = 16.0f;

        Text = string.Empty;
        Alignment = TextAlignment.Center;
        VerticalAlignment = TextVerticalAlignment.Center;
        BackgroundColor = Color.Transparent;
    }

    public override void Arrange(Rectangle remainingSpace)
    {
        base.Arrange(remainingSpace);

        SpriteFontBase font = Font.GetFont(FontSize);
        Vector2 size = font.MeasureString(Text);

        _textPos.X = Alignment switch
        {
            TextAlignment.Left => Bounds.Left,
            TextAlignment.Center => Bounds.Left + (Bounds.Width - size.X) * 0.5f,
            TextAlignment.Right => Bounds.Right - size.X,
            _ => throw new NotSupportedException($"Layout error: Text alignment '{Alignment}' is currently not supported.")
        };

        _textPos.Y = VerticalAlignment switch
        {
            TextVerticalAlignment.Top => Bounds.Top,
            TextVerticalAlignment.Center => Bounds.Top + (Bounds.Height - size.Y) * 0.5f,
            TextVerticalAlignment.Bottom => Bounds.Bottom - size.Y,
            _ => throw new NotSupportedException($"Layout error: Vertical text alignment '{VerticalAlignment}' is currently not supported.")
        };
    }

    protected override int ComputeAutoWidth(int parentSize)
    {
        SpriteFontBase font = Font.GetFont(FontSize);
        Vector2 size = font.MeasureString(Text);

        return (int)size.X;
    }

    protected override int ComputeAutoHeight(int parentSize)
    {
        SpriteFontBase font = Font.GetFont(FontSize);
        Vector2 size = font.MeasureString(Text);

        return (int)size.Y;
    }

    public override void Draw(UIRenderer renderer)
    {
        base.Draw(renderer);

        SpriteFontBase font = Font.GetFont(FontSize);
        renderer.DrawString(font, Text, _textPos, TextColor);
    }
}