using System;
using System.Collections.Generic;

namespace Pokemon.DesktopGL.Dialogues;

public class DialogueManager
{
    private readonly Queue<string> _lines = new();
    private string _fullLine = string.Empty;
    private string _displayedText = string.Empty;

    private float _typeTimer;
    private int _charIndex;

    private const float TypeSpeed = 0.035f;

    public bool IsActive { get; private set; }
    public bool LineFinished => _displayedText == _fullLine;

    public string DisplayedText => _displayedText;

    public event Action DialogueStarted;
    public event Action DialogueEnded;

    public void StartDialogue(IEnumerable<string> lines)
    {
        _lines.Clear();
        foreach (var line in lines)
            _lines.Enqueue(line);

        IsActive = true;
        DialogueStarted?.Invoke();
        NextLine();
    }

    public void Update(float dt)
    {
        if (!IsActive) return;

        if (!LineFinished)
        {
            _typeTimer -= dt;
            while (_typeTimer <= 0f && _charIndex < _fullLine.Length)
            {
                _displayedText += _fullLine[_charIndex];
                _charIndex++;
                _typeTimer += TypeSpeed;
            }
        }
    }

    public void SkipOrNext()
    {
        if (!IsActive) return;

        if (!LineFinished)
        {
            _displayedText = _fullLine;
            _charIndex = _fullLine.Length;
        }
        else
        {
            NextLine();
        }
    }

    private void NextLine()
    {
        if (_lines.Count > 0)
        {
            _fullLine = _lines.Dequeue();
            _displayedText = string.Empty;
            _charIndex = 0;
            _typeTimer = 0f;
        }
        else
        {
            IsActive = false;
            _fullLine = string.Empty;
            _displayedText = string.Empty;
            DialogueEnded?.Invoke();
        }
    }
}