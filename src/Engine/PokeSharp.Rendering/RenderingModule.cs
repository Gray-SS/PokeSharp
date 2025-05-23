using Ninject;
using PokeSharp.Core.Modules;

namespace PokeSharp.Rendering;

public sealed class RenderingModule : Module
{
    public override string Name => "Rendering";

    public override void Configure(IKernel kernel)
    {
    }
}