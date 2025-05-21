using Ninject;
using PokeSharp.Core.Services;

namespace PokeSharp.Rendering.Extensions;

public static class KernelExtensions
{
    public static void LoadRenderingModule(this IKernel kernel)
    {
        IReflectionManager reflectionManager = kernel.Get<IReflectionManager>();
        reflectionManager.RegisterAssembly(typeof(KernelExtensions).Assembly);
    }
}