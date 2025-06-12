using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.ImGuiNet;
using PokeCore.Logging;
using PokeEngine.Rendering;
using PokeLab.Application.Commands;
using PokeLab.Application.Common;
using PokeLab.Application.ProjectManagement;
using PokeLab.Presentation.Common;
using PokeLab.Presentation.ImGui.Common;

namespace PokeLab.Presentation.ImGui;

public sealed class PokeLabImGuiRenderer : IRenderer
{
    private static readonly ushort[] IconRanges =
    [
        0xF000, 0xF8FF, 0
    ];

    private string _projectName = string.Empty;
    private string _projectPath = string.Empty;
    private string? _formError = null;
    private bool _showCreateProjectPopup = false;

    private readonly Logger _logger;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IWindowService _windowService;
    private readonly IEditorViewManager _viewManager;
    private readonly IGuiResourceManager _resManager;
    private readonly ImGuiRenderer _imGuiRenderer;

    public PokeLabImGuiRenderer(
        Logger<PokeLabImGuiRenderer> logger,
        ICommandDispatcher commandDispatcher,
        IWindowService windowService,
        ImGuiRenderer renderer,
        IGuiResourceManager resManager,
        IEditorViewManager viewManager)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
        _windowService = windowService;
        _viewManager = viewManager;
        _imGuiRenderer = renderer;
        _resManager = resManager;

        ConfigureImGui();
    }

    private unsafe void ConfigureImGui()
    {
        ImGuiIOPtr io = Gui.GetIO();
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

        DrawDockspace();
        _viewManager.RenderViews();

        _imGuiRenderer.AfterLayout();
    }

    private void DrawDockspace()
    {
        ImGuiWindowFlags flags = GetDockspaceSettings();
        if (Gui.Begin("DockSpace Root", flags))
        {
            Gui.PopStyleVar(3);

            DrawMainMenuBar();

            uint dockspaceId = Gui.GetID("MyDockspace");
            Gui.DockSpace(dockspaceId, NVec2.Zero, ImGuiDockNodeFlags.None);

            DrawCreatePopup();
        }

        Gui.End();
    }

    private static ImGuiWindowFlags GetDockspaceSettings()
    {
        ImGuiViewportPtr viewport = Gui.GetMainViewport();
        Gui.SetNextWindowPos(viewport.WorkPos);
        Gui.SetNextWindowSize(viewport.WorkSize);
        Gui.SetNextWindowViewport(viewport.ID);

        Gui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        Gui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        Gui.PushStyleVar(ImGuiStyleVar.WindowPadding, NVec2.Zero);

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
        if (Gui.BeginMainMenuBar())
        {
            if (Gui.BeginMenu("File"))
            {
                if (Gui.MenuItem("New project"))
                {
                    _showCreateProjectPopup = true;
                }

                if (Gui.MenuItem("Open project"))
                {
                    _ = OpenProjectAsync();
                }

                // if (Gui.MenuItem("Delete project"))
                // {
                //     var result = Dialog.FileOpen("pkproj");
                //     if (result.IsOk)
                //     {
                //         _projectManager.TryDeleteProject(result.Path);
                //     }
                // }

                Gui.Separator();

                if (Gui.MenuItem("Exit"))
                {
                    _ = _commandDispatcher.ExecuteAsync(new ExitCommand());
                }

                Gui.EndMenu();
            }

            Gui.EndMainMenuBar();
        }
    }

    private void DrawCreatePopup()
    {
        if (_showCreateProjectPopup)
        {
            Gui.OpenPopup("Create Project");
            _showCreateProjectPopup = false;
        }

        var center = Gui.GetMainViewport().GetCenter();
        Gui.SetNextWindowPos(center, ImGuiCond.Appearing, new NVec2(0.5f, 0.5f));
        Gui.PushStyleVar(ImGuiStyleVar.WindowPadding, new NVec2(20, 20));

        if (Gui.BeginPopup("Create Project", ImGuiWindowFlags.AlwaysAutoResize))
        {
            Gui.Dummy(new(0, 5));
            Gui.Text("Create a new project");
            Gui.Separator();
            Gui.Dummy(new(0, 10));

            Gui.Text("Project Name");
            Gui.SameLine();
            Gui.InputText("##project_name", ref _projectName, 256);
            Gui.Dummy(new(0, 5));

            Gui.Text("Project Path");
            Gui.SameLine();
            Gui.InputText("##project_path", ref _projectPath, 256);
            Gui.SameLine();
            Gui.Dummy(new(5, 0));
            Gui.SameLine();
            if (Gui.Button("Browse"))
            {
                _ = OpenBrowseDirectoryAsync();
            }
            Gui.Dummy(new(0, 5));

            if (!string.IsNullOrEmpty(_formError))
            {
                // Display error
                Gui.PushStyleColor(ImGuiCol.Text, new NVec4(1, 0.2f, 0.2f, 1));
                Gui.TextWrapped(_formError);
                Gui.PopStyleColor();
                Gui.Spacing();
            }

            Gui.Spacing();
            Gui.Separator();
            Gui.Spacing();

            float buttonWidth = 100f;
            float spacing = 10f;
            float totalWidth = buttonWidth * 2 + spacing;
            float cursorX = (Gui.GetWindowSize().X - totalWidth) * 0.5f;

            Gui.SetCursorPosX(cursorX);

            if (Gui.Button("Create", new NVec2(buttonWidth, 0)))
            {
                if (string.IsNullOrWhiteSpace(_projectName) || string.IsNullOrWhiteSpace(_projectPath))
                {
                    _formError = "Project name and path must not be empty.";
                }
                else
                {
                    _ = CreateProjectAsync(_projectName, _projectPath);

                    _formError = null;
                    Gui.CloseCurrentPopup();
                    // if (!_projectManager.TryCreateProject(_projectName, _projectPath, true, out _))
                    // {
                    //     _formError = "Failed to create project. Check the console window to see more details.";
                    // }
                    // else
                    // {
                    // }
                }
            }

            Gui.SameLine();

            if (Gui.Button("Cancel", new NVec2(buttonWidth, 0)))
            {
                _formError = null;
                Gui.CloseCurrentPopup();
            }

            Gui.EndPopup();
        }

        Gui.PopStyleVar();
    }

    private async Task OpenProjectAsync()
    {
        string defaultPath = AppContext.BaseDirectory;
        try
        {
            string? path = await _windowService.ShowOpenFileDialogAsync(defaultPath, "pkproj");
            if (path != null)
            {
                await _commandDispatcher.ExecuteAsync(new OpenProjectCommand(path));
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to open project.", ex);
        }
    }

    private async Task OpenBrowseDirectoryAsync()
    {
        string? result = await _windowService.ShowOpenDirectoryDialogAsync(null!);
        if (result != null)
            _projectPath = result;
    }

    private async Task CreateProjectAsync(string name, string directory)
    {
        await _commandDispatcher.ExecuteAsync(new NewProjectCommand(name, directory));
    }
}