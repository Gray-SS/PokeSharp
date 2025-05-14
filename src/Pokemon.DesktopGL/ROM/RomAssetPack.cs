using System.Collections.Generic;
using Pokemon.DesktopGL.ROM.Graphics;

namespace Pokemon.DesktopGL.ROM;

/// <summary>
/// This asset pack must have all the abstracted assets required
/// to start loading them and use them to build & run the project
/// </summary>
public sealed class RomAssetsPack
{
    // Sounds

    // Sprites

    // Palettes

    // Spritesheets

    // Animations

    // Objects
    public EntityGraphicsInfo PlayerEntityGraphicsInfo { get; set; }
    public List<EntityGraphicsInfo> NpcsEntityGraphicsInfo { get; set; }

    // Pokemons Data

    // Items Data

    public RomAssetsPack()
    {
        NpcsEntityGraphicsInfo = new List<EntityGraphicsInfo>();
    }
}