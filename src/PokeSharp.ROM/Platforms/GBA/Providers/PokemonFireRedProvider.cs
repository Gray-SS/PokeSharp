using PokeSharp.ROM.Descriptors;
using PokeSharp.ROM.Graphics;
using PokeSharp.ROM.Platforms.GBA.Utils;
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
        ExtractPokemons(assetsPack);
        ExtractEntityGraphics(assetsPack);

        return assetsPack;
    }

    private void ExtractEntityGraphics(RomAssetsPack pack)
    {
        for (int i = 0; i < 152; i++)
        {
            EntityGraphicsDescriptor descriptor = ExtractEntityGraphics(i);
            pack.EntitiesGraphicsInfo.Add(descriptor);
        }
    }

    private EntityGraphicsDescriptor ExtractEntityGraphics(int index)
    {
        GbaPointer offsetPtr = GetPointer(GbaRomOffsetPointers.ENTITY_GRAPHICS_INFO);
        offsetPtr.Add<GbaPointer>(index);

        GbaPointer graphicsInfoPtr = Reader.ReadPointer(offsetPtr.PhysicalAddress);
        int graphicsInfoOffset = graphicsInfoPtr.PhysicalAddress;

        ObjectEventGraphicsInfo info = Reader.Read<ObjectEventGraphicsInfo>(graphicsInfoOffset);

        PaletteDescriptor palette = ExtractEntityPalette(info.PaletteTag);
        SpriteSheetDescriptor spriteSheet = ExtractEntitySpriteSheet(in info, palette);
        // RomAnimation[] animations = ExtractEntityAnimations(info.AnimsPtr);

        return new EntityGraphicsDescriptor();
    }

    private PaletteDescriptor ExtractEntityPalette(short paletteTag)
    {
        GbaPointer palettesPtr = GetPointer(GbaRomOffsetPointers.ENTITY_PALETTES);
        ReadOnlySpan<SpritePalette> palettes = Reader.ReadArrayRef<SpritePalette>(palettesPtr.PhysicalAddress, 19);

        for (int i = 0; i < palettes.Length; i++)
        {
            ref readonly SpritePalette palette = ref palettes[i];

            if (palette.RawDataPtr == GbaPointer.Null)
                break;

            if (palette.Tag == paletteTag)
                return new PaletteDescriptor(palette.RawDataPtr.PhysicalAddress, true, RomPixelFormat.Bpp4);
        }

        throw new InvalidOperationException($"Unable a matching palette for object event graphics with palette tag: 0x{paletteTag:X4}.");
    }

    private SpriteSheetDescriptor ExtractEntitySpriteSheet(in ObjectEventGraphicsInfo info, PaletteDescriptor palette)
    {

    }

    private void ExtractPokemons(RomAssetsPack pack)
    {
        for (int i = 1; i < 151; i++)
        {
            int nameOffset = GetPokemonNameOffset(i);
            int frontGfxOffset = GetPokemonSpriteOffset(GbaRomOffsetPointers.FRONT_POKEMON_SPRITES, i);
            int backGfxOffset = GetPokemonSpriteOffset(GbaRomOffsetPointers.BACK_POKEMON_SPRITES, i);

            int paletteOffset = GetPokemonPaletteOffset(GbaRomOffsetPointers.REGULAR_PALETTES, i);

            var palette = new PaletteDescriptor(paletteOffset, true, RomPixelFormat.Bpp4);

            pack.Palettes.Add(palette);
            pack.PokemonNames.Add(new NameDescriptor(nameOffset, 11, NameKind.Pokemom));
            pack.PokemonFrontSprites.Add(new SpriteDescriptor(frontGfxOffset, 64, 64, true, SpriteKind.FrontPokemon, palette, RomPixelFormat.Bpp4));
            pack.PokemonBackSprites.Add(new SpriteDescriptor(backGfxOffset, 64, 64, true, SpriteKind.BackPokemon, palette, RomPixelFormat.Bpp4));
        }
    }

    private int GetPokemonNameOffset(int i)
    {
        GbaPointer pointer = GetPointer(GbaRomOffsetPointers.POKEMON_NAMES);
        return pointer.PhysicalAddress + i * 11;
    }

    private int GetPokemonSpriteOffset(GbaRomOffsetPointers offset, int i)
    {
        GbaPointer spritesTablePtr = GetPointer(offset);
        return spritesTablePtr.PhysicalAddress + i * Marshal.SizeOf<CompressedSpriteSheet>();
    }

    private int GetPokemonPaletteOffset(GbaRomOffsetPointers offset, int i)
    {
        GbaPointer palettesTablePtr = GetPointer(offset);
        return palettesTablePtr.PhysicalAddress + i * Marshal.SizeOf<CompressedSpriteSheet>();
    }

    public override string Load(NameDescriptor desc)
    {
        ReadOnlySpan<byte> text = Reader.ReadSpan(desc.Address, desc.Length);
        return DecodeGbaText(text);
    }

    public override IRomTexture Load(SpriteDescriptor desc)
    {
        byte[] rawData;
        if (desc.Compressed)
        {
            var spriteSheet = Reader.Read<CompressedSpriteSheet>(desc.Offset);
            rawData = LZ77.DecompressLZ77(RomData, spriteSheet.CompressedDataPtr.PhysicalAddress);
        }
        else
        {
            var spriteSheet = Reader.Read<SpriteSheet>(desc.Offset);
            rawData = Reader.ReadBytes(spriteSheet.RawDataPtr.PhysicalAddress, spriteSheet.Size);
        }

        IRomPalette palette = Load(desc.Palette);
        return new Rom4bppTexture(desc.Width, desc.Height, rawData, palette);
    }

    public override IRomPalette Load(PaletteDescriptor desc)
    {
        byte[] data;
        if (desc.Compressed)
        {
            var palette = Reader.Read<CompressedSpritePalette>(desc.Offset);
            data = LZ77.DecompressLZ77(RomData, palette.DataPtr.PhysicalAddress);
        }
        else
        {
            int size = desc.PixelFormat switch
            {
                RomPixelFormat.Bpp4 => 32,
                RomPixelFormat.Bpp8 => 256,
                RomPixelFormat.Bpp16 => 65536,

                _ => throw new NotImplementedException($"Palette size for pixel format '{desc.PixelFormat}' isn't defined.")
            };

            var palette = Reader.Read<CompressedSpritePalette>(desc.Offset);
            data = Reader.ReadBytes(palette.DataPtr.PhysicalAddress, size);
        }

        return Rom4bppPalette.FromRawData(data, true);
    }

    public EntityGraphicsInfo ExtractEntityGraphicsInfo(int index)
    {
        GbaPointer offsetPtr = GetPointer(GbaRomOffsetPointers.ENTITY_GRAPHICS_INFO);
        offsetPtr.Add<GbaPointer>(index);

        GbaPointer graphicsInfoPtr = Reader.ReadPointer(offsetPtr.PhysicalAddress);
        ObjectEventGraphicsInfo info = Reader.Read<ObjectEventGraphicsInfo>(graphicsInfoPtr.PhysicalAddress);

        IRomPalette palette = GetOrExtractEntityPalette(info.PaletteTag);
        RomSpriteSheet spriteSheet = ExtractEntitySpriteSheet(in info, palette);
        RomAnimation[] animations = ExtractEntityAnimations(info.AnimsPtr);

        return new EntityGraphicsInfo(info.Width, info.Height, info.ShadowSize, palette, spriteSheet, animations);
    }

    private int GetFramesCount(GbaPointer imagesPointer)
    {
        int framesCount = 0;
        int spriteFrameImageSize = Marshal.SizeOf<SpriteFrameImage>();

        for (int i = 0; i < MAX_FRAMES_COUNT; i++)
        {
            SpriteFrameImage sfi = Reader.Read<SpriteFrameImage>(imagesPointer.PhysicalAddress, spriteFrameImageSize * i);

            if (sfi.DataPtr == GbaPointer.Null || sfi.Size == 0)
                break;

            framesCount++;
        }

        return framesCount == 0 ? 1 : framesCount;
    }

    private RomSpriteSheet ExtractEntitySpriteSheet(in ObjectEventGraphicsInfo info, IRomPalette palette)
    {
        int baseOffset = info.ImagesPtr.PhysicalAddress;
        int framesCount = GetFramesCount(info.ImagesPtr);
        int frameDataLength = info.Width * info.Height / 2;

        byte[] rawData = new byte[frameDataLength * framesCount];
        ReadOnlySpan<SpriteFrameImage> frames = Reader.ReadArrayRef<SpriteFrameImage>(baseOffset, framesCount);

        for (int i = 0; i < frames.Length; i++)
        {
            ref readonly SpriteFrameImage sfi = ref frames[i];

            int spriteDataOffset = sfi.DataPtr.PhysicalAddress;
            ReadOnlySpan<byte> frameSegment = Reader.ReadSpan(spriteDataOffset, frameDataLength);

            Span<byte> rawDataSegment = rawData.AsSpan(frameDataLength * i, frameDataLength);
            frameSegment.CopyTo(rawDataSegment);
        }

        var image = new Rom4bppTexture(info.Width, info.Height * framesCount, rawData, palette);
        return new RomSpriteSheet(image, 1, framesCount);
    }

    private RomAnimation[] ExtractEntityAnimations(GbaPointer animsPointer)
    {
        var animations = new List<RomAnimation>();
        var seenPointers = new HashSet<GbaPointer>();

        const int MAX_ANIMATIONS = 50;

        for (int i = 0; i < MAX_ANIMATIONS; i++)
        {
            GbaPointer animBasePointer = Reader.ReadPointer(animsPointer.PhysicalAddress);

            if (!animBasePointer.ReferencesValidLocation(RomData.Length) || seenPointers.Contains(animBasePointer))
                break;

            seenPointers.Add(animBasePointer);

            try
            {
                RomAnimation animation = ExtractEntityAnimation(animBasePointer);
                animations.Add(animation);
            }
            catch (Exception)
            {
                break;
            }

            animsPointer = animsPointer.Inc<GbaPointer>();
        }

        return [.. animations];
    }

    private RomAnimation ExtractEntityAnimation(GbaPointer animPointer)
    {
        var frames = new List<IRomAnimationCmd>();
        int animCmdSize = Marshal.SizeOf<AnimCmd>();
        int crntAnimCmdOffset = animPointer.PhysicalAddress;

        AnimCmd cmd;
        do
        {
            cmd = Reader.Read<AnimCmd>(crntAnimCmdOffset);
            switch (cmd.Type)
            {
                case AnimCmdType.Loop:
                    frames.Add(new RomAnimationCmdLoop(cmd.LoopCount));
                    break;
                case AnimCmdType.Jump:
                    frames.Add(new RomAnimationCmdJump(cmd.JumpTarget));
                    break;
                case AnimCmdType.End:
                    frames.Add(RomAnimationCmdEnd.Default);
                    break;
                default:
                    frames.Add(new RomAnimationCmdFrame(cmd.ImageValue, cmd.Duration, cmd.HFlip, cmd.VFlip));
                    break;
            }

            crntAnimCmdOffset += animCmdSize;

            // Si on a un Jump ou un End, on arrête d'extraire
            if (cmd.Type == AnimCmdType.Jump || cmd.Type == AnimCmdType.End)
                break;

        } while (true);

        return new RomAnimation([.. frames]);
    }

    private IRomPalette GetOrExtractEntityPalette(short paletteTag)
    {
        GbaPointer palettesPtr = GetPointer(GbaRomOffsetPointers.ENTITY_PALETTES);
        ReadOnlySpan<SpritePalette> palettes = Reader.ReadArrayRef<SpritePalette>(palettesPtr.PhysicalAddress, 19);

        for (int i = 0; i < palettes.Length; i++)
        {
            ref readonly SpritePalette palette = ref palettes[i];

            if (palette.RawDataPtr == GbaPointer.Null)
                break;

            if (palette.Tag == paletteTag)
                return GetOrExtractPalette(palette.RawDataPtr.PhysicalAddress);
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

    #region Helper Structs

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    private readonly struct SpritePalette
    {
        [FieldOffset(0x0)]
        public readonly GbaPointer RawDataPtr; // Raw uncompressed data pointer

        [FieldOffset(0x4)]
        public readonly short Tag;
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    private readonly struct CompressedSpritePalette
    {
        [FieldOffset(0x0)]
        public readonly GbaPointer DataPtr; // LZ77 compressed data pointer

        [FieldOffset(0x4)]
        public readonly short Tag;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private readonly struct SpriteSheet
    {
        [FieldOffset(0x0)]
        public readonly GbaPointer RawDataPtr; // Raw uncompressed pixel data

        [FieldOffset(0x4)]
        public readonly short Size;

        [FieldOffset(0x6)]
        public readonly short Tag;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private readonly struct CompressedSpriteSheet
    {
        [FieldOffset(0x0)]
        public readonly GbaPointer CompressedDataPtr;  // LZ77 compressed pixel data

        [FieldOffset(0x4)]
        public readonly short Size;

        [FieldOffset(0x6)]
        public readonly short Tag;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private readonly struct OamData
    {
        public readonly byte Y;

        private readonly byte attr0;
        public int AffineMode => (attr0 >> 0) & 0b11;
        public int ObjMode => (attr0 >> 2) & 0b11;
        public bool Mosaic => ((attr0 >> 4) & 0b1) != 0;
        public bool Is8bpp => ((attr0 >> 5) & 0b1) != 0;
        public int Shape => (attr0 >> 6) & 0b11;

        private readonly ushort attr1;
        public int X => attr1 & 0x1FF;

        public int MatrixNum => (attr1 >> 9) & 0b11111; // Only if affine
        public bool HFlip => ((attr1 >> 12) & 0b1) != 0; // Only if not affine
        public bool VFlip => ((attr1 >> 13) & 0b1) != 0; // Only if not affine
        public int Size => (attr1 >> 14) & 0b11;

        private readonly ushort attr2;
        public int TileIndex => attr2 & 0x3FF;
        public int Priority => (attr2 >> 10) & 0b11;
        public int PaletteNum => (attr2 >> 12) & 0b1111;

        public readonly ushort AffineParam;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 36)]
    private readonly struct ObjectEventGraphicsInfo
    {
        [FieldOffset(0x0)]
        public readonly short TileTag;

        [FieldOffset(0x2)]
        public readonly short PaletteTag;

        [FieldOffset(0x4)]
        public readonly short ReflectionPaletteTag;

        [FieldOffset(0x6)]
        public readonly short Size;

        [FieldOffset(0x8)]
        public readonly short Width;

        [FieldOffset(0xA)]
        public readonly short Height;

        [FieldOffset(0xC)]
        public readonly short RawAttr;

        [FieldOffset(0xD)]
        public readonly byte Tracks;

        [FieldOffset(0x10)]
        public readonly GbaPointer OamPtr;

        [FieldOffset(0x14)]
        private readonly int _dummy;

        [FieldOffset(0x18)]
        public readonly GbaPointer AnimsPtr;

        [FieldOffset(0x1C)]
        public readonly GbaPointer ImagesPtr;

        [FieldOffset(0x20)]
        private readonly int _dummy2;

        public byte PaletteSlot => (byte)(RawAttr & 0xF);
        public byte ShadowSize => (byte)((RawAttr >> 4) & 0b11);
        public bool Inanimate => ((RawAttr >> 6) & 0xb1) == 1;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private readonly struct SpriteFrameImage
    {
        [FieldOffset(0x0)]
        public readonly GbaPointer DataPtr;

        [FieldOffset(0x4)]
        public readonly int Size;
    }

    private enum AnimCmdType : short
    {
        Frame = 0,
        Loop = -3,
        Jump = -2,
        End = -1
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    private readonly struct AnimCmd
    {
        [FieldOffset(0x0)]
        public readonly short TypeRaw;

        [FieldOffset(0x2)]
        public readonly ushort ParamRaw;

        public AnimCmdType Type => (AnimCmdType)TypeRaw;

        public ushort ImageValue => (ushort)(TypeRaw >= 0 ? TypeRaw : 0); // Only valid for frames

        public byte Duration => (byte)(ParamRaw & 0b0011_1111);
        public bool HFlip => (ParamRaw & (1 << 6)) != 0;
        public bool VFlip => (ParamRaw & (1 << 7)) != 0;

        public byte LoopCount => (byte)(ParamRaw & 0b0011_1111); // For type = 1
        public byte JumpTarget => (byte)(ParamRaw & 0b0011_1111); // For type = -2

        public bool IsEnd => Type == AnimCmdType.End;
    }

    #endregion
} 