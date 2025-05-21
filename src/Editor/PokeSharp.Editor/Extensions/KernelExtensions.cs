using Ninject;
using PokeSharp.Core.Services;

namespace PokeSharp.Editor.Extensions;

public static class KernelExtensions
{
    public static void LoadEditorModule(this IKernel kernel)
    {
        IReflectionManager reflectionManager = kernel.Get<IReflectionManager>();
        reflectionManager.RegisterAssembly(typeof(KernelExtensions).Assembly);

        kernel.Load(new EditorModule());
    }
}