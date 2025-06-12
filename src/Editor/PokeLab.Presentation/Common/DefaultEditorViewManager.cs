using PokeCore.DependencyInjection.Abstractions;

namespace PokeLab.Presentation.Common;

public sealed class DefaultEditorViewManager : IEditorViewManager
{
    private readonly IEditorView[] _views;

    public DefaultEditorViewManager(IServiceContainer services)
    {
        _views = services.GetServices<IEditorView>().ToArray();
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