using System.Collections;
using System.Text;
using Pokemon.DesktopGL.Core.Coroutines;

namespace Pokemon.DesktopGL.Miscellaneous;

public sealed class TextTyper
{
    public float Delay { get; }
    public StringBuilder CurrentText { get; }
    public bool IsWriting { get; private set; }

    public TextTyper(float delay)
    {
        Delay = delay;
        CurrentText = new StringBuilder();
    }

    public IEnumerator Write(string text)
    {
        if (!IsWriting)
        {
            IsWriting = true;
            CurrentText.Clear();

            foreach (var c in text)
            {
                CurrentText.Append(c);
                yield return new WaitForSeconds(Delay);
            }

            IsWriting = false;
        }
    }
}