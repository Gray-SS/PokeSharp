using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace PokeSharp.Editor.Services;

public interface IGuiResourceManager
{
    nint RegisterTexture(Texture2D texture);
    void RegisterFont(string fontName, ImFontPtr font);

    nint GetTexture(Texture2D texture);
    ImFontPtr GetFont(string fontName);
}