using Ninject.Modules;
using PokeSharp.ROM.Services;

namespace PokeSharp.ROM;

public sealed class RomModule : NinjectModule
{
    private static readonly string GBA_CONFIG_PATH = Path.Combine(AppContext.BaseDirectory, "configs", "gbas.yaml");

    public override void Load()
    {
        Bind<IGbaConfigProvider>().To<GbaConfigProvider>()
            .WithConstructorArgument("yamlPath", GBA_CONFIG_PATH);
    }
}
