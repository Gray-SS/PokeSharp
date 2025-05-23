using PokeSharp.Core;
using PokeSharp.Core.Modules;
using PokeSharp.Assets;
using PokeSharp.Inputs;
using PokeSharp.Rendering;
using PokeSharp.ROM;

namespace PokeSharp.DesktopGL;

public sealed class PokesharpApp : App<PokesharpGame>
{
    public override string AppName => "PokéSharp Runtime";

    protected override void RegisterModules(IModuleLoader loader)
    {
        loader.RegisterModule(new RomModule());
        loader.RegisterModule(new AssetsModule());
        loader.RegisterModule(new InputsModule());
        loader.RegisterModule(new RenderingModule());
    }
}