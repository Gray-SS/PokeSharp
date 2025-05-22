using System.Reflection;
using Ninject;
using PokeSharp.Core.Attributes;
using PokeSharp.Core.Exceptions;

namespace PokeSharp.Core.Services;

public sealed class ReflectionManager : IReflectionManager
{
    private readonly IKernel _kernel;
    private readonly List<Assembly> _assemblies;

    public ReflectionManager(IKernel kernel)
    {
        _kernel = kernel;
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
            foreach (Type concreteType in assembly.GetTypes().Where(x =>
                         typeof(T).IsAssignableFrom(x) &&
                         !x.IsAbstract &&
                         x.IsClass))
            {
                T instance;

                if (concreteType.IsAssignableTo(typeof(Engine)))
                {
                    instance = (T)(object)Engine.Instance;
                }
                else
                {
                    // Try to find a parent interfaces that is a registered service
                    var bindableInterface = concreteType
                        .GetInterfaces()
                        .Where(i => i != typeof(T))
                        .FirstOrDefault(i => _kernel.GetBindings(i).Length == 1);

                    if (bindableInterface != null)
                    {
                        // A bindable interface has been found, resolve the interface
                        instance = (T)_kernel.Get(bindableInterface);
                    }
                    else
                    {
                        // Fallback, auto-binding will be done in that case
                        instance = (T)_kernel.Get(concreteType);
                    }
                }

                int priority = concreteType.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0;

                if (!prioritizedInstances.TryGetValue(priority, out List<T>? list))
                    prioritizedInstances[priority] = list = new List<T>();

                list.Add(instance);
            }
        }

        return [.. prioritizedInstances.SelectMany(pair => pair.Value)];
    }

}