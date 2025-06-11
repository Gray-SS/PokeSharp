namespace PokeEngine.Core;

public interface IGameLoop
{
    event EventHandler? Initialized;
    event EventHandler<DrawContext>? Rendered;
    event EventHandler<UpdateContext>? Updated;
}