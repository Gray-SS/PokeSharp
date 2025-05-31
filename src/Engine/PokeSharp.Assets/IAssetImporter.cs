using PokeSharp.Assets.VFS;

namespace PokeSharp.Assets;

public interface IAssetImporter
{
    Type ProcessorType { get; }

    bool CanImport(VirtualPath path);
    object? Import(IVirtualFile file);
}