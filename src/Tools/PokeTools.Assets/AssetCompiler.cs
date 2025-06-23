namespace PokeTools.Assets;

public abstract class AssetCompiler<TProcessed> : IAssetCompiler
    where TProcessed : class
{
    public abstract void Write(TProcessed asset, string path);

    void IAssetCompiler.Compile(object asset, string path)
    {
    }
}