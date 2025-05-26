using ImGuiNET;
using NativeFileDialogSharp;
using PokeSharp.Assets;
using PokeSharp.Assets.VFS;
using PokeSharp.Scenes;

namespace PokeSharp.Editor.Views;

public sealed class ContentViewer : IGuiHook
{
    private readonly AssetPipeline _pipeline;
    private readonly IVirtualFileSystem _vfs;

    public ContentViewer(AssetPipeline pipeline, IVirtualFileSystem vfs)
    {
        _vfs = vfs;
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
                            _pipeline.Load(result.Path);
                        }
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMenuBar();
            }

            if (ImGui.BeginPopupContextWindow())
            {
                if (ImGui.BeginMenu("New"))
                {
                    if (ImGui.MenuItem("Scene"))
                    {
                        var scene = new Scene("NewScene");
                    }
                }

                ImGui.EndPopup();
            }

            foreach (IVirtualDirectory directory in _vfs.GetMountedDirectories())
            {
                ImGui.Text($"{directory.Name}");
                ImGui.Text($"{directory.VirtualPath}");
            }

            ImGui.End();
        }
    }
}
