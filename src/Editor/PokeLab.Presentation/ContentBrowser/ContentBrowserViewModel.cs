using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PokeCore.Logging;
using PokeLab.Application.Commands;
using PokeLab.Application.ContentBrowser;
using PokeLab.Application.ProjectManagement.Messages;
using PokeLab.Presentation.Common;

namespace PokeLab.Presentation.ContentBrowser;

public sealed partial class ContentBrowserViewModel
: StatefulViewModel<ContentBrowserState>, IDisposable
{
    public bool IsInitialized => _contentBrowserService.IsInitialized;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<ContentBrowserItem> _items = new();

    private bool _isDisposed;

    private readonly Logger _logger;
    private readonly ITickSource _tickSource;
    private readonly IWindowService _windowService;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IContentBrowserService _contentBrowserService;

    public ContentBrowserViewModel(
        Logger<ContentBrowserViewModel> logger,
        ITickSource tickSource,
        IWindowService windowService,
        ICommandDispatcher commandDispatcher,
        IContentBrowserService contentBrowserService
    )
    {
        _logger = logger;
        _tickSource = tickSource;
        _windowService = windowService;
        _commandDispatcher = commandDispatcher;
        _contentBrowserService = contentBrowserService;

        SubscribeEvents();
    }

    [RelayCommand(CanExecute = nameof(CanPerformNavbarOperations))]
    private async Task SaveAsync()
    {
        await _commandDispatcher.SendAsync(new ProjectCommands.Save());
    }

    [RelayCommand(CanExecute = nameof(CanPerformNavbarOperations))]
    private async Task ImportAsync()
    {
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string? path = await _windowService.ShowOpenFileDialogAsync(defaultPath, string.Empty);

        if (path == null)
            return;
    }

    private bool CanPerformNavbarOperations()
        => IsInitialized && !IsLoading;

    private void SubscribeEvents()
    {
        _tickSource.OnTick += OnTick;
    }

    private void UnsubscribeEvents()
    {
        _tickSource.OnTick -= OnTick;
    }

    private void OnTick()
    {
        _contentBrowserService.Tick();
    }

    public override ContentBrowserState CaptureState()
    {
        return new ContentBrowserState(
        );
    }

    public override void RestoreState(ContentBrowserState state)
    {
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            UnsubscribeEvents();
            _isDisposed = true;
        }
    }
}
