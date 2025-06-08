using ImGuiNET;
using NativeFileDialogSharp;
using PokeSharp.Core.Annotations;
using PokeSharp.Editor.Services;

using NVec2 = System.Numerics.Vector2;

namespace PokeSharp.Editor.Views;

[Priority(999)]
public sealed class MainViewer : IEditorView
{
    private readonly IProjectManager _projectManager;

    private string _projectName = string.Empty;
    private string _projectPath = string.Empty;
    private string? _formError = null;
    private bool _showCreateProjectPopup = false;

    public MainViewer(IProjectManager projectManager)
    {
        _projectManager = projectManager;

    }

    public void DrawGui()
    {
        DrawDockspace();
    }

    private void DrawDockspace()
    {
        ImGuiWindowFlags flags = GetDockspaceSettings();
        if (ImGui.Begin("DockSpace Root", flags))
        {
            ImGui.PopStyleVar(3);

            DrawMainMenuBar();

            uint dockspaceId = ImGui.GetID("MyDockspace");
            ImGui.DockSpace(dockspaceId, NVec2.Zero, ImGuiDockNodeFlags.None);

            DrawCreatePopup();
        }

        ImGui.End();
    }

    private static ImGuiWindowFlags GetDockspaceSettings()
    {
        ImGuiViewportPtr viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        ImGui.SetNextWindowViewport(viewport.ID);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, NVec2.Zero);

        return ImGuiWindowFlags.NoDocking |
               ImGuiWindowFlags.NoTitleBar |
               ImGuiWindowFlags.NoCollapse |
               ImGuiWindowFlags.NoResize |
               ImGuiWindowFlags.NoMove |
               ImGuiWindowFlags.NoBringToFrontOnFocus |
               ImGuiWindowFlags.NoNavFocus;
    }

    private void DrawMainMenuBar()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New project"))
                {
                    _showCreateProjectPopup = true;
                }

                if (ImGui.MenuItem("Open project"))
                {
                    var result = Dialog.FileOpen("pkproj");
                    if (result.IsOk)
                    {
                        _projectManager.TryOpenProject(result.Path, out _);
                    }
                }

                if (ImGui.MenuItem("Delete project"))
                {
                    var result = Dialog.FileOpen("pkproj");
                    if (result.IsOk)
                    {
                        _projectManager.TryDeleteProject(result.Path);
                    }
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Exit"))
                {
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }

    private void DrawCreatePopup()
    {
        if (_showCreateProjectPopup)
        {
            ImGui.OpenPopup("Create Project");
            _showCreateProjectPopup = false;
        }

        var center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new NVec2(0.5f, 0.5f));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new NVec2(20, 20));

        if (ImGui.BeginPopup("Create Project", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.Dummy(new(0, 5));
            ImGui.Text("Create a new project");
            ImGui.Separator();
            ImGui.Dummy(new(0, 10));

            ImGui.Text("Project Name");
            ImGui.SameLine();
            ImGui.InputText("##project_name", ref _projectName, 256);
            ImGui.Dummy(new(0, 5));

            ImGui.Text("Project Path");
            ImGui.SameLine();
            ImGui.InputText("##project_path", ref _projectPath, 256);
            ImGui.SameLine();
            ImGui.Dummy(new(5, 0));
            ImGui.SameLine();
            if (ImGui.Button("Browse"))
            {
                var result = Dialog.FolderPicker();
                if (result.IsOk)
                    _projectPath = result.Path;
            }
            ImGui.Dummy(new(0, 5));

            if (!string.IsNullOrEmpty(_formError))
            {
                // Display error
                ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(1, 0.2f, 0.2f, 1));
                ImGui.TextWrapped(_formError);
                ImGui.PopStyleColor();
                ImGui.Spacing();
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            float buttonWidth = 100f;
            float spacing = 10f;
            float totalWidth = buttonWidth * 2 + spacing;
            float cursorX = (ImGui.GetWindowSize().X - totalWidth) * 0.5f;

            ImGui.SetCursorPosX(cursorX);

            if (ImGui.Button("Create", new NVec2(buttonWidth, 0)))
            {
                if (string.IsNullOrWhiteSpace(_projectName) || string.IsNullOrWhiteSpace(_projectPath))
                {
                    _formError = "Project name and path must not be empty.";
                }
                else
                {
                    if (!_projectManager.TryCreateProject(_projectName, _projectPath, true, out _))
                    {
                        _formError = "Failed to create project. Check the console window to see more details.";
                    }
                    else
                    {
                        _formError = null;
                        ImGui.CloseCurrentPopup();
                    }
                }
            }

            ImGui.SameLine();

            if (ImGui.Button("Cancel", new NVec2(buttonWidth, 0)))
            {
                _formError = null;
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        ImGui.PopStyleVar();
    }
}