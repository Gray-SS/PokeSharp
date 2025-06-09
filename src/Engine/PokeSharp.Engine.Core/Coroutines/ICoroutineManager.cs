using System.Collections;

namespace PokeSharp.Engine.Core.Coroutines;

public interface ICoroutineManager
{
    void StartCoroutine(IEnumerator routine);
}