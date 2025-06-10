using Ninject;
using PokeCore.Hosting.Modules;

namespace PokeEngine.Rendering;

public sealed class RenderingModule : Module
{
    public override string Name => "Rendering";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<IRenderingPipeline>().To<RenderingPipeline>().InTransientScope();
    }
}