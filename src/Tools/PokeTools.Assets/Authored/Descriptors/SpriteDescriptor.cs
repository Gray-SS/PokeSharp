using System.Drawing;

namespace PokeTools.Assets.Authored.Descriptors;

public record struct SpriteDescriptor(
    Guid? TextureId,
    Rectangle? SourceRect
);