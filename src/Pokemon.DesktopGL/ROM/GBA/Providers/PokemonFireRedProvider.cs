using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Pokemon.DesktopGL.ROM.Graphics;

namespace Pokemon.DesktopGL.ROM.GBA.Providers;

public sealed class PokemonFireRedProvider : GbaPokemonRomProvider
{
    private readonly Dictionary<int, IGraphicsPalette> _palettes;

    public PokemonFireRedProvider(byte[] romData, Dictionary<GbaRomOffsetPointers, int> offsets) : base(romData, offsets)
    {
        _palettes = new Dictionary<int, IGraphicsPalette>();
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

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private readonly struct SpriteFrameImage
    {
        [FieldOffset(0x0)]
        public readonly int DataPtr;

        [FieldOffset(0x4)]
        public readonly int Size;
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
        int spritesPointer = Reader.ReadPointer(entryOffset, 0x1C);

        IGraphicsPalette palette = GetOrExtractEntityPalette(paletteTag);
        int spritesBaseOffset = AddressResolver.ToPhysical(spritesPointer);

        int spriteFrameSize = Marshal.SizeOf<SpriteFrameImage>();
        SpriteFrameImage firstSFI = Reader.ReadStruct<SpriteFrameImage>(spritesBaseOffset, spriteFrameSize * 0x00);
        int firstSpriteBaseOffset = AddressResolver.ToPhysical(firstSFI.DataPtr);

        SpriteFrameImage lastSFI = Reader.ReadStruct<SpriteFrameImage>(spritesBaseOffset, spriteFrameSize * 0x09);
        int lastSpriteBaseOffset = AddressResolver.ToPhysical(lastSFI.DataPtr);

        int size = lastSpriteBaseOffset - firstSpriteBaseOffset;

        IGraphicsData spritesheet = ExtractSpriteSheet(firstSpriteBaseOffset, size, width, height, palette);
        return new EntityGraphicsInfo(width, height, shadowSize, palette, spritesheet);
    }

    private IGraphicsData ExtractSpriteSheet(int baseOffset, int size, int width, int height, IGraphicsPalette palette)
    {
        byte[] rawData = Reader.ReadBytes(baseOffset, size);
        return new Gba4bppGraphicsData(width * 9, height, rawData, palette);
    }

    private IGraphicsPalette GetOrExtractEntityPalette(short paletteTag)
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

    private IGraphicsPalette GetOrExtractPalette(int paletteOffset)
    {
        if (_palettes.TryGetValue(paletteOffset, out IGraphicsPalette palette))
            return palette;

        ReadOnlySpan<byte> paletteData = Reader.ReadSpan(paletteOffset, GbaGraphicsPalette.BYTES_COUNT);
        palette = GbaGraphicsPalette.FromRawData(paletteData, true);

        _palettes[paletteOffset] = palette;
        return palette;
    }

    public override IGraphicsData ExtractPokemonIconSprite(int index)
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

    public override IGraphicsData ExtractItemIconSprite(int index)
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

    public override IGraphicsData ExtractBackPokemonSprite(int index)
    {
        // int gfxOffset = GetFileOffsetFromPtr(GbaRomOffsetKind.BACK_POKEMON_SPRITES, index);
        // byte[] rawGraphics = DecompressLZ77(gfxOffset);

        // int paletteOffset = GetFileOffsetFromPtr(GbaRomOffsetKind.REGULAR_PALETTES, index);
        // byte[] paletteData = DecompressLZ77(paletteOffset);
        // IGraphicsPalette palette = GbaGraphicsPalette.FromRawData(paletteData, true);

        // return new Gba4bppGraphicsData(64, 64, rawGraphics, palette);
        throw new NotImplementedException();
    }

    public override IGraphicsData ExtractFrontPokemonSprite(int index)
    {
        // int gfxOffset = GetFileOffsetFromPtr(GbaRomOffsetKind.FRONT_POKEMON_SPRITES, index);
        // byte[] rawGraphics = DecompressLZ77(gfxOffset);

        // int paletteOffset = GetFileOffsetFromPtr(GbaRomOffsetKind.REGULAR_PALETTES, index);
        // byte[] paletteData = DecompressLZ77(paletteOffset);
        // IGraphicsPalette palette = GbaGraphicsPalette.FromRawData(paletteData, true);

        // return new Gba4bppGraphicsData(64, 64, rawGraphics, palette);
        throw new NotImplementedException();
    }
}