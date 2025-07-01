using System.Drawing;

namespace PokeTools.Assets.Intermediate;

public sealed class RawSprite : IRawAsset
{
    public Guid? TextureId { get; set; }
    public Rectangle? TextureRegion { get; set; }

    public IEnumerable<Guid> GetDependencies()
    {
        if (TextureId != null)
            yield return TextureId.Value;
    }
}