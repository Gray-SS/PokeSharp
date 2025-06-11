namespace PokeEngine.Core.Coroutines;

public interface ICoroutine
{
    bool IsFinished(UpdateContext context);
}