using Ninject;
using PokeSharp.Core.Modules;
using PokeSharp.ROM.Services;

namespace PokeSharp.ROM;

public sealed class RomModule : Module
{
    private static readonly string GBA_CONFIG_PATH = Path.Combine(AppContext.BaseDirectory, "configs", "gbas.yaml");

    public override string ModuleName => "Rom";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<IGbaConfigProvider>()
            .To<GbaConfigProvider>()
            .WithConstructorArgument("yamlPath", GBA_CONFIG_PATH);
    }
}
