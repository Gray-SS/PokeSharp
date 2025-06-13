using PokeLab.Presentation.ContentBrowser;

namespace PokeLab.Presentation.ImGui.ContentBrowser;

public sealed class ImGuiContentBrowserView : IContentBrowserView
{
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value) return;

            _searchText = value;
            SearchTextChanged?.Invoke(value);
        }
    }

    private string _searchText = string.Empty;

    public event Action<string>? SearchTextChanged;

    public string Id => "ContentBrowser";
    public string Title => "Content Browser";
    public bool IsVisible { get; set; }

    public void RefreshView()
    {
    }

    public void Render()
    {
        if (Gui.Begin(Title))
        {
            Gui.Text("Hello, World !");

            Gui.PushID("search_text");

            string searchText = SearchText;
            if (Gui.InputText("Search Text", ref searchText, 256))
            {
                SearchText = searchText;
            }

            Gui.PopID();
        }

        Gui.End();
    }

    public void SetItems(IEnumerable<ContentModel> items)
    {
    }
}
