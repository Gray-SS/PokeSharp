namespace PokeLab.Presentation.Common;

public sealed class DefaultEditorViewManager : IEditorViewManager
{
    private readonly IEditorView[] _views;

    public DefaultEditorViewManager(IEditorView[] views)
    {
        _views = views;
    }

    public IReadOnlyCollection<IEditorView> GetEditorViews()
    {
        return _views;
    }

    public void RenderViews()
    {
        foreach (IEditorView view in _views)
        {
            view.Render();
        }
    }
}