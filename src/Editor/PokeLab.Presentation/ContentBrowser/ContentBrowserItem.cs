using PokeCore.IO;
using PokeLab.Domain.Assets;

namespace PokeLab.Presentation.ContentBrowser;

public abstract class ContentBrowserItem
{
    public string Name { get; }

    public ContentBrowserItem(string name)
    {
        Name = name;
    }
}

public sealed class ContentBrowserFolderItem : ContentBrowserItem
{
    public VirtualPath Path { get; }

    public ContentBrowserFolderItem(string name, VirtualPath path) : base(name)
    {
        Path = path;
    }
}

public sealed class ContentBrowserAssetItem : ContentBrowserItem
{
    public AssetMetadata AssetMetadata { get; }

    public ContentBrowserAssetItem(string name, AssetMetadata assetMetadata) : base(name)
    {
        AssetMetadata = assetMetadata;
    }
}