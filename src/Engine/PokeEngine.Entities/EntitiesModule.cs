using Ninject;
using PokeCore.Hosting.Modules;

namespace PokeEngine.Entities;

public sealed class EntitiesModule : Module
{
    public override string Name => "PokéSharp - Entities";

    public override void Configure(IKernel kernel)
    {
    }
}