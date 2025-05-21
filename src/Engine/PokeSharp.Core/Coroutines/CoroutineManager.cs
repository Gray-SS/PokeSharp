using System.Collections;
using Microsoft.Xna.Framework;

namespace PokeSharp.Core.Coroutines;

public class CoroutineManager : IEngineHook
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