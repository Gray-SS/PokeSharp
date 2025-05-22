using Ninject;
using PokeSharp.Core.Services;

namespace PokeSharp.ROM.Extensions;

public static class KernelExtensions
{
    public static void LoadRomModule(this IKernel kernel)
    {
        kernel.Load(new RomModule());

        IReflectionManager reflectionManager = kernel.Get<IReflectionManager>();
        reflectionManager.RegisterAssembly(typeof(KernelExtensions).Assembly);
    }
}