using System.Collections;

namespace PokeSharp.Core.Coroutines;

public interface ICoroutineManager
{
    void StartCoroutine(IEnumerator routine);
}