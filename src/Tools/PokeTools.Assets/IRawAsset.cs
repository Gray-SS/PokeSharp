namespace PokeTools.Assets;

public interface IRawAsset
{
    IEnumerable<Guid> GetDependencies();
}