using PokeSharp.Assets.VFS;

namespace PokeSharp.Assets;

public interface IAssetImporter
{
    string SupportedExtensions { get; }

    Type ProcessorType { get; }

    object? Import(IVirtualFile file);
}