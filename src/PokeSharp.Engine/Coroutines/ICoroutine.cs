namespace PokeSharp.Engine.Coroutines;

public interface ICoroutine
{
    bool IsFinished(float dt);
}