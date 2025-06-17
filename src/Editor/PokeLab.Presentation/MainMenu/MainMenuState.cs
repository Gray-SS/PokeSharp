namespace PokeLab.Presentation.MainMenu;

public enum MainMenuViewState
{
    Idle,
    Loading,
    CreatePopup
}

public record MainMenuState(
    string ProjectName,
    string ProjectPath,
    string? ErrorMessage,
    MainMenuViewState ViewState
);