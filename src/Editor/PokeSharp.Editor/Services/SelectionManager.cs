namespace PokeSharp.Editor.Services;

public sealed class SelectionManager : ISelectionManager
{
    public bool HasSelection => SelectedObject != null;
    public object? SelectedObject { get; private set; }

    public event EventHandler<object?>? SelectionChanged;

    public void ClearSelection()
    {
        SelectedObject = null;
        SelectionChanged?.Invoke(this, null);
    }

    public void SelectObject(object obj)
    {
        SelectedObject = obj;
        SelectionChanged?.Invoke(this, obj);
    }
}
