using FontStashSharp;
using Microsoft.Xna.Framework;
using PokeSharp.Engine.Inputs;
using PokeSharp.Engine.Renderers;

namespace PokeSharp.Engine.UI;

public sealed class UIButton : UINode
{
    public string Text { get; set; } = "";
    public float FontSize { get; set; } = 16f;
    public FontSystem Font { get; set; }
    public Color TextColor { get; set; } = Color.White;
    public TextAlignment TextAlignment { get; set; }
    public TextVerticalAlignment VerticalTextAlignment { get; set; }

    public EventHandler<EventArgs>? OnMouseEnter;
    public EventHandler<EventArgs>? OnMouseLeave;
    public EventHandler<EventArgs>? OnMouseHover;
    public EventHandler<EventArgs>? OnClicked;

    public UIButton()
    {
        Font = PokesharpEngine.Instance.UIManager.DefaultFont;

        TextAlignment = TextAlignment.Center;
        VerticalTextAlignment = TextVerticalAlignment.Center;
        TextColor = Color.Black;
        BackgroundColor = Color.White;
        Padding = new UIThickness(10, 0);
    }

    private bool _hovered;

    public override void Update(GameTime gameTime)
    {
        var mousePos = PokesharpEngine.Instance.InputManager.MousePosition;
        bool inside = Bounds.Contains(mousePos);

        if (inside && !_hovered)
            OnMouseEnter?.Invoke(this, EventArgs.Empty);
        else if (!inside && _hovered)
            OnMouseLeave?.Invoke(this, EventArgs.Empty);

        if (inside)
        {
            OnMouseHover?.Invoke(this, EventArgs.Empty);

            if (PokesharpEngine.Instance.InputManager.IsMouseButtonPressed(MouseButtonType.Left))
                OnClicked?.Invoke(this, EventArgs.Empty);
        }

        _hovered = inside;
    }

    public override void Draw(UIRenderer renderer)
    {
        base.Draw(renderer);

        if (Font != null && !string.IsNullOrEmpty(Text))
        {
            var bounds = ApplyPadding(Bounds);
            var font = Font.GetFont(FontSize);
            var size = font.MeasureString(Text);

            Vector2 textPosition = bounds.Location.ToVector2();

            switch (TextAlignment)
            {
                case TextAlignment.Center:
                    textPosition.X += (bounds.Width - size.X) * 0.5f;
                    break;
                case TextAlignment.Right:
                    textPosition.X = bounds.Right - size.X;
                    break;
            }

            switch (VerticalTextAlignment)
            {
                case TextVerticalAlignment.Center:
                    textPosition.Y += (bounds.Height - size.Y) * 0.5f;
                    break;
                case TextVerticalAlignment.Bottom:
                    textPosition.Y = bounds.Bottom - size.Y;
                    break;
            }

            renderer.DrawString(font, Text, textPosition, TextColor);
        }
    }

    protected override int ComputeAutoWidth(int parentSize)
    {
        if (Font == null || string.IsNullOrEmpty(Text))
            return 0;

        var font = Font.GetFont(FontSize);
        var size = font.MeasureString(Text);
        return (int)MathF.Ceiling(size.X) + Padding.Horizontal;
    }

    protected override int ComputeAutoHeight(int parentSize)
    {
        if (Font == null || string.IsNullOrEmpty(Text))
            return 0;

        var font = Font.GetFont(FontSize);
        var size = font.MeasureString(Text);
        return (int)MathF.Ceiling(size.X) + Padding.Horizontal;
    }
}
