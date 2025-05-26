using Ninject;
using PokeSharp.Core.Modules;

namespace PokeSharp.Entities;

public sealed class EntitiesModule : Module
{
    public override string Name => "PokéSharp - Entities";

    public override void Configure(IKernel kernel)
    {
    }
}