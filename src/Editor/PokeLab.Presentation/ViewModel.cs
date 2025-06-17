using CommunityToolkit.Mvvm.ComponentModel;

namespace PokeLab.Presentation;

public abstract partial class ViewModel : ObservableObject, IViewModel
{
}

public abstract class StatefulViewModel<TState> : ViewModel, IStatefulViewModel<TState>
    where TState : class
{
    public abstract TState CaptureState();
    public abstract void RestoreState(TState state);

    void IStatefulViewModel.RestoreState(object state)
        => this.RestoreState((TState)state);

    object IStatefulViewModel.CaptureState()
        => this.CaptureState();
}