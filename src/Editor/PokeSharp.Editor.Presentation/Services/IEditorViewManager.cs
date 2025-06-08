namespace PokeSharp.Editor.Presentation.Services;

public interface IEditorViewManager
{
    /// <summary>
    /// Register the specified view
    /// </summary>
    /// <param name="view">The view to be registered</param>
    void RegisterView(IEditorView view);

    /// <summary>
    /// Unregister the view at the specified viewId
    /// </summary>
    /// <param name="viewId">The view id to unregister</param>
    void UnregisterView(string viewId);

    /// <summary>
    /// Render all registered views
    /// </summary>
    void RenderViews();

    /// <summary>
    /// Gets all the registered editor view
    /// </summary>
    /// <returns>A read-only collection of the registered views</returns>
    IReadOnlyCollection<IEditorView> GetEditorViews();
}