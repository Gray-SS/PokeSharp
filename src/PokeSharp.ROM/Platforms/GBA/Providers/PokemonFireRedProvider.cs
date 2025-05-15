using PokeSharp.ROM.Graphics;
using System.Runtime.InteropServices;


namespace PokeSharp.ROM.Platforms.GBA.Providers;

public sealed class PokemonFireRedProvider : GbaPokemonRomProvider
{
    private const int MAX_FRAMES_COUNT = 9;

    private readonly Dictionary<int, IRomPalette> _palettes;

    public PokemonFireRedProvider(byte[] romData, Dictionary<GbaRomOffsetPointers, int> offsets) : base(romData, offsets)
    {
        _palettes = new Dictionary<int, IRomPalette>();
    }

    public override RomAssetsPack ExtractAssetPack()
    {
        var assetsPack = new RomAssetsPack();
        assetsPack.PlayerEntityGraphicsInfo = ExtractEntityGraphicsInfo(0);

        return assetsPack;
    }

    public override string ExtractPokemonName(int index)
    {
        int offsetPtr = GetOffsetPointer(GbaRomOffsetPointers.POKEMON_NAMES);
        int baseOffset = AddressResolver.ToPhysical(offsetPtr);
        int entryOffset = baseOffset + index * 11;

        ReadOnlySpan<byte> text = Reader.ReadSpan(entryOffset, 11);
        return DecodeGbaText(text);
    }

    public override EntityGraphicsInfo ExtractEntityGraphicsInfo(int index)
    {
        int offsetPtr = GetOffsetPointer(GbaRomOffsetPointers.ENTITY_GRAPHICS_INFO);
        int baseOffset = AddressResolver.ToPhysical(offsetPtr);
        int entryOffset = baseOffset + index * 0x24;

        short paletteTag = Reader.ReadInt16(entryOffset, 0x02);
        ushort width = Reader.ReadUInt16(entryOffset, 0x08);
        ushort height = Reader.ReadUInt16(entryOffset, 0x0A);
        int attrFlags = Reader.ReadInt32(entryOffset, 0x0C);
        int shadowSize = (attrFlags >> 4) & 0b11;

        IRomPalette palette = GetOrExtractEntityPalette(paletteTag);

        int imagesPointer = Reader.ReadPointer(entryOffset, 0x1C);
        RomSpriteSheet spriteSheet = ExtractEntitySpriteSheet(imagesPointer, width, height, palette);

        int animsPointer = Reader.ReadPointer(entryOffset, 0x18);
        GraphicsAnimation[] animations = ExtractEntityAnimations(animsPointer);

        return new EntityGraphicsInfo(width, height, shadowSize, palette, spriteSheet, animations);
    }

    private int GetFramesCount(int imagesPointer)
    {
        int framesCount = 0;
        int baseOffset = AddressResolver.ToPhysical(imagesPointer);
        int spriteFrameImageSize = Marshal.SizeOf<SpriteFrameImage>();

        for (int i = 0; i < MAX_FRAMES_COUNT; i++)
        {
            SpriteFrameImage sfi = Reader.Read<SpriteFrameImage>(baseOffset, spriteFrameImageSize * i);

            if (sfi.DataPtr == 0 || sfi.Size == 0)
                break;

            framesCount++;
        }

        return framesCount == 0 ? 1 : framesCount;
    }

    private RomSpriteSheet ExtractEntitySpriteSheet(int imagesPointer, ushort width, ushort height, IRomPalette palette)
    {
        int baseOffset = AddressResolver.ToPhysical(imagesPointer);
        int framesCount = GetFramesCount(imagesPointer);
        int frameDataLength = width * height / 2;

        byte[] rawData = new byte[frameDataLength * framesCount];
        ReadOnlySpan<SpriteFrameImage> frames = Reader.ReadArrayRef<SpriteFrameImage>(baseOffset, framesCount);

        for (int i = 0; i < frames.Length; i++)
        {
            ref readonly SpriteFrameImage sfi = ref frames[i];

            int spriteDataOffset = AddressResolver.ToPhysical(sfi.DataPtr);
            ReadOnlySpan<byte> frameSegment = Reader.ReadSpan(spriteDataOffset, frameDataLength);

            Span<byte> rawDataSegment = rawData.AsSpan(frameDataLength * i, frameDataLength);
            frameSegment.CopyTo(rawDataSegment);
        }

        var image = new Rom4bppTexture(width, height * framesCount, rawData, palette);
        return new RomSpriteSheet(image, 1, framesCount);
    }

    private GraphicsAnimation[] ExtractEntityAnimations(int animsPointer)
    {
        

        return [];
    }

    private IRomPalette GetOrExtractEntityPalette(short paletteTag)
    {
        int offsetPtr = GetOffsetPointer(GbaRomOffsetPointers.ENTITY_PALETTES);
        int baseOffset = AddressResolver.ToPhysical(offsetPtr);
        int crntOffset = baseOffset;

        while (crntOffset + 8 < RomData.Length)
        {
            int palettePointer = Reader.ReadPointer(crntOffset, 0x00);
            short foundPaletteTag = Reader.ReadInt16(crntOffset, 0x04);

            if (palettePointer == 0x00)
                continue;

            if (foundPaletteTag == paletteTag)
            {
                int paletteOffset = AddressResolver.ToPhysical(palettePointer);
                return GetOrExtractPalette(paletteOffset);
            }

            crntOffset += 8;
        }

        throw new InvalidOperationException("Couldn't load the sprite palette from the palette tag");
    }

