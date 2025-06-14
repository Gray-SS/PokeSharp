namespace PokeLab.Presentation.States;

public interface IStateStore<TState, TIntent>
    where TState : class
    where TIntent : class
{
    TState CurrentState { get; }
    event Action<TState>? OnStateChanged;

    Task DispatchAsync(TIntent intent);
}