using PokeLab.Presentation.States;

namespace PokeLab.Presentation.MainMenu;

public sealed class MainMenuReducer : IStateReducer<MainMenuState, MainMenuIntents>
{
    public MainMenuState Reduce(MainMenuState state, MainMenuIntents intent)
    {
        return intent switch
        {
            MainMenuIntents.SetProjectName setProjectName => state with { ProjectName = setProjectName.Value },
            MainMenuIntents.SetProjectPath setProjectPath => state with { ProjectPath = setProjectPath.Value },
            MainMenuIntents.SetError setError => state with { ErrorMessage = setError.Error },
            MainMenuIntents.SetState setState => state with { State = setState.State },
            MainMenuIntents.SetInDialog setInDialog => state with { IsInDialog = setInDialog.IsInDialog },
            MainMenuIntents.StartCreateProject => state with
            {
                ProjectName = "MyPokemon",
                ProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                ErrorMessage = null,
                State = MainMenuViewState.CreatePopup,
                IsInDialog = false,
            },
            MainMenuIntents.ClearForm => state with
            {
                ProjectName = string.Empty,
                ProjectPath = string.Empty,
                State = MainMenuViewState.Idle,
                ErrorMessage = null,
                IsInDialog = false,
            },
            _ => state
        };
    }
}