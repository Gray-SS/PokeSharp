using PokeCore.IO;
using PokeCore.Assets;
using PokeCore.Common;

namespace PokeTools.Assets;

public interface IAssetBuilder
{
    bool CanBuild(string extension);
    Result<IAsset> Build(VirtualPath path);
}