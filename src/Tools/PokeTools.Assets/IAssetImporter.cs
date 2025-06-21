using PokeCore.IO;
using PokeCore.Common;

namespace PokeTools.Assets;

public interface IAssetImporter
{
    string SupportedExtensions { get; }

    Type ProcessorType { get; }

    Result<object> Import(IVirtualFile file);
}