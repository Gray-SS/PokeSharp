using Ninject;
using PokeCore.Hosting.Modules;
using PokeEngine.ROM.Services;

namespace PokeEngine.ROM;

public sealed class RomModule : Module
{
    private static readonly string GBA_CONFIG_PATH = Path.Combine(AppContext.BaseDirectory, "configs", "gbas.yaml");

    public override string Name => "Rom";

    public override void ConfigureServices(IKernel kernel)
    {
        kernel.Bind<IGbaConfigProvider>()
            .To<GbaConfigProvider>()
            .WithConstructorArgument("yamlPath", GBA_CONFIG_PATH);
    }
}
