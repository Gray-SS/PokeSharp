namespace PokeLab.Presentation.Common;

public interface IEditorViewManager
{
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