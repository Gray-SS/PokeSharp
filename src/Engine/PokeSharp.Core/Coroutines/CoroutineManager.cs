using System.Collections;
using Microsoft.Xna.Framework;
using PokeSharp.Core.Attributes;

namespace PokeSharp.Core.Coroutines;

[Priority(999)]
public class CoroutineManager : ICoroutineManager, IEngineHook
{
    private readonly List<Coroutine> _coroutines = new();

    public void StartCoroutine(IEnumerator routine)
    {
        _coroutines.Add(new Coroutine(routine));
    }

    public void Initialize()
    {
    }

    public void Update(GameTime gameTime)
    {
        for (int i = _coroutines.Count - 1; i >= 0; i--)
        {
            var coroutine = _coroutines[i];
            coroutine.Update(gameTime);

            if (coroutine.IsDone)
            {
                _coroutines.RemoveAt(i);
            }
        }
    }

    public void Draw(GameTime gameTime)
    {
    }
}