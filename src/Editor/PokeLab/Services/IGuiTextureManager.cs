using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace PokeLab.Services;

public interface IGuiResourceManager
{
    void ClearFonts();

    nint RegisterTexture(Texture2D texture);
    void RegisterFont(string fontName, ImFontPtr font);

    nint GetTexture(Texture2D texture);
    ImFontPtr GetFont(string fontName);
}