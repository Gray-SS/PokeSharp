namespace PokeLab.Presentation.ContentBrowser;

public interface IContentBrowserView : IView
{
    string SearchText { get; set; }
    event Action<string>? SearchTextChanged;

    void RefreshView();
    void SetItems(IEnumerable<ContentModel> items);
}