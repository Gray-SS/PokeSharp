namespace PokeSharp.Editor.Presentation;

public interface IEditorView
{
    /// <summary>
    /// Gets the unique identifier of this view (<i>e.g.</i> <c>Console</c>, <c>AssetBrowser</c>, ect.).
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the title of the view
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Determines if the view is visible.
    /// </summary>
    bool IsVisible { get; set; }

    /// <summary>
    /// Method called each frame to render the view
    /// </summary>
    void Render();
}