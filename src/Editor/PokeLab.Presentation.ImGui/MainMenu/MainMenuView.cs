using ImGuiNET;
using PokeLab.Presentation.MainMenu;

namespace PokeLab.Presentation.ImGui.MainMenu;

public sealed class MainMenuView : IMainMenuView
{
    public string Id => "Main Menu";
    public string Title => "Main Menu";
    public bool IsVisible { get; set; }

    public CreateProjectFormViewModel CreateProjectForm { get; }

    public event Action<MainMenuIntents>? OnIntents;

    public MainMenuView()
    {
        CreateProjectForm = new CreateProjectFormViewModel();
    }

    public void Render()
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
                if (Gui.MenuItem("New project"))
                    InvokeIntent(MainMenuIntents.StartCreateProject);

                if (Gui.MenuItem("Open project"))
                    InvokeIntent(MainMenuIntents.OpenProject);

                if (Gui.MenuItem("Delete project"))
                    InvokeIntent(MainMenuIntents.DeleteProject);

                Gui.Separator();

                if (Gui.MenuItem("Exit"))
                    InvokeIntent(MainMenuIntents.ExitApplication);

                Gui.EndMenu();
            }

            Gui.EndMainMenuBar();
        }
    }

    private void DrawCreatePopup()
    {
        CreateProjectFormViewModel form = CreateProjectForm;
        if (form.IsVisible)
        {
            Gui.OpenPopup("Create Project");
            form.Hide();
        }

        var center = Gui.GetMainViewport().GetCenter();
        Gui.SetNextWindowPos(center, ImGuiCond.Appearing, new NVec2(0.5f, 0.5f));
        Gui.PushStyleVar(ImGuiStyleVar.WindowPadding, new NVec2(20, 20));

        if (Gui.BeginPopupModal("Create Project", ImGuiWindowFlags.AlwaysAutoResize))
        {
            Gui.Dummy(new(0, 5));
            Gui.Text("Create a new project");
            Gui.Separator();
            Gui.Dummy(new(0, 10));

            Gui.Text("Project Name");
            Gui.SameLine();

            string projectName = form.ProjectName;
            if (Gui.InputText("##project_name", ref projectName, 256))
                form.ProjectName = projectName;

            Gui.Dummy(new(0, 5));

            Gui.Text("Project Path");
            Gui.SameLine();

            string projectPath = form.ProjectPath;
            if (Gui.InputText("##project_path", ref projectPath, 256))
                form.ProjectPath = projectPath;

            Gui.SameLine();
            Gui.Dummy(new(5, 0));
            Gui.SameLine();

            if (Gui.Button("Browse"))
                InvokeIntent(MainMenuIntents.BrowseProjectPath);

            Gui.Dummy(new(0, 5));

            if (!string.IsNullOrEmpty(form.ErrorMessage))
            {
                // Display error
                Gui.PushStyleColor(ImGuiCol.Text, new NVec4(1, 0.2f, 0.2f, 1));
                Gui.TextWrapped(form.ErrorMessage);
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
                InvokeIntent(MainMenuIntents.ConfirmCreateProject);

                if (CreateProjectForm.ErrorMessage == null)
                {
                    // Executing went fine, closing the popup
                    Gui.CloseCurrentPopup();
                }
            }

            Gui.SameLine();

            if (Gui.Button("Cancel", new NVec2(buttonWidth, 0)))
            {
                form.Reset();
                Gui.CloseCurrentPopup();
            }

            Gui.EndPopup();
        }

        Gui.PopStyleVar();
    }

    private void InvokeIntent(MainMenuIntents intent)
    {
        OnIntents?.Invoke(intent);
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