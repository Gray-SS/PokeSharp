namespace PokeLab.Presentation.States;

public interface IStateEffect<TState, TIntent>
    where TState : class
    where TIntent : class
{
    Task HandleAsync(TIntent intent, IStateStore<TState, TIntent> store);
}