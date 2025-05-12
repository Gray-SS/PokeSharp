using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon.DesktopGL.ROM.GBA.Providers;

public sealed class PokemonFireRedProvider : GbaPokemonRomProvider
{
    public PokemonFireRedProvider(RomInfo romInfo, byte[] romData, Dictionary<GbaRomOffsetKind, int> offsets) : base(romInfo, romData, offsets)
    {
    }

    public override string GetPokemonName(int index)
    {
        int offset = GetFileOffset(GbaRomOffsetKind.POKEMON_NAMES);
        int entryOffset = offset + index * 11;

        Span<byte> nameBytes = Data.AsSpan(entryOffset, 11);
        return DecodePokemonText(nameBytes);
    }

    public string LoadCreatureName(int index)
    {
        int baseCreatureNameOffset = GetFileOffset(0x08245EE0);
        int entryOffset = baseCreatureNameOffset + index * 11;

        Span<byte> nameBytes = Data.AsSpan(entryOffset, 11);
        return DecodePokemonText(nameBytes);
    }

    private static string DecodePokemonText(Span<byte> data)
    {
        var sb = new StringBuilder();
        foreach (byte b in data)
        {
            if (b == 0xFF || b == 0x00) break;

            if (b >= 0xBB && b <= 0xD4)
                sb.Append((char)('a' + (b - 0xBB)));
            else if (b >= 0xA1 && b <= 0xBA)
                sb.Append((char)('A' + (b - 0xA1)));
            // else if (_charset.TryGetValue(b, out char c))
            //     sb.Append(c);
            else
                sb.Append('?');
        }
        return sb.ToString();
    }

    // public CreatureData LoadCreatureData(int index)
    // {
    //     int baseStatsOffset = GetFileOffset(0x08254784);
    //     int entryOffset = baseStatsOffset + index * 28;

    //     var stats = new Stats
    //     {
    //         HP = RomData[entryOffset + 0],
    //         Attack = RomData[entryOffset + 1],
    //         Defense = RomData[entryOffset + 2],
    //         Speed = RomData[entryOffset + 3],
    //         SpAttack = RomData[entryOffset + 4],
    //         SpDefense = RomData[entryOffset + 5]
    //     };

    //     var type1 = (CreatureType)RomData[entryOffset + 6];
    //     byte? type2 = RomData[entryOffset + 7];

    //     int catchRate = BitConverter.ToUInt16(RomData, entryOffset + 8);
    //     int baseEXP = BitConverter.ToUInt16(RomData, entryOffset + 10);
    //     var growthRate = (CreatureGrowthRate)RomData[entryOffset + 19];

    //     return new CreatureData
    //     {
    //         Id = $"test_{index}",
    //         Name = LoadCreatureName(index),
    //         BaseEXP = baseEXP,
    //         BaseStats = stats,
    //         CatchRate = catchRate,
    //         GrowthRate = growthRate,
    //         LearnableMoveIds = [],
    //         Type1 = type1,
    //         Type2 = type2.HasValue ? (CreatureType)type2.Value : CreatureType.None,
    //         BackSpritePath = "",
    //         FrontSpritePath = ""
    //     };
    // }

    // public string LoadCreatureName(int index)
    // {
    //     int baseCreatureNameOffset = GetFileOffset(0x08245EE0);
    //     int entryOffset = baseCreatureNameOffset + index * 11;

    //     Span<byte> nameBytes = RomData.AsSpan(entryOffset, 11);
    //     return DecodePokemonText(nameBytes);
    // }

    // private static string DecodePokemonText(Span<byte> data)
    // {
    //     var sb = new StringBuilder();
    //     foreach (byte b in data)
    //     {
    //         if (b == 0xFF || b == 0x00) break;

    //         if (b >= 0xBB && b <= 0xD4)
    //             sb.Append((char)('a' + (b - 0xBB)));
    //         else if (b >= 0xA1 && b <= 0xBA)
    //             sb.Append((char)('A' + (b - 0xA1)));
    //         else if (_charset.TryGetValue(b, out char c))
    //             sb.Append(c);
    //         else
    //             sb.Append('?');
    //     }
    //     return sb.ToString();
    // }

