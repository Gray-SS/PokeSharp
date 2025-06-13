using PokeCore.Logging;
using PokeLab.Application.Common;
using PokeLab.Application.Commands;
using PokeLab.Application.ProjectManagement;
using PokeLab.Presentation.Common;

namespace PokeLab.Presentation.MainMenu;

public sealed class MainMenuPresenter : IPresenter
{
    private readonly Logger _logger;
    private readonly IMainMenuView _mainView;
    private readonly IWindowService _windowService;
    private readonly ICommandDispatcher _commandDispatcher;

    public MainMenuPresenter(
        IMainMenuView mainView,
        IWindowService windowService,
        ICommandDispatcher commandDispatcher,
        Logger<MainMenuPresenter> logger
    )
    {
        _mainView = mainView;
        _mainView.OnIntents += OnMainViewIntents;

        _logger = logger;
        _windowService = windowService;
        _commandDispatcher = commandDispatcher;
    }

    private void OnMainViewIntents(MainMenuIntents intent)
    {
        _logger.Trace($"{intent} request received");
        _logger.Trace("Processing create project request...");

        switch (intent)
        {
            case MainMenuIntents.OpenProject:
                OnOpenProjectRequest();
                break;
            case MainMenuIntents.DeleteProject:
                OnDeleteProjectRequest();
                break;
            case MainMenuIntents.ExitApplication:
                OnExitApplicationRequest();
                break;

            case MainMenuIntents.StartCreateProject:
                OnCreateProjectRequest();
                break;
            case MainMenuIntents.BrowseProjectPath:
                OnBrowseProjectPathRequest();
                break;
            case MainMenuIntents.ConfirmCreateProject:
                OnCreateProjectConfirm();
                break;
        }
    }

    private void OnCreateProjectRequest()
    {
        var form = _mainView.CreateProjectForm;

        form.Reset();
        form.Show();

        form.ProjectName = "CoolTruck";
        form.ProjectPath = Environment.CurrentDirectory;
    }

    private void OnOpenProjectRequest()
    {
        _ = Task.Run(async () =>
        {
            _logger.Trace("Opening file dialog...");
            string? path = await _windowService.ShowOpenFileDialogAsync(null!, "pkproj");

            if (path == null)
            {
                _logger.Trace("File dialog cancelled.");
                return;
            }

            _logger.Trace($"File opened: '{path}'");
            await _commandDispatcher.ExecuteAsync(new OpenProjectCommand(path));
        });
    }

    private void OnBrowseProjectPathRequest()
    {
        string defaultPath = _mainView.CreateProjectForm.ProjectPath;

        _ = Task.Run(async () =>
        {
            _logger.Trace("Opening file dialog...");
            string? path = await _windowService.ShowOpenDirectoryDialogAsync(defaultPath);

            if (path == null)
            {
                _logger.Trace("File dialog cancelled.");
                return;
            }

            _logger.Trace($"File opened: '{path}'");
            _mainView.CreateProjectForm.ProjectPath = path;
        });
    }

    private void OnCreateProjectConfirm()
    {
        var form = _mainView.CreateProjectForm;

        if (!form.Validate())
            return;

        _ = Task.Run(async () =>
        {
            //TODO: Find a way to handle errors
            await _commandDispatcher.ExecuteAsync(new NewProjectCommand(form.ProjectName, form.ProjectPath));

            form.Reset();
        });
    }

    private void OnDeleteProjectRequest()
    {
        _ = Task.Run(async () =>
        {
            _logger.Trace("Opening file dialog...");
            string? path = await _windowService.ShowOpenFileDialogAsync(null!, "pkproj");

            if (path == null)
            {
                _logger.Trace("File dialog cancelled.");
                return;
            }

            _logger.Trace($"File opened: '{path}'");
            await _commandDispatcher.ExecuteAsync(new DeleteProjectCommand(path));
        });
    }

    private void OnExitApplicationRequest()
    {
        _ = Task.Run(async () => await _commandDispatcher.ExecuteAsync(new ExitCommand()));
    }
}