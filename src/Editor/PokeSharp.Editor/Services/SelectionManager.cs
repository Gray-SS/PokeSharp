
namespace PokeSharp.Editor.Services;

public sealed class SelectionManager : ISelectionManager
{
    public object? SelectedObject => _selectedObjects.FirstOrDefault();

    public bool IsSelecting => _selectedObjects.Count > 0;
    public bool IsMultiSelect => _selectedObjects.Count > 1;
    public bool IsSingleSelect => _selectedObjects.Count == 1;
    public int SelectionCount => _selectedObjects.Count;

    public IReadOnlyCollection<object> SelectedObjects => _selectedObjects;

    public event EventHandler<SelectionUpdatedArgs>? SelectionUpdated;

    private HashSet<object> _selectedObjects = new();

    public void ClearSelection()
    {
        _selectedObjects.Clear();
        SelectionUpdated?.Invoke(this, new SelectionUpdatedArgs(this, _selectedObjects));
    }

    public void SelectObject(object obj, bool additive = false)
    {
        if (!additive)
            _selectedObjects.Clear();

        _selectedObjects.Add(obj);
        SelectionUpdated?.Invoke(this, new SelectionUpdatedArgs(this, _selectedObjects));
    }

    public void UnselectObject(object obj)
    {
        _selectedObjects.Remove(obj);
        SelectionUpdated?.Invoke(this, new SelectionUpdatedArgs(this, _selectedObjects));
    }

    public void SelectObjects(IEnumerable<object> objects)
    {
        _selectedObjects.Clear();

        foreach (var item in objects)
            _selectedObjects.Add(item);

        SelectionUpdated?.Invoke(this, new SelectionUpdatedArgs(this, _selectedObjects));
    }
}
