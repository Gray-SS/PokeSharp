using PokeCore.Assets;
using PokeCore.Common.Results;
using PokeTools.Assets.Core;
using PokeTools.Assets.Models;
using PokeTools.Assets.Pipeline.Abstractions;
using PokeTools.Assets.Services.Abstractions;

namespace PokeTools.Assets.Services;

public sealed class AssetPipelineExecutor : IAssetPipelineExecutor
{
    public async Task<Result<IAsset>> ExecuteAsync(
        AssetPipelineDefinition definition,
        AssetMetadata metadata,
        Stream inputStream,
        Stream outputStream
    )
    {
        definition.Deconstruct(
            out IAssetImporter importer,
            out IAssetProcessor processor,
            out IAssetSerializer serializer
        );

        Result<IRawAsset> importResult = importer.Import(inputStream);
        if (importResult.IsFailure)
            return Result.Failure(importResult.Error);

        IRawAsset rawAsset = importResult.Data;
        IEnumerable<Guid> dependencies = rawAsset.GetDependencies();

        Result<IAsset> processResult = processor.Process(metadata.Id, rawAsset);
        if (processResult.IsFailure)
            return Result.Failure(processResult.Error);

        IAsset asset = processResult.Data;

        using BinaryWriter writer = new(outputStream);
        writer.Write(asset.Id.ToString());
        writer.Write((byte)asset.AssetType);

        writer.Write(dependencies.Count());
        foreach (Guid id in dependencies)
            writer.Write(id.ToString());

        Result<Unit> serializeResult = serializer.Serialize(asset, writer);
        if (serializeResult.IsFailure)
            return Result.Failure(serializeResult.Error);

        writer.Flush();
        await outputStream.FlushAsync();

        if (outputStream.Position == 0)
            return Result.Failure("Compilation failed. No bytes wrote to the output stream.");

        await writer.DisposeAsync();

        return Result.Success(asset);
    }
}