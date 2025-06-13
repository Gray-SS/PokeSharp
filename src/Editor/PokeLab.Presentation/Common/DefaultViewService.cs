namespace PokeLab.Presentation.Common;

public sealed class DefaultViewService : IViewService
{
    private readonly IView[] _views;

    public DefaultViewService(IView[] views)
    {
        _views = views;
    }

    public IReadOnlyCollection<IView> GetEditorViews()
    {
        return _views;
    }

    public void RenderViews()
    {
        foreach (IView view in _views)
        {
            view.Render();
        }
    }
}