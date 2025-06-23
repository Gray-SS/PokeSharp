using PokeCore.Common;

namespace PokeTools.Assets;

public interface IAssetImporter
{
    Type ProcessorType { get; }
    string[] SupportedExtensions { get; }

    Result<object> Import(Stream stream);
}