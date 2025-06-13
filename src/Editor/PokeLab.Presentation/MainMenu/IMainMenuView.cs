namespace PokeLab.Presentation.MainMenu;

public interface IMainMenuView : IView
{
    CreateProjectFormViewModel CreateProjectForm { get; }

    event Action<MainMenuIntents>? OnIntents;
}