using System.Collections;

namespace PokeEngine.Core.Coroutines;

public interface ICoroutineManager
{
    void StartCoroutine(IEnumerator routine);
}