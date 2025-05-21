using Ninject;
using PokeSharp.Core.Services;

namespace PokeSharp.Core.Extensions;

public static class KernelExtensions
{
    public static void LoadCoreModule(this IKernel kernel)
    {
        kernel.Load<CoreModule>();

        IReflectionManager reflectionManager = kernel.Get<IReflectionManager>();
        reflectionManager.RegisterAssembly(typeof(KernelExtensions).Assembly);
    }

    public static void LoadCoreModule<TEngine>(this IKernel kernel) where TEngine : Engine
    {
        kernel.Load<CoreModule<TEngine>>();

        IReflectionManager reflectionManager = kernel.Get<IReflectionManager>();
        reflectionManager.RegisterAssembly(typeof(KernelExtensions).Assembly);
        reflectionManager.RegisterAssembly(typeof(TEngine).Assembly);
    }
}