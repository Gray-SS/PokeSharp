namespace PokeLab.Presentation.States;

public abstract class StatefulView<TState, TIntent> : IView
    where TState : class
    where TIntent : class
{
    public abstract string Id { get; }
    public abstract string Title { get; }
    public bool IsVisible { get; set; }

    public IStateStore<TState, TIntent> Store { get; }

    public StatefulView(IStateStore<TState, TIntent> store)
    {
        Store = store;
    }

    public abstract void Render();

    public void Dispatch(TIntent intent)
    {
        _ = Store.DispatchAsync(intent);
    }
}