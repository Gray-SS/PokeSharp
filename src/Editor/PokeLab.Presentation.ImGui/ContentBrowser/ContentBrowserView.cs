using FontAwesome;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using PokeCore.IO;
using PokeLab.Presentation.ContentBrowser;
using PokeLab.Presentation.ImGui.Common;
using PokeLab.Presentation.ImGui.Helpers;

namespace PokeLab.Presentation.ImGui.ContentBrowser;

public sealed class ContentBrowserView : View<ContentBrowserViewModel>
{
    private readonly Dictionary<string, nint> _icons;

    public ContentBrowserView(
        ContentBrowserViewModel viewModel,
        IGuiResourceManager resourceManager,
        GraphicsDevice graphicsDevice
    ) : base(viewModel)
    {
        _icons = new Dictionary<string, nint>()
        {
            ["folder"] = LoadAndRegisterTexture(resourceManager, graphicsDevice, AppContext.BaseDirectory, "Resources", "folder.png"),
            ["file"] = LoadAndRegisterTexture(resourceManager, graphicsDevice, AppContext.BaseDirectory, "Resources", "file.png"),
            ["invalid-asset"] = LoadAndRegisterTexture(resourceManager, graphicsDevice, AppContext.BaseDirectory, "Resources", "question-mark.png")
        };
    }

    private static nint LoadAndRegisterTexture(IGuiResourceManager resourceManager, GraphicsDevice device, params string[] paths)
    {
        string path = Path.Combine(paths);
        Texture2D texture = Texture2D.FromFile(device, path);

        return resourceManager.RegisterTexture(texture);
    }

    public override void Render()
    {
        if (Gui.Begin("Content Browser"))
        {
            DrawNavigationBar();

            if (ViewModel.IsLoading)
            {
                var cursor = Gui.GetCursorPos();
                var availSize = Gui.GetContentRegionAvail();

                var pos = cursor + availSize * 0.5f - Gui.GetStyle().WindowPadding;
                Gui.SetCursorPos(pos);
                GuiHelper.LoadingSpinner(20f, 3, Color.White);
            }
        }

        Gui.End();
    }

    private void DrawNavigationBar()
    {
        Gui.PushStyleVar(ImGuiStyleVar.WindowPadding, new NVec2(8));
        if (Gui.BeginChild("##navbar", new NVec2(0, 40), ImGuiChildFlags.Borders))
        {
            if (Gui.Button($"{FontAwesomeIcons.Plus} New"))
            {
                Gui.OpenPopup("AddPopup");
            }

            if (Gui.BeginItemTooltip())
            {
                Gui.EndTooltip();
            }

            Gui.SameLine();
            if (Gui.Button($"{FontAwesomeIcons.FileImport}  Import"))
            { }

            if (Gui.BeginItemTooltip())
            {
                Gui.EndTooltip();
            }

            Gui.SameLine();
            bool canSaveProject = ViewModel.SaveCommand.CanExecute(null);
            if (GuiHelper.Button($"{FontAwesomeIcons.FloppyDisk}  Save", canSaveProject))
                ExecuteBackground(ViewModel.SaveCommand);

            if (Gui.BeginItemTooltip())
            {
                if (!canSaveProject)
                {
                    Gui.Text("Save the project");
                    Gui.Text("This action is currently disabled as no project is currently loaded.");
                }
                else
                {
                    Gui.Text("Save the project");
                }
                Gui.EndTooltip();
            }

            Gui.SameLine(0, 20f);
            DrawNavBackButton();

            Gui.SameLine();
            DrawNavForwardButton();

            Gui.SameLine(0, 20f);
            float size = Gui.GetContentRegionAvail().Y;
            Gui.Image(_icons["folder"], new(size, size));
            Gui.SameLine();

            DrawFoldersBreadcrumb();

            Gui.EndChild();
        }
        Gui.PopStyleVar();
    }

    private void DrawFoldersBreadcrumb()
    {
        List<string> segments = [string.Empty];
        // segments.AddRange(_navigator.CurrentPath.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries));

        VirtualPath current = null!;
        for (int i = 0; i < 0; i++)
        {
            string segment = segments[i];
            // if (segment == string.Empty) current = VirtualPath.BuildRoot(_navigator.CurrentPath.Scheme);
            // else current = current.Combine(segment + "/");

            string displayName = current.IsRoot ? "Root" : segment;

            if (Gui.Button(displayName))
            {
                // _navigator.NavigateTo(current);
            }

            if (Gui.BeginDragDropTarget())
            {
                // if (ImGuiHelper.AcceptDragDropPayload("MOVE_ENTRY", out IVirtualEntry? toMoveEntry))
                //     _assetPipeline.TryMove(toMoveEntry.Path, current);

                Gui.EndDragDropTarget();
            }

            if (i < segments.Count - 1)
            {
                Gui.SameLine();
                Gui.Text(">");
                Gui.SameLine();
            }
        }
    }

    private void DrawNavForwardButton()
    {
        bool isActive = true; /* _navigator.CanNavigateForward(); */
        if (Gui.ArrowButton("##forward", ImGuiDir.Right) && isActive)
        {
            // _navigator.NavigateForward();
        }
    }

    private void DrawNavBackButton()
    {
        bool isActive = true; /* _navigator.CanNavigateBack(); */
        if (Gui.ArrowButton("##back", ImGuiDir.Left) && isActive)
        {
            // _navigator.NavigateBack();
        }
    }
}