namespace PokeLab.Presentation.States;

public interface IStateReducer<TState, TIntent>
    where TState : class
    where TIntent : class
{
    TState Reduce(TState state, TIntent intent);
}