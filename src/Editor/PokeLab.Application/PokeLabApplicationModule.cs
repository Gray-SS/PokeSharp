using Ninject;
using PokeCore.Hosting.Modules;
using PokeLab.Application.Commands;

namespace PokeLab.Application;

public sealed class PokeLabApplicationModule : Module
{
    public override string Name => "PokéLab Application Module";

    public override void ConfigureServices(IKernel kernel)
    {
        kernel.Bind<ICommandDispatcher>().To<CommandDispatcher>();
    }
}