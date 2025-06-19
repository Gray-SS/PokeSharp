using PokeCore.IO;

namespace PokeTools.Assets;

public interface IAssetImporter
{
    string SupportedExtensions { get; }

    Type ProcessorType { get; }

    Result<object, string> Import(IVirtualFile file);
}