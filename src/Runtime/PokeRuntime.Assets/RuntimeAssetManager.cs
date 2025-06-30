using PokeCore.Assets;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeRuntime.Assets;

public sealed class RuntimeAssetManager(
    IServiceResolver service
) : IAssetManager
{
    public T Load<T>(string assetPath) where T : IRuntimeAsset
    {
        using Stream stream = File.OpenRead(assetPath);
        using BinaryReader reader = new(stream);

        if (!service.TryGetService(out IRuntimeAssetLoader<T>? loader))
            throw new InvalidOperationException($"The asset loader for asset '{typeof(T).Name}' isn't registered.");

        Guid id = Guid.Parse(reader.ReadString());
        AssetType type = (AssetType)reader.ReadByte();
        return loader.Load(id, reader);
    }
}