using PokeCore.Assets;
using PokeCore.Common;

namespace PokeTools.Assets;

public sealed class AssetPipeline : IAssetPipeline
{
    public AssetType AssetType => Importer.Metadata.AssetType;
    public IAssetImporter Importer { get; }
    public IAssetProcessor Processor { get; }
    public IAssetCompiler Compiler { get; }

    public AssetPipeline(IAssetImporter importer, IAssetProcessor processor, IAssetCompiler compiler)
    {
        Importer = importer;
        Processor = processor;
        Compiler = compiler;
    }

    public async Task<Result> BuildAsync(AssetMetadata metadata, Stream inputStream, Stream outputStream)
    {
        Result<object> importResult = Importer.Import(inputStream);
        if (importResult.TryGetError(out Error? error))
            return Result.Failure(error);

        object rawAsset = importResult.GetValue();
        Result<IAsset> processResult = Processor.Process(metadata.Id, rawAsset);
        if (processResult.TryGetError(out error))
            return Result.Failure(error);

        IAsset asset = processResult.GetValue();

        using BinaryWriter writer = new(outputStream);
        writer.Write(asset.Id.ToString());
        writer.Write((byte)asset.AssetType);

        Result compileResult = Compiler.Compile(asset, writer);
        if (compileResult.TryGetError(out error))
            return Result.Failure(error);

        writer.Flush();
        await outputStream.FlushAsync();

        if (outputStream.Position == 0)
            return Result.Failure(new Error("Compilation failed. No bytes wrote to the output stream."));

        await writer.DisposeAsync();
        return Result.Success();
    }
}