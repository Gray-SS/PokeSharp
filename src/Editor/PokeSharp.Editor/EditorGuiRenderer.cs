using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.ImGuiNet;
using PokeSharp.Assets;
using PokeSharp.Editor.Services;
using PokeSharp.Rendering;

namespace PokeSharp.Editor
{
    public sealed class EditorGuiRenderer : IRenderer
    {
        private static readonly ushort[] IconRanges =
        [
            0xF000, 0xF8FF, 0
        ];


        private bool _rebuildFontAtlas;
        private readonly IEditorViewManager _dispatcher;
        private readonly IGuiResourceManager _resManager;
        private readonly ImGuiRenderer _imGuiRenderer;

        public EditorGuiRenderer(
            AssetPipeline assetPipeline,
            ImGuiRenderer renderer,
            IGuiResourceManager resManager,
            IEditorViewManager dispatcher)
        {
            _dispatcher = dispatcher;
            _imGuiRenderer = renderer;
            _resManager = resManager;

            assetPipeline.AssetImported += (s, e) => _rebuildFontAtlas = true;
            ConfigureImGui();
        }

        private unsafe void ConfigureImGui()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            ImFontConfigPtr config = ImGuiNative.ImFontConfig_ImFontConfig();
            config.MergeMode = false;
            config.PixelSnapH = true;
            string interPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts", "inter_medium.ttf");
            ImFontPtr fontMedium = io.Fonts.AddFontFromFileTTF(interPath, 18.0f, config);
            if (fontMedium.NativePtr == null)
                throw new InvalidOperationException($"Couldn't load inter font at path '{interPath}'");

            _resManager.RegisterFont("medium", fontMedium);
            ImGuiNative.ImFontConfig_destroy(config.NativePtr);

            ImFontConfigPtr iconConfig = ImGuiNative.ImFontConfig_ImFontConfig();
            iconConfig.MergeMode = true;
            iconConfig.PixelSnapH = true;
            fixed (ushort* rangesPtr = IconRanges)
            {
                string faPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts", "fa-solid-900.ttf");
                ImFontPtr fa = io.Fonts.AddFontFromFileTTF(faPath, 18.0f, iconConfig, (IntPtr)rangesPtr);
                if (fa.NativePtr == null)
                    throw new InvalidOperationException($"Couldn't load font awesome icons at path '{faPath}'");
            }

            ImGuiNative.ImFontConfig_destroy(iconConfig.NativePtr);

            _imGuiRenderer.RebuildFontAtlas();
        }

        public void Draw(GameTime gameTime)
        {
            _imGuiRenderer.BeforeLayout(gameTime);
            _dispatcher.Draw();
            _imGuiRenderer.AfterLayout();
        }
    }
}