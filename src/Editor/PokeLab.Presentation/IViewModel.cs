namespace PokeLab.Presentation;

public interface IViewModel
{
}

public interface IStatefulViewModel : IViewModel
{
    object CaptureState();
    void RestoreState(object state);
}

public interface IStatefulViewModel<TState> : IStatefulViewModel
    where TState : class
{
    new TState CaptureState();
    void RestoreState(TState state);
}