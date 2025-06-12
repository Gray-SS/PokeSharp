using PokeLab.Presentation.ContentBrowser;

namespace PokeLab.Presentation.ImGui.ContentBrowser;

public sealed class ImGuiContentBrowserView : IContentBrowserView
{
    public string SearchText { get; set; } = string.Empty;

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
        }

        Gui.End();
    }

    public void SetItems(IEnumerable<ContentViewModel> items)
    {
    }
}
