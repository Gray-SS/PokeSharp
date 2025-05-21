using Ninject;
using PokeSharp.Core.Services;

namespace PokeSharp.Inputs.Extensions;

public static class KernelExtensions
{
    public static void LoadInputsModule(this IKernel kernel)
    {
        kernel.Load<InputModule>();

        IReflectionManager reflectionManager = kernel.Get<IReflectionManager>();
        reflectionManager.RegisterAssembly(typeof(KernelExtensions).Assembly);
    }
}