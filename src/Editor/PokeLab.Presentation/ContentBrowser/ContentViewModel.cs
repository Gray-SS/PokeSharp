namespace PokeLab.Presentation.ContentBrowser;

public sealed class ContentViewModel
{
    public string Name { get; }

    public ContentViewModel(string name)
    {
        Name = name;
    }
}