    // public Texture2D LoadSprite(GraphicsDevice graphics, int index)
    // {
    //     int fileOffset = GetFileOffset(FRONT_SPRITE_TABLE_OFFSET);
    //     int spritePointer = BitConverter.ToInt32(RomData, fileOffset + (index * 4));
    //     int spriteDataOffset = GetFileOffset(spritePointer);

    //     byte[] spriteData = DecompressLZ77(RomData, spriteDataOffset);
    //     Color[] palette = LoadPalette(index);

    //     Texture2D texture = CreateTextureFrom4bppTiles(graphics, spriteData, 64, 64, palette);
    //     return texture;
    // }

    // public static Texture2D CreateTextureFrom4bppTiles(GraphicsDevice graphics, byte[] tileData, int width, int height, Color[] palette)
    // {
    //     int tileWidth = 8;
    //     int tileHeight = 8;
    //     int tilesPerRow = width / tileWidth;

    //     Color[] pixels = new Color[width * height];
    //     int tileIndex = 0;

    //     for (int ty = 0; ty < height / tileHeight; ty++)
    //     {
    //         for (int tx = 0; tx < tilesPerRow; tx++)
    //         {
    //             for (int row = 0; row < tileHeight; row++)
    //             {
    //                 for (int col = 0; col < tileWidth; col += 2)
    //                 {
    //                     byte packed = tileData[tileIndex++];
    //                     int x = tx * tileWidth + col;
    //                     int y = ty * tileHeight + row;

    //                     byte lowNibble = (byte)(packed & 0xF);
    //                     byte highNibble = (byte)(packed >> 4);

    //                     pixels[y * width + x] = palette[lowNibble];
    //                     pixels[y * width + x + 1] = palette[highNibble];
    //                 }
    //             }
    //         }
    //     }

    //     var texture = new Texture2D(graphics, width, height);
    //     texture.SetData(pixels);
    //     return texture;
    // }

    // public Color[] LoadPalette(int index)
    // {
    //     int paletteTableOffset = GetFileOffset(0x0823730C);
    //     int palettePointer = BitConverter.ToInt32(RomData, paletteTableOffset + (index * 4));
    //     int paletteOffset = GetFileOffset(palettePointer);

    //     byte[] paletteData = DecompressLZ77(RomData, paletteOffset);

    //     Color[] colors = new Color[16];
    //     for (int i = 0; i < 16; i++)
    //     {
    //         if (i == 0)
    //         {
    //             colors[i] = Color.Transparent;
    //             continue;
    //         }

    //         ushort entry = BitConverter.ToUInt16(paletteData, i * 2);
    //         int r = (entry & 0x1F) << 3;
    //         int g = ((entry >> 5) & 0x1F) << 3;
    //         int b = ((entry >> 10) & 0x1F) << 3;
    //         colors[i] = new Color(r, g, b);
    //     }

    //     return colors;
    // }

    // public static int GetFileOffset(int offset)
    //     => offset - FILE_OFFSET;

    // public static byte[] DecompressLZ77(byte[] data, int offset)
    // {
    //     if (data[offset] != 0x10)
    //         throw new InvalidOperationException("Data is not LZ77 compressed.");

    //     int decompressedSize = data[offset + 1] | (data[offset + 2] << 8) | (data[offset + 3] << 16);
    //     var result = new List<byte>(decompressedSize);

    //     int pos = offset + 4;

    //     while (result.Count < decompressedSize)
    //     {
    //         byte flag = data[pos++];
    //         for (int i = 0; i < 8 && result.Count < decompressedSize; i++)
    //         {
    //             if ((flag & (0x80 >> i)) == 0)
    //             {
    //                 result.Add(data[pos++]);
    //             }
    //             else
    //             {
    //                 byte b1 = data[pos++];
    //                 byte b2 = data[pos++];
    //                 int length = ((b1 >> 4) & 0xF) + 3;
    //                 int disp = ((b1 & 0xF) << 8) | b2;

    //                 int copyPos = result.Count - disp - 1;
    //                 for (int j = 0; j < length; j++)
    //                 {
    //                     result.Add(result[copyPos + j]);
    //                 }
    //             }
    //         }
    //     }

    //     return [.. result];
    // }
}