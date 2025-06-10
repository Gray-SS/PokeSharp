using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Services;

namespace PokeLab.Services;

public sealed class EditorViewManager : IEditorViewManager
{
    private readonly Logger _logger;
    private readonly IEditorView[] _views;

    public EditorViewManager(Logger logger, IReflectionManager reflectionManager)
    {
        _logger = logger;
        _views = reflectionManager.InstantiateClassesOfType<IEditorView>();

        _logger.Debug($"{_views.Length} views registered.");
        foreach (IEditorView view in _views)
            _logger.Trace($"View {view.GetType().Name}");
    }

    public void Draw()
    {
        foreach (IEditorView view in _views)
        {
            view.DrawGui();
        }
    }
}