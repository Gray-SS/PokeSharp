using ImGuiNET;
using PokeSharp.Editor.UI;

namespace PokeSharp.Editor.Views;

public sealed class AssetsViewer : IImGuiHook
{
    public void DrawGui()
    {
        if (ImGui.Begin("Assets viewer"))
        {
            ImGui.TextWrapped("Hello from the editor !");

            ImGui.End();
        }
    }
}