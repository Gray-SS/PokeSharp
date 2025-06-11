using System.Collections;
using Microsoft.Xna.Framework;
using PokeCore.Common.Annotations;

namespace PokeEngine.Core.Coroutines;

[Priority(999)]
public class CoroutineManager : ICoroutineManager
{
    private readonly List<Coroutine> _coroutines = new();

    public CoroutineManager(IGameLoop gameLoop)
    {
        gameLoop.Updated += OnUpdate;
    }

    public void StartCoroutine(IEnumerator routine)
    {
        _coroutines.Add(new Coroutine(routine));
    }

    public void OnUpdate(object? sender, UpdateContext context)
    {
        for (int i = _coroutines.Count - 1; i >= 0; i--)
        {
            var coroutine = _coroutines[i];
            coroutine.Update(context);

            if (coroutine.IsDone)
            {
                _coroutines.RemoveAt(i);
            }
        }
    }
}