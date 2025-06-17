using ImGuiNET;
using PokeLab.Presentation.MainMenu;
using PokeLab.Presentation.ImGui.Helpers;
using System.ComponentModel;

namespace PokeLab.Presentation.ImGui.MainMenu;

public sealed class MainMenuView : View<MainMenuViewModel>
{
    private bool _shouldOpenPopup;
    private bool _shouldClosePopup;
    private MainMenuViewState _lastViewState;

    public MainMenuView(MainMenuViewModel viewModel) : base(viewModel)
    {
        viewModel.PropertyChanging += OnPropertyChanging;
        viewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanging(object? sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.ViewState))
        {
            _lastViewState = ViewModel.ViewState;
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.ViewState))
        {
            _shouldOpenPopup = _lastViewState == MainMenuViewState.Idle && ViewModel.ViewState != MainMenuViewState.Idle;
            _shouldClosePopup = _lastViewState != MainMenuViewState.Idle && ViewModel.ViewState == MainMenuViewState.Idle;
        }
    }

    public override void Render()
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

    private void DrawMainMenuBar()
    {
        if (Gui.BeginMainMenuBar())
        {
            if (Gui.BeginMenu("File"))
            {
                bool inDialog = ViewModel.IsInDialog;

                if (Gui.MenuItem("New project", !inDialog))
                    Execute(ViewModel.StartCreateProjectCommand);

                if (Gui.MenuItem("Open project", !inDialog))
                    ExecuteBackground(ViewModel.OpenProjectCommand);

                if (Gui.MenuItem("Delete project", !inDialog))
                    ExecuteBackground(ViewModel.DeleteProjectCommand);

                Gui.Separator();

                if (Gui.MenuItem("Exit", !inDialog))
                    ExecuteBackground(ViewModel.ExitApplicationCommand);

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

            string projectName = ViewModel.ProjectName;
            if (Gui.InputText("##project_name", ref projectName, 256))
                ViewModel.ProjectName = projectName;

            Gui.Dummy(new(0, 5));

            Gui.Text("Project Path");
            Gui.SameLine();

            string projectPath = ViewModel.ProjectPath;
            if (Gui.InputText("##project_path", ref projectPath, 256))
                ViewModel.ProjectPath = projectPath;

            Gui.SameLine();
            Gui.Dummy(new(5, 0));
            Gui.SameLine();

            bool canBrowseProjectPath = ViewModel.BrowseProjectPathCommand.CanExecute(null);
            Gui.Text($"Button State: {(canBrowseProjectPath ? "Enabled" : "Disabled")}");
            if (GuiHelper.Button("Browse", ViewModel.BrowseProjectPathCommand.CanExecute(null)))
                ExecuteBackground(ViewModel.BrowseProjectPathCommand);

            Gui.Dummy(new(0, 5));

            if (!string.IsNullOrEmpty(ViewModel.ErrorMessage))
            {
                Gui.PushStyleColor(ImGuiCol.Text, new NVec4(1, 0.2f, 0.2f, 1));
                Gui.TextWrapped(ViewModel.ErrorMessage);
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

            if (ViewModel.IsLoading)
            {
                GuiHelper.LoadingSpinner(6f, 2, Color.Cyan);
                Gui.SameLine();
            }
            if (GuiHelper.Button("Create", new NVec2(buttonWidth, 0), ViewModel.ConfirmProjectCreationCommand.CanExecute(null)))
                ExecuteBackground(ViewModel.ConfirmProjectCreationCommand);

            Gui.SameLine();

            if (GuiHelper.Button("Cancel", new NVec2(buttonWidth, 0), ViewModel.ClearFormCommand.CanExecute(null)))
                Execute(ViewModel.ClearFormCommand);

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