    private IRomPalette GetOrExtractPalette(int paletteOffset)
    {
        if (_palettes.TryGetValue(paletteOffset, out IRomPalette? palette))
            return palette;

        ReadOnlySpan<byte> paletteData = Reader.ReadSpan(paletteOffset, Rom4bppPalette.BYTES_COUNT);
        palette = Rom4bppPalette.FromRawData(paletteData, true);

        _palettes[paletteOffset] = palette;
        return palette;
    }

    public override IRomTexture ExtractPokemonIconSprite(int index)
    {
        // int gfxOffset = GetFileOffsetFromPtr(GbaRomOffsetKind.ICON_SPRITES, index);
        // byte[] rawData = RomData[gfxOffset..(gfxOffset + 1024)];

        // int paletteDataOffset = GetFileOffset(GbaRomOffsetKind.ICON_PALETTE_DATA, index, 1);
        // byte paletteIndex = RomData[paletteDataOffset];

        // if (paletteIndex < 0 || paletteIndex >= 3)
        //     throw new InvalidOperationException("Extracted palette index is wrong. This could be an offset issue.");

        // int paletteOffset = GetFileOffset(GbaRomOffsetKind.ICON_PALETTES, paletteIndex, 32);
        // byte[] paletteData = RomData[paletteOffset..(paletteOffset + 32)];
        // IGraphicsPalette palette = GbaGraphicsPalette.FromRawData(paletteData, true);

        // return new Gba4bppGraphicsData(32, 64, rawData, palette);
        throw new NotImplementedException();
    }

    public override IRomTexture ExtractItemIconSprite(int index)
    {
        // index *= 2;
        // int gfxOffset = GetFileOffsetFromPtr(GbaRomOffsetKind.ITEM_SPRITES, index);
        // byte[] rawData = DecompressLZ77(gfxOffset);

        // int paletteOffset = GetFileOffsetFromPtr(GbaRomOffsetKind.ITEM_SPRITES, index + 1);
        // byte[] paletteData = DecompressLZ77(paletteOffset);
        // IGraphicsPalette palette = GbaGraphicsPalette.FromRawData(paletteData, true);

        // return new Gba4bppGraphicsData(24, 24, rawData, palette);

        throw new NotImplementedException();
    }

    public override IRomTexture ExtractBackPokemonSprite(int index)
    {
        // int gfxOffset = GetFileOffsetFromPtr(GbaRomOffsetKind.BACK_POKEMON_SPRITES, index);
        // byte[] rawGraphics = DecompressLZ77(gfxOffset);

        // int paletteOffset = GetFileOffsetFromPtr(GbaRomOffsetKind.REGULAR_PALETTES, index);
        // byte[] paletteData = DecompressLZ77(paletteOffset);
        // IGraphicsPalette palette = GbaGraphicsPalette.FromRawData(paletteData, true);

        // return new Gba4bppGraphicsData(64, 64, rawGraphics, palette);
        throw new NotImplementedException();
    }

    public override IRomTexture ExtractFrontPokemonSprite(int index)
    {
        // int gfxOffset = GetFileOffsetFromPtr(GbaRomOffsetKind.FRONT_POKEMON_SPRITES, index);
        // byte[] rawGraphics = DecompressLZ77(gfxOffset);

        // int paletteOffset = GetFileOffsetFromPtr(GbaRomOffsetKind.REGULAR_PALETTES, index);
        // byte[] paletteData = DecompressLZ77(paletteOffset);
        // IGraphicsPalette palette = GbaGraphicsPalette.FromRawData(paletteData, true);

        // return new Gba4bppGraphicsData(64, 64, rawGraphics, palette);
        throw new NotImplementedException();
    }

    #region Helper Structs

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private readonly struct SpriteFrameImage
    {
        [FieldOffset(0x0)]
        public readonly int DataPtr;

        [FieldOffset(0x4)]
        public readonly int Size;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private readonly struct AnimFrameCmd
    {
        [FieldOffset(0x0)]
        public readonly uint packedValue;

        public ushort ImageValue => (ushort)(packedValue & 0xFFFF);         // bits 0–15 (16 bits)
        public byte Duration => (byte)((packedValue >> 16) & 0b0011_1111);  // bits 16–21 (6 bits)
        public bool HFlip => ((packedValue >> 22) & 0b1) != 0;              // bit 22 (1 bit)
        public bool VFlip => ((packedValue >> 23) & 0b1) != 0;              // bit 23 (1 bit)
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private readonly struct AnimLoopCmd
    {
        [FieldOffset(0x0)]
        public readonly uint packedValue;

        public ushort Type => (ushort)(packedValue & 0xFFFF);               // bits 0–15 (16 bits)
        public ushort Count => (ushort)((packedValue >> 16) & 0b0011_1111); // bits 16-21 (6 bits)
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private readonly struct AnimJumpCmd
    {
        [FieldOffset(0x0)]
        public readonly uint packedValue;

        public ushort Type => (ushort)(packedValue & 0xFFFF);                // bits 0–15 (16 bits)
        public ushort Target => (ushort)((packedValue >> 16) & 0b0011_1111); // bits 16-21 (6 bits)
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private readonly struct AnimCmd
    {
        [FieldOffset(0x0)]
        public readonly short type;

        [FieldOffset(0x02)]
        public readonly AnimFrameCmd frame;

        [FieldOffset(0x02)]
        public readonly AnimJumpCmd jump;

        [FieldOffset(0x02)]
        public readonly AnimLoopCmd loop;
    }

    #endregion
}