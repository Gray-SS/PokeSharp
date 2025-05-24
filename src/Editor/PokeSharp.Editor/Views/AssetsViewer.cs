using ImGuiNET;
using NativeFileDialogSharp;
using PokeSharp.Assets;
using PokeSharp.Editor.UI;

namespace PokeSharp.Editor.Views;

public sealed class AssetsViewer : IGuiHook
{
    private readonly AssetPipeline _pipeline;

    private bool _openErrorPopup = false;
    private string? _errorMessage;

    public AssetsViewer(AssetPipeline pipeline)
    {
        _pipeline = pipeline;
    }

    public void DrawGui()
    {
        if (ImGui.Begin("Assets viewer", ImGuiWindowFlags.MenuBar))
        {
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Load asset"))
                    {
                        DialogResult result = Dialog.FileOpen(defaultPath: Environment.CurrentDirectory);
                        if (result.IsOk)
                        {
                            try
                            {
                                _pipeline.Load(result.Path);
                            }
                            catch (Exception ex)
                            {
                                _errorMessage = ex.Message;
                                _openErrorPopup = true;
                            }
                        }
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMenuBar();
            }

            foreach (object asset in _pipeline.LoadedAssets)
            {
                ImGui.TextUnformatted(asset.GetType().Name);
            }

            ImGui.End();
        }

        if (_openErrorPopup)
        {
            ImGui.OpenPopup("error");
            _openErrorPopup = false;
        }

        if (ImGui.BeginPopupModal("error"))
        {
            ImGui.TextWrapped(_errorMessage ?? "An unknown error occurred.");
            ImGui.Separator();
            if (ImGui.Button("Close"))
            {
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
    }
}
