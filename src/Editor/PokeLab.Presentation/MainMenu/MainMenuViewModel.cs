using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PokeCore.Logging;
using PokeLab.Application.Commands;
using PokeLab.Application.Common.Messages;
using PokeLab.Application.ProjectManagement.Messages;
using PokeLab.Presentation.Common;

namespace PokeLab.Presentation.MainMenu;

public sealed partial class MainMenuViewModel(
    Logger<MainMenuViewModel> logger,
    IWindowService windowService,
    ICommandDispatcher commandDispatcher
) : StatefulViewModel<MainMenuState>
{
    public bool IsIdle => ViewState == MainMenuViewState.Idle && !IsInDialog;
    public bool IsBusy => !IsIdle;
    public bool IsLoading => ViewState == MainMenuViewState.Loading;

    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private string _projectPath = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private MainMenuViewState _viewState;

    [ObservableProperty]
    private bool _isInDialog;

    [RelayCommand(CanExecute = nameof(IsIdle))]
    private async Task OpenProjectAsync()
    {
        IsInDialog = true;

        logger.Trace("Opening file dialog...");
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string? path = await windowService.ShowOpenFileDialogAsync(defaultPath, "pkproj");

        IsInDialog = false;

        if (path != null)
        {
            logger.Trace($"File opened: '{path}'");
            await commandDispatcher.SendAsync(new ProjectCommands.Open(path));
        }
        else logger.Trace("File dialog cancelled.");
    }

    [RelayCommand(CanExecute = nameof(IsIdle))]
    private async Task ExitApplicationAsync()
    {
        logger.Trace("Exiting application...");
        await commandDispatcher.SendAsync(new CommonCommands.Exit());
    }

    [RelayCommand(CanExecute = nameof(IsIdle))]
    private async Task DeleteProjectAsync()
    {
        IsInDialog = true;

        logger.Trace("Opening file dialog...");
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string? path = await windowService.ShowOpenFileDialogAsync(defaultPath, "pkproj");

        IsInDialog = false;

        if (path != null)
        {
            logger.Trace($"File opened: '{path}'");
            await commandDispatcher.SendAsync(new ProjectCommands.Delete(path));
        }
        else logger.Trace("File dialog cancelled.");
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileOperations))]
    private async Task BrowseProjectPathAsync()
    {
        IsInDialog = true;

        logger.Trace("Opening file dialog...");
        string? path = await windowService.ShowOpenDirectoryDialogAsync(ProjectPath);

        IsInDialog = false;

        if (path != null)
        {
            logger.Trace($"File opened: '{path}'");
            ProjectPath = path;
        }
        else logger.Trace("File dialog cancelled.");
    }

    private bool CanExecuteFileOperations() => !IsInDialog && !IsLoading;

    [RelayCommand(CanExecute = nameof(CanConfirmProjectCreation))]
    private async Task ConfirmProjectCreationAsync()
    {
        ViewState = MainMenuViewState.Loading;

        var validationResult = ValidateProjectCreation();
        if (!validationResult.IsValid)
        {
            ErrorMessage = validationResult.ErrorMessage;
            ViewState = MainMenuViewState.CreatePopup;
            return;
        }

        await commandDispatcher.SendAsync(new ProjectCommands.Create(ProjectName, ProjectPath));

        ClearFormCommand.Execute(null);
    }

    private bool CanConfirmProjectCreation() =>
        !string.IsNullOrWhiteSpace(ProjectName) &&
        !string.IsNullOrWhiteSpace(ProjectPath) &&
        !IsLoading && !IsInDialog;

    private (bool IsValid, string? ErrorMessage) ValidateProjectCreation()
    {
        if (string.IsNullOrWhiteSpace(ProjectName))
            return (false, "The project name cannot be empty");

        if (string.IsNullOrWhiteSpace(ProjectPath))
            return (false, "The project path cannot be empty");

        if (!Directory.Exists(ProjectPath))
            return (false, "The specified directory does not exist");

        return (true, null);
    }

    [RelayCommand]
    private void StartCreateProject()
    {
        ProjectName = GenerateDefaultProjectName();
        ProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        ErrorMessage = null;
        ViewState = MainMenuViewState.CreatePopup;
    }

    private static string GenerateDefaultProjectName()
    {
        var baseName = "MyPokeGame";
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var counter = 1;

        while (File.Exists(Path.Combine(documentsPath, $"{baseName}{(counter == 1 ? string.Empty : counter.ToString())}")))
        {
            counter++;
        }

        return counter == 1 ? baseName : $"{baseName}{counter}";
    }

    [RelayCommand]
    private void ClearForm()
    {
        ProjectName = string.Empty;
        ProjectPath = string.Empty;
        ErrorMessage = null;
        ViewState = MainMenuViewState.Idle;
    }

    public override MainMenuState CaptureState()
    {
        return new MainMenuState(ProjectName, ProjectPath, ErrorMessage, ViewState);
    }

    public override void RestoreState(MainMenuState state)
    {
        ProjectName = state.ProjectName;
        ProjectPath = state.ProjectPath;
        ErrorMessage = state.ErrorMessage;
        ViewState = state.ViewState;
    }
}