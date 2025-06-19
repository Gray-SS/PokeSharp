using PokeCore.IO;

namespace PokeTools.Assets.CLI;

public sealed class TestImporter : AssetImporter<string>
{
    public override Type ProcessorType => typeof(TestImporter);
    public override string SupportedExtensions => ".test";

    public override Result<string, string> Import(IVirtualFile file)
    {
        return Result.Success("The import succeeded!");
    }
}
