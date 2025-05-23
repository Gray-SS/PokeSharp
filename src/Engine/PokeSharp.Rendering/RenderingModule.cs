using Ninject;
using PokeSharp.Core.Modules;

namespace PokeSharp.Rendering;

public sealed class RenderingModule : Module
{
    public override string ModuleName => "Rendering";

    public override void Configure(IKernel kernel)
    {
    }
}