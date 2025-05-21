using System.Reflection;
using PokeSharp.Core.Attributes;
using PokeSharp.Core.Exceptions;

namespace PokeSharp.Core.Services;

public sealed class ReflectionManager : IReflectionManager
{
    private readonly List<Assembly> _assemblies;

    public ReflectionManager()
    {
        _assemblies = new List<Assembly>();
    }

    public void RegisterAssembly(Assembly assembly)
    {
        if (_assemblies.Contains(assembly))
        {
            throw new EngineException($"""
                Assembly '{assembly.FullName}' already registered in the reflection manager.
                Make sure to register this assembly only once.
            """);
        }

        _assemblies.Add(assembly);
    }

    public T[] InstantiateClassesOfType<T>() where T : class
    {
        var prioritizedInstances = new SortedDictionary<int, List<T>>(Comparer<int>.Create((x, y) => y.CompareTo(x)));

        foreach (Assembly assembly in _assemblies)
        {
            foreach (Type type in assembly.GetTypes().Where(x =>
                typeof(T).IsAssignableFrom(x) &&
                !x.IsAbstract &&
                x.IsClass))
            {
                T service = type.IsAssignableTo(typeof(Engine)) ?
                    (T)(object)Engine.Instance :
                    (T)S.GetService(type);

                int priority = type.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0;

                if (!prioritizedInstances.TryGetValue(priority, out List<T>? value))
                {
                    value = new List<T>();
                    prioritizedInstances[priority] = value;
                }

                value.Add(service);
            }
        }

        return [.. prioritizedInstances.SelectMany(pair => pair.Value)];
    }
}