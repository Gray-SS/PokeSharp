using System.Reflection;

namespace PokeCore.Common;

public interface IReflectionManager
{
    void RegisterAssembly(Assembly assembly);

    T[] InstantiateClassesOfType<T>() where T : class;
}