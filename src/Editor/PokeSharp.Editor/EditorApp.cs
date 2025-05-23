using PokeSharp.Assets;
using PokeSharp.Core;
using PokeSharp.Core.Modules;
using PokeSharp.Inputs;
using PokeSharp.Rendering;
using PokeSharp.ROM;

namespace PokeSharp.Editor;

public sealed class EditorApp : App<EditorEngine>
{
    public override string AppName => "PokéSharp Editor";
    public override Version AppVersion => new Version(0, 0, 0, 84);

    protected override void RegisterModules(IModuleLoader loader)
    {
        loader.RegisterModule(new RomModule());
        loader.RegisterModule(new AssetsModule());
        loader.RegisterModule(new InputsModule());
        loader.RegisterModule(new RenderingModule());
        loader.RegisterModule(new EditorModule());
    }
}