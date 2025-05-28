using System.Diagnostics.CodeAnalysis;

namespace PokeSharp.Editor.Services;

public interface ISelectionManager
{
    bool HasSelection { get; }

    [MemberNotNullWhen(true, nameof(HasSelection))]
    object? SelectedObject { get; }

    // TODO: Create a specific event args for this
    event EventHandler<object?>? SelectionChanged;

    void SelectObject(object obj);
    void ClearSelection();
}