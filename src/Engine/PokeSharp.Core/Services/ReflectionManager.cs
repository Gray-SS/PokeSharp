using System.Reflection;
using Ninject;
using PokeSharp.Core.Attributes;
using PokeSharp.Core.Logging;

namespace PokeSharp.Core.Services;

public sealed class ReflectionManager : IReflectionManager
{
    private readonly IKernel _kernel;
    private readonly ILogger _logger;
    private readonly List<Assembly> _assemblies;

    public ReflectionManager(IKernel kernel, ILogger logger)
    {
        _kernel = kernel;
        _logger = logger;
        _assemblies = new List<Assembly>();
    }

    public void RegisterAssembly(Assembly assembly)
    {
        if (_assemblies.Contains(assembly))
        {
            _logger.Warn($"Assembly '{assembly.FullName}' already registered in the reflection manager."
                        + "Make sure to register this assembly only once.");
        }

        _assemblies.Add(assembly);
    }

    public T[] InstantiateClassesOfType<T>() where T : class
    {
        var prioritizedInstances = new SortedDictionary<int, List<T>>(Comparer<int>.Create((x, y) => y.CompareTo(x)));

        _logger.Debug($"Instantiating classes of type '{typeof(T).Name}'...");

        foreach (Assembly assembly in _assemblies)
        {
            foreach (Type concreteType in assembly.GetTypes().Where(x =>
                         typeof(T).IsAssignableFrom(x) &&
                         !x.IsAbstract &&
                         x.IsClass))
            {
                T instance;

                _logger.Debug($"Concrete type found: '{concreteType.Name}'.");
                _logger.Debug("Trying to find a bindable interface...");

                // Try to find a parent interfaces that is a registered service
                var bindableInterface = concreteType
                    .GetInterfaces()
                    .Where(i => i != typeof(T))
                    .FirstOrDefault(i => _kernel.GetBindings(i).Length == 1);

                if (bindableInterface != null)
                {
                    // A bindable interface has been found, resolve the interface
                    _logger.Debug($"Bindable interface found: '{bindableInterface.Name}'.");
                    instance = (T)_kernel.Get(bindableInterface);
                    _logger.Debug($"Bindable interface resolved as: '{instance.GetType().Name}'.");
                }
                else
                {
                    // Fallback, auto-binding will be done in that case
                    _logger.Debug($"Fallback, no bindable interface found for concrete type. Auto-binding...");
                    instance = (T)_kernel.Get(concreteType);
                    _logger.Debug($"Auto-binded type resolved as: '{instance.GetType().Name}'.");
                }

                PriorityAttribute? priorityAttr = concreteType.GetCustomAttribute<PriorityAttribute>();
                if (priorityAttr != null)
                {
                    _logger.Debug($"Priority attribute found with value: {priorityAttr.Priority}.");
                }

                int priority = priorityAttr?.Priority ?? 0;

                if (!prioritizedInstances.TryGetValue(priority, out List<T>? list))
                    prioritizedInstances[priority] = list = new List<T>();

                list.Add(instance);
            }
        }

        T[] instances = [.. prioritizedInstances.SelectMany(pair => pair.Value)];
        _logger.Debug($"Successfully instantied {instances.Length} classes of type '{typeof(T).Name}'...");

        return instances;
    }

}