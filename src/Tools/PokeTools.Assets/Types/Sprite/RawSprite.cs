using System.Drawing;
using PokeTools.Assets.Core;

namespace PokeTools.Assets.Types.Sprite;

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