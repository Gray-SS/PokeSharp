using ImGuiNET;
using PokeLab.Presentation.States;
using PokeLab.Presentation.MainMenu;
using PokeLab.Presentation.ImGui.Helpers;

namespace PokeLab.Presentation.ImGui.MainMenu;

public sealed class MainMenuView : StatefulView<MainMenuState, MainMenuIntents>, IMainMenuView
{
    public override string Id => "Main Menu";
    public override string Title => "Main Menu";

    private bool _shouldOpenPopup;
    private bool _shouldClosePopup;
    private MainMenuState _state;
    private MainMenuState? _newState;

    public MainMenuView(IStateStore<MainMenuState, MainMenuIntents> store) : base(store)
    {
        _state = store.CurrentState;
        _shouldOpenPopup = false;
        _shouldClosePopup = false;

        store.OnStateChanged += (state) => _newState = state;
    }

    public override void Render()
    {
        if (_newState != null)
        {
            _shouldOpenPopup = _newState.State != MainMenuViewState.Idle && _state.State == MainMenuViewState.Idle;
            _shouldClosePopup = _newState.State == MainMenuViewState.Idle && _state.State != MainMenuViewState.Idle;

            _state = _newState;
            _newState = null;
        }

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

    private void DrawMainMenuBar()
    {
        if (Gui.BeginMainMenuBar())
        {
            if (Gui.BeginMenu("File"))
            {
                bool inDialog = _state.IsInDialog;

                if (Gui.MenuItem("New project", !inDialog))
                    Dispatch(new MainMenuIntents.StartCreateProject());

                if (Gui.MenuItem("Open project", !inDialog))
                    Dispatch(new MainMenuIntents.OpenProject());

                if (Gui.MenuItem("Delete project", !inDialog))
                    Dispatch(new MainMenuIntents.DeleteProject());

                Gui.Separator();

                if (Gui.MenuItem("Exit", !inDialog))
                    Dispatch(new MainMenuIntents.ExitApplication());

                Gui.EndMenu();
            }

            Gui.EndMainMenuBar();
        }
    }

    private void DrawCreatePopup()
    {
        if (_shouldOpenPopup)
        {
            Gui.OpenPopup("Create Project");
            _shouldOpenPopup = false;
        }

        var center = Gui.GetMainViewport().GetCenter();
        Gui.SetNextWindowPos(center, ImGuiCond.Appearing, new NVec2(0.5f, 0.5f));
        Gui.PushStyleVar(ImGuiStyleVar.WindowPadding, new NVec2(20, 20));

        if (Gui.BeginPopupModal("Create Project", ImGuiWindowFlags.AlwaysAutoResize))
        {
            if (_shouldClosePopup)
            {
                Gui.CloseCurrentPopup();
                _shouldClosePopup = false;
            }

            Gui.Dummy(new(0, 5));
            Gui.Text("Create a new project");
            Gui.Separator();
            Gui.Dummy(new(0, 10));

            Gui.Text("Project Name");
            Gui.SameLine();

            string projectName = _state.ProjectName;
            if (Gui.InputText("##project_name", ref projectName, 256))
                Dispatch(new MainMenuIntents.SetProjectName(projectName));

            Gui.Dummy(new(0, 5));

            Gui.Text("Project Path");
            Gui.SameLine();

            string projectPath = _state.ProjectPath;
            if (Gui.InputText("##project_path", ref projectPath, 256))
                Dispatch(new MainMenuIntents.SetProjectPath(projectPath));

            Gui.SameLine();
            Gui.Dummy(new(5, 0));
            Gui.SameLine();

            if (Gui.Button("Browse"))
                Dispatch(new MainMenuIntents.BrowseProjectPath());

            Gui.Dummy(new(0, 5));

            if (!string.IsNullOrEmpty(_state.ErrorMessage))
            {
                Gui.PushStyleColor(ImGuiCol.Text, new NVec4(1, 0.2f, 0.2f, 1));
                Gui.TextWrapped(_state.ErrorMessage);
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

            if (_state.State == MainMenuViewState.Loading)
            {
                GuiHelper.LoadingSpinner(6f, 2, Color.Cyan);
                Gui.SameLine();
            }

            if (Gui.Button("Create", new NVec2(buttonWidth, 0)))
                Dispatch(new MainMenuIntents.ConfirmCreateProject());

            Gui.SameLine();

            if (Gui.Button("Cancel", new NVec2(buttonWidth, 0)))
                Dispatch(new MainMenuIntents.ClearForm());

            Gui.EndPopup();
        }

        Gui.PopStyleVar();
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
}