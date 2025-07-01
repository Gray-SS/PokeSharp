using PokeCore.Assets;
using PokeCore.Common;

namespace PokeTools.Assets;

public sealed class AssetPipeline : IAssetPipeline
{
    public AssetType AssetType => Importer.Metadata.AssetType;
    public IAssetImporter Importer { get; }
    public IAssetProcessor Processor { get; }
    public IAssetSerializer Serializer { get; }

    public AssetPipeline(IAssetImporter importer, IAssetProcessor processor, IAssetSerializer serializer)
    {
        Importer = importer;
        Processor = processor;
        Serializer = serializer;
    }

    public async Task<Result> BuildAsync(AssetMetadata metadata, Stream inputStream, Stream outputStream)
    {
        Result<IRawAsset> importResult = Importer.Import(inputStream);
        if (importResult.TryGetError(out Error? error))
            return Result.Failure(error);

        IRawAsset rawAsset = importResult.GetValue();
        IEnumerable<Guid> dependencies = rawAsset.GetDependencies();

        Result<IAsset> processResult = Processor.Process(metadata.Id, rawAsset);
        if (processResult.TryGetError(out error))
            return Result.Failure(error);

        IAsset asset = processResult.GetValue();

        using BinaryWriter writer = new(outputStream);
        writer.Write(asset.Id.ToString());
        writer.Write((byte)asset.AssetType);

        writer.Write(dependencies.Count());
        foreach (Guid id in dependencies)
            writer.Write(id.ToString());

        Result serializeResult = Serializer.Serialize(asset, writer);
        if (serializeResult.TryGetError(out error))
            return Result.Failure(error);

        writer.Flush();
        await outputStream.FlushAsync();

        if (outputStream.Position == 0)
            return Result.Failure(new Error("Compilation failed. No bytes wrote to the output stream."));

        await writer.DisposeAsync();
        return Result.Success();
    }
}