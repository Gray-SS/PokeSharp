using PokeCore.IO;
using PokeCore.Common;

namespace PokeTools.Assets;

public interface IAssetPipeline
{
    Task<Result> ImportAsync(VirtualPath path);
}