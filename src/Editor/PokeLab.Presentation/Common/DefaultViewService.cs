namespace PokeLab.Presentation.Common;

public sealed class DefaultViewService : IViewService
{
    private readonly IEditorView[] _views;

    public DefaultViewService(IEditorView[] views)
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