using System.Reflection;

namespace PokeCore.Hosting.Services;

public interface IReflectionManager
{
    void RegisterAssembly(Assembly assembly);

    T[] InstantiateClassesOfType<T>() where T : class;
}