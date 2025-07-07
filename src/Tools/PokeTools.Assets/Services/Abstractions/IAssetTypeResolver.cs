using PokeCore.Assets;
using PokeCore.IO;

namespace PokeTools.Assets.Services.Abstractions;

public interface IAssetTypeResolver
{
    AssetType ResolveTypeFromPath(VirtualPath path);
}