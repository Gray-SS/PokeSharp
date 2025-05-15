using System.Collections;
using PokeSharp.Core.Coroutines;

namespace PokeSharp.Core.Managers;

public class CoroutineManager
{
    public static void Start(IEnumerator routine)
        => Engine.Instance.CoroutineManager.StartCoroutine(routine);

    private readonly List<Coroutine> _coroutines = new();

    public void StartCoroutine(IEnumerator routine)
    {
        _coroutines.Add(new Coroutine(routine));
    }

    public void Update(float dt)
    {
        for (int i = _coroutines.Count - 1; i >= 0; i--)
        {
            var coroutine = _coroutines[i];
            coroutine.Update(dt);

            if (coroutine.IsDone)
            {
                _coroutines.RemoveAt(i);
            }
        }
    }
}