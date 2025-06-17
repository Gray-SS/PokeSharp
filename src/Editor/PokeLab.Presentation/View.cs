using CommunityToolkit.Mvvm.Input;

namespace PokeLab.Presentation;

public abstract class View<TViewModel> : IView
    where TViewModel : IViewModel
{
    public TViewModel ViewModel { get; }

    public View(TViewModel viewModel)
    {
        ViewModel = viewModel;
    }

    public abstract void Render();

    public void Execute(IRelayCommand command)
    {
        command.Execute(null);
    }

    public void Execute<T>(IRelayCommand<T> command, T parameter)
    {
        command.Execute(parameter);
    }

    public async Task ExecuteAsync(IAsyncRelayCommand command)
    {
        await command.ExecuteAsync(null);
    }

    public async Task ExecuteAsync<T>(IAsyncRelayCommand<T> command, T parameter)
    {
        await command.ExecuteAsync(parameter);
    }

    public void ExecuteBackground(IAsyncRelayCommand command)
    {
        _ = Task.Run(() => ExecuteAsync(command));
    }

    public void ExecuteBackground<T>(IAsyncRelayCommand<T> command, T parameter)
    {
        _ = Task.Run(() => ExecuteAsync(command, parameter));
    }
}