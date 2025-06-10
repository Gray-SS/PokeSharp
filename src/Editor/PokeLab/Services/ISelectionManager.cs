namespace PokeLab.Services;

public sealed class SelectionUpdatedArgs : EventArgs
{
    public bool IsSelecting { get; }
    public bool IsMultiSelect { get; }
    public bool IsSingleSelect { get; }
    public object SelectedObject { get; }
    public ISelectionManager SelectionManager { get; }
    public IReadOnlyCollection<object> SelectedObjects { get; }

    public SelectionUpdatedArgs(ISelectionManager manager, IReadOnlyCollection<object> selectedObjects)
    {
        SelectionManager = manager;
        SelectedObjects = selectedObjects;
        IsMultiSelect = selectedObjects.Count > 1;
        IsSingleSelect = selectedObjects.Count == 1;
        IsSelecting = selectedObjects.Count > 0;
        SelectedObject = SelectedObjects.FirstOrDefault()!;
    }
}

public interface ISelectionManager
{
    bool IsSelecting { get; }
    bool IsMultiSelect { get; }
    bool IsSingleSelect { get; }
    int SelectionCount { get; }

    object? SelectedObject { get; }
    public IReadOnlyCollection<object> SelectedObjects { get; }

    event EventHandler<SelectionUpdatedArgs>? SelectionUpdated;

    void SelectObject(object obj, bool additive = false);
    void UnselectObject(object obj);
    void SelectObjects(IEnumerable<object> objects);

    void ClearSelection();
}