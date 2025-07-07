namespace PokeTools.Assets.Core;

public interface IRawAsset
{
    IEnumerable<Guid> GetDependencies();
}