using PokeCore.IO;

namespace PokeEngine.Assets;

public interface IAssetImporter
{
    string SupportedExtensions { get; }

    Type ProcessorType { get; }

    object? Import(IVirtualFile file);
}