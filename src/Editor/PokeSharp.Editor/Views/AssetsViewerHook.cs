using ImGuiNET;
using PokeSharp.Editor.UI;

namespace PokeSharp.Editor.Views;

public sealed class AssetsViewerHook : IImGuiHook
{
    public void DrawGui()
    {
        if (ImGui.Begin("Assets viewer"))
        {

            ImGui.End();
        }
    }
}
