namespace PokeSharp.Core.Coroutines;

public interface ICoroutine
{
    bool IsFinished(float dt);
}