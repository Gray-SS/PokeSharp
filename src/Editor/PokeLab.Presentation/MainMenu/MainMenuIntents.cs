namespace PokeLab.Presentation.MainMenu;

public abstract record MainMenuIntents
{
    public record OpenProject : MainMenuIntents;
    public record DeleteProject : MainMenuIntents;
    public record ExitApplication : MainMenuIntents;
    public record StartCreateProject : MainMenuIntents;
    public record BrowseProjectPath : MainMenuIntents;
    public record ConfirmCreateProject : MainMenuIntents;

    public record SetState(MainMenuViewState State) : MainMenuIntents;
    public record SetInDialog(bool IsInDialog) : MainMenuIntents;
    public record SetError(string Error) : MainMenuIntents;
    public record SetProjectName(string Value) : MainMenuIntents;
    public record SetProjectPath(string Value) : MainMenuIntents;
    public record ClearForm : MainMenuIntents;
}