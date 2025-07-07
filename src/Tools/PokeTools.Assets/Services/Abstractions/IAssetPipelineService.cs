using PokeCore.IO;
using PokeCore.Common;
using PokeCore.Common.Results;

namespace PokeTools.Assets.Services.Abstractions;

public interface IAssetPipelineService
{
    Result<Unit> BuildBundle(VirtualPath dirPath);
    Task<Result<Unit>> BuildAsync(VirtualPath inputPath, VirtualPath outputPath);
}