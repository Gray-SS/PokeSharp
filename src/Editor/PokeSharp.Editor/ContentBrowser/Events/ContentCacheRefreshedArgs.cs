namespace PokeSharp.Editor.ContentBrowser.Events;

public sealed class ContentCacheRefreshedArgs : EventArgs
{
    public ContentScope RefreshScope { get; }

    public ContentCacheRefreshedArgs(ContentScope refreshScope)
    {
        RefreshScope = refreshScope;
    }
}