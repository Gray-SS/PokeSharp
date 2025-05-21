using Ninject;
using PokeSharp.Core.Services;

namespace PokeSharp.Assets.Extensions;

public static class KernelExtensions
{
    public static void LoadAssetsModule(this IKernel kernel)
    {
        kernel.Load<AssetsModule>();

        IReflectionManager reflectionManager = kernel.Get<IReflectionManager>();
        reflectionManager.RegisterAssembly(typeof(KernelExtensions).Assembly);
    }
}