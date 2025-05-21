namespace PokeSharp.Core.Services;

public interface IReflectionManager
{
    T[] InstantiateClassesOfType<T>() where T : class;
}