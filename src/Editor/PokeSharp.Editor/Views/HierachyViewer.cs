using ImGuiNET;

namespace PokeSharp.Editor.Views;

public sealed class HierarchyViewer : IGuiHook
{
    public void DrawGui()
    {
        if (ImGui.Begin("Hierachy"))
        {

            ImGui.End();
        }
    }
}