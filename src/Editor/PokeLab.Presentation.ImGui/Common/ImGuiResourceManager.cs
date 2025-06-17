using ImGuiNET;
using MonoGame.ImGuiNet;
using Microsoft.Xna.Framework.Graphics;
using PokeCore.Logging;

namespace PokeLab.Presentation.ImGui.Common;

public sealed class GuiResourceManager : IGuiResourceManager
{
    private readonly Logger _logger;
    private readonly ImGuiRenderer _renderer;
    private readonly Dictionary<Texture2D, nint> _textures;
    private readonly Dictionary<string, ImFontPtr> _fonts;

    public GuiResourceManager(ImGuiRenderer renderer, Logger<GuiResourceManager> logger)
    {
        _logger = logger;
        _renderer = renderer;
        _textures = new Dictionary<Texture2D, nint>();
        _fonts = new Dictionary<string, ImFontPtr>();
    }

    public void ClearFonts()
    {
        _fonts.Clear();
    }

    public void RegisterFont(string fontName, ImFontPtr font)
    {
        _logger.Trace($"Registering font with name '{fontName}'");

        if (_fonts.ContainsKey(fontName))
        {
            _logger.Warn($"A font with name '{fontName}' is already registered. Skipping.");
            return;
        }

        _fonts.Add(fontName, font);
    }

    public nint RegisterTexture(Texture2D texture)
    {
        _logger.Trace($"Registering texture '{texture.Name}'");

        if (_textures.TryGetValue(texture, out nint value))
        {
            _logger.Warn($"The texture '{texture.Name}' is already registered. Skipping.");
            return value;
        }

        nint id = _renderer.BindTexture(texture);
        _textures.Add(texture, id);

        _logger.Info($"Texture '{texture.Name}' has been registered successfully with id '{id}'.");
        return id;
    }

    public ImFontPtr GetFont(string fontName)
    {
        if (!_fonts.TryGetValue(fontName, out ImFontPtr font))
        {
            _logger.Error($"The font '{fontName}' is not registered. Returning null pointer.");
            return default;
        }

        return font;
    }

    public nint GetTexture(Texture2D texture)
    {
        if (!_textures.TryGetValue(texture, out nint id))
        {
            _logger.Error($"The texture '{texture.Name}' is not registered. Returning null pointer.");
            return _textures.Values.FirstOrDefault();
        }

        return id;
    }

}