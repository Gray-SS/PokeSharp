using ImGuiNET;
using PokeSharp.Assets;
using PokeSharp.Editor.UI;

namespace PokeSharp.Editor.Views;

public sealed class AssetsViewer : IGuiHook
{
    private readonly AssetPipeline _pipeline;

    public AssetsViewer(AssetPipeline pipeline)
    {
        _pipeline = pipeline;
    }

    public void DrawGui()
    {
        if (ImGui.Begin("Assets viewer"))
        {
            if (ImGui.CollapsingHeader("ROM"))
            {
                ImGui.Dummy(new(0, 5));
                ImGui.TextWrapped("All the assets from THE ROM");

                foreach (object asset in _pipeline.LoadedAssets)
                {
                    ImGui.Text($"{asset}");
                }
            }

            ImGui.End();
        }
    }
}