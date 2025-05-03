using System;
using System.Collections.Generic;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.Core.Graphics;

namespace Pokemon.DesktopGL.Core.Renderers;

public sealed class UIRenderer
{
    private readonly SpriteBatch _spriteBatch;

    public UIRenderer(GraphicsDevice device)
    {
        _spriteBatch = new SpriteBatch(device);
    }

    public void Begin()
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    }

    public void Draw(Sprite sprite, Rectangle bounds, Color color, float opacity = 1.0f)
    {
        _spriteBatch.Draw(sprite.Texture, bounds, sprite.SourceRect, color * opacity, 0.0f, Vector2.Zero, 0, 0.0f);
    }

    public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color)
    {
        _spriteBatch.DrawString(font, text, position, color);
    }

    public void DrawWrappedText(SpriteFontBase font, string text, Vector2 position, Color color, float maxWidth)
    {
        List<string> lines = WrapText(font, text, maxWidth);

        float yPosition = position.Y;

        foreach (string line in lines)
        {
            float xPosition = position.X;

            _spriteBatch.DrawString(font, line, new Vector2(xPosition, yPosition), color);
            yPosition += font.LineHeight;
        }
    }

    public void End()
    {
        _spriteBatch.End();
    }

    private static List<string> WrapText(SpriteFontBase font, string text, float maxWidth)
    {
        List<string> lines = [];

        if (string.IsNullOrEmpty(text))
            return lines;

        // Séparer le texte par les retours à la ligne existants
        string[] paragraphs = text.Split(["\r\n", "\n"], StringSplitOptions.None);

        foreach (string paragraph in paragraphs)
        {
            // Si le paragraphe est vide, ajouter une ligne vide
            if (string.IsNullOrEmpty(paragraph))
            {
                lines.Add(string.Empty);
                continue;
            }

            // Découper le paragraphe en mots
            string[] words = paragraph.Split(' ');
            string currentLine = string.Empty;

            foreach (string word in words)
            {
                // Mesurer la largeur de la ligne actuelle + le mot à ajouter
                string testLine = currentLine.Length == 0 ? word : currentLine + " " + word;
                Vector2 size = font.MeasureString(testLine);

                // Si la ligne dépasse la largeur maximale, on commence une nouvelle ligne
                if (size.X > maxWidth && currentLine.Length > 0)
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            // Ajouter la dernière ligne du paragraphe
            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);
        }

        return lines;
    }
}