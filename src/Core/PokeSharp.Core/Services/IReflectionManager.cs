using System.Reflection;

namespace PokeSharp.Core.Services;

public interface IReflectionManager
{
    void RegisterAssembly(Assembly assembly);

    T[] InstantiateClassesOfType<T>() where T : class;
}