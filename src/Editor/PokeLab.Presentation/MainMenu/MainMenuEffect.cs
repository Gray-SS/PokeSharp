using PokeCore.Logging;
using PokeLab.Application.Commands;
using PokeLab.Application.Common;
using PokeLab.Application.ProjectManagement;
using PokeLab.Presentation.Common;
using PokeLab.Presentation.States;

namespace PokeLab.Presentation.MainMenu;

public sealed class MainMenuEffect : IStateEffect<MainMenuState, MainMenuIntents>
{
    private readonly Logger _logger;
    private readonly IWindowService _windowService;
    private readonly ICommandDispatcher _commandDispatcher;

    public MainMenuEffect(
        Logger<MainMenuEffect> logger,
        IWindowService windowService,
        ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _windowService = windowService;
        _commandDispatcher = commandDispatcher;
    }

    public async Task HandleAsync(MainMenuIntents intent, IStateStore<MainMenuState, MainMenuIntents> store)
    {
        Task task = intent switch
        {
            MainMenuIntents.OpenProject => OpenProjectAsync(store),
            MainMenuIntents.DeleteProject => DeleteProjectAsync(store),
            MainMenuIntents.BrowseProjectPath => BrowseProjectPathAsync(store),
            MainMenuIntents.ConfirmCreateProject => ConfirmProjectCreationAsync(store),
            MainMenuIntents.ExitApplication => ExitApplicationAsync(store),
            _ => Task.CompletedTask
        };

        await task;
    }

    private async Task OpenProjectAsync(IStateStore<MainMenuState, MainMenuIntents> store)
    {
        await store.DispatchAsync(new MainMenuIntents.SetInDialog(true));

        _logger.Trace("Opening file dialog...");
        string? path = await _windowService.ShowOpenFileDialogAsync(null!, "pkproj");

        await store.DispatchAsync(new MainMenuIntents.SetInDialog(false));

        if (path != null)
        {
            _logger.Trace($"File opened: '{path}'");
            await _commandDispatcher.ExecuteAsync(new OpenProjectCommand(path));
        }
        else _logger.Trace("File dialog cancelled.");
    }

    private async Task ExitApplicationAsync(IStateStore<MainMenuState, MainMenuIntents> store)
    {
        _logger.Trace("Exiting application...");
        await _commandDispatcher.ExecuteAsync(new ExitCommand());
    }

    private async Task BrowseProjectPathAsync(IStateStore<MainMenuState, MainMenuIntents> store)
    {
        await store.DispatchAsync(new MainMenuIntents.SetInDialog(true));

        _logger.Trace("Opening file dialog...");
        string? path = await _windowService.ShowOpenDirectoryDialogAsync(store.CurrentState.ProjectPath);

        await store.DispatchAsync(new MainMenuIntents.SetInDialog(false));

        if (path != null)
        {
            _logger.Trace($"File opened: '{path}'");
            await store.DispatchAsync(new MainMenuIntents.SetProjectPath(path));
        }
        else _logger.Trace("File dialog cancelled.");
    }

    private async Task ConfirmProjectCreationAsync(IStateStore<MainMenuState, MainMenuIntents> store)
    {
        MainMenuState state = store.CurrentState;
        await store.DispatchAsync(new MainMenuIntents.SetState(MainMenuViewState.Loading));

        string projectName = state.ProjectName.Trim();
        if (string.IsNullOrEmpty(projectName))
        {
            await store.DispatchAsync(new MainMenuIntents.SetError("The project name must be not null or empty"));
            await store.DispatchAsync(new MainMenuIntents.SetState(MainMenuViewState.CreatePopup));
            return;
        }

        string projectPath = state.ProjectPath.Trim();
        if (string.IsNullOrEmpty(projectPath))
        {
            await store.DispatchAsync(new MainMenuIntents.SetError("The project path must be not null or empty"));
            await store.DispatchAsync(new MainMenuIntents.SetState(MainMenuViewState.CreatePopup));
            return;
        }

        // Simulate delay
        await Task.Delay(1000);
        await _commandDispatcher.ExecuteAsync(new NewProjectCommand(projectName, projectPath));

        await store.DispatchAsync(new MainMenuIntents.ClearForm());
    }

    private async Task DeleteProjectAsync(IStateStore<MainMenuState, MainMenuIntents> store)
    {
        await store.DispatchAsync(new MainMenuIntents.SetInDialog(true));

        _logger.Trace("Opening file dialog...");
        string? path = await _windowService.ShowOpenFileDialogAsync(null!, "pkproj");

        await store.DispatchAsync(new MainMenuIntents.SetInDialog(false));

        if (path != null)
        {
            _logger.Trace($"File opened: '{path}'");
            await _commandDispatcher.ExecuteAsync(new DeleteProjectCommand(path));
        }
        else _logger.Trace("File dialog cancelled.");
    }
}