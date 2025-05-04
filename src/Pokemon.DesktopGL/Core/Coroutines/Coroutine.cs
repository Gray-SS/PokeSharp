using System.Collections;
using System.Collections.Generic;

namespace Pokemon.DesktopGL.Core.Coroutines;

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

    public void Update(float dt)
    {
        if (IsDone || _routines.Count == 0)
            return;

        var routine = _routines.Peek();
        if (routine.Current is ICoroutine coroutine)
        {
            if (coroutine.IsFinished(dt))
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
}