using ImGuiNET;
using PokeSharp.Assets.VFS;
using PokeSharp.Editor.Services;

namespace PokeSharp.Editor.Views;

public sealed class InspectorViewer : IGuiHook
{
    private Type? _selectedObjType;
    private readonly ISelectionManager _selectionService;

    public InspectorViewer(ISelectionManager selectionService)
    {
        _selectionService = selectionService;
        _selectionService.SelectionChanged += OnSelectionChanged;
    }

    // TODO: Yeah, I clearly need to create an event args for this, but you know, I was lazy
    private void OnSelectionChanged(object? sender, object? e)
    {
        _selectedObjType = e?.GetType();
    }

    public void DrawGui()
    {
        if (ImGui.Begin("Inspector"))
        {
            if (!_selectionService.HasSelection)
            {
                ImGui.Text("No object currently selected");
            }
            else
            {
                ImGui.Text($"Type: {_selectedObjType!.Name}");
                if (_selectionService.SelectedObject is IVirtualFile file)
                {
                    ImGui.Text($"File name: {file.Name}");
                }
            }

            ImGui.End();
        }
    }
}