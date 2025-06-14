using PokeCore.Diagnostics;

namespace PokeLab.Presentation.States;

public sealed class StateStore<TState, TIntent> : IStateStore<TState, TIntent>
    where TState : class
    where TIntent : class
{
    public TState CurrentState { get; private set; }

    public event Action<TState>? OnStateChanged;

    private readonly IStateEffect<TState, TIntent> _effect;
    private readonly IStateReducer<TState, TIntent> _reducer;

    public StateStore(TState defaultState, IStateReducer<TState, TIntent> reducer, IStateEffect<TState, TIntent> effect)
    {
        CurrentState = defaultState;

        _effect = effect;
        _reducer = reducer;
    }

    public async Task DispatchAsync(TIntent intent)
    {
        TState newState = _reducer.Reduce(CurrentState, intent);
        ThrowHelper.AssertNotNull(newState, "The reduced state must be not null");

        if (!newState.Equals(intent))
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }

        await _effect.HandleAsync(intent, this);
    }
}