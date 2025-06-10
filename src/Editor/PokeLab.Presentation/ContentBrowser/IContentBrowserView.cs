namespace PokeLab.Presentation.ContentBrowser;

public interface IContentBrowserView : IEditorView
{
    string SearchText { get; set; }

    void RefreshView();
    void SetItems(IEnumerable<ContentViewModel> items);
}