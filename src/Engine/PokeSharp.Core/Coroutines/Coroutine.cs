using System.Collections;
using Microsoft.Xna.Framework;

namespace PokeSharp.Core.Coroutines;

public sealed class Coroutine
{
    public bool IsDone { get; private set; }
    private readonly Stack<IEnumerator> _routines;

    public Coroutine(IEnumerator routine)
    {
        IsDone = false;
        _routines = new Stack<IEnumerator>();
        _routines.Push(routine);
    }

    public void Update(GameTime gameTime)
    {
        if (IsDone || _routines.Count == 0)
            return;

        var routine = _routines.Peek();
        if (routine.Current is ICoroutine coroutine)
        {
            if (coroutine.IsFinished(gameTime))
                ExecuteStep(routine);
        }
        else if (routine.Current is IEnumerator nested)
        {
            _routines.Push(nested);

            if (!nested.MoveNext())
            {
                _routines.Pop();
                ExecuteStep(routine);
            }
        }
        else ExecuteStep(routine);
    }

    private void ExecuteStep(IEnumerator routine)
    {
        if (!routine.MoveNext())
        {
            _routines.Pop();
            if (_routines.Count == 0)
                IsDone = true;
        }
    }

    // Helper function to start a coroutine easily with a static method
    public static void Start(IEnumerator routine)
        => S.GetService<ICoroutineManager>().StartCoroutine(routine);
}