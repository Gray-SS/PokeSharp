using System.Reflection;
using Ninject;
using PokeSharp.Core.Attributes;
using PokeSharp.Core.Logging;

namespace PokeSharp.Core.Services;

public sealed class ReflectionManager : IReflectionManager
{
    private readonly IKernel _kernel;
    private readonly ILogger _logger;
    private readonly HashSet<Assembly> _assemblies;

    public ReflectionManager(IKernel kernel, ILogger logger)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _assemblies = new HashSet<Assembly>();
    }

    public void RegisterAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        var assemblyName = assembly.GetName().Name;

        if (!_assemblies.Add(assembly))
        {
            _logger.Warn($"Assembly '{assemblyName}' is already registered - skipping duplicate registration");
            return;
        }

        _logger.Debug($"Registered assembly '{assemblyName}' for reflection scanning");
    }

    public T[] InstantiateClassesOfType<T>() where T : class
    {
        var prioritizedInstances = new SortedDictionary<int, List<T>>(
            Comparer<int>.Create((x, y) => y.CompareTo(x)));

        _logger.Info($"Starting instantiation of classes implementing '{typeof(T).Name}'");

        foreach (var assembly in _assemblies)
        {
            ScanAssemblyForType<T>(assembly, prioritizedInstances);
        }

        var instances = prioritizedInstances.SelectMany(pair => pair.Value).ToArray();
        _logger.Info($"Instantiation completed: {instances.Length} instances of '{typeof(T).Name}' created");

        return instances;
    }

    private void ScanAssemblyForType<T>(Assembly assembly, SortedDictionary<int, List<T>> prioritizedInstances)
        where T : class
    {
        var assemblyName = assembly.GetName().Name;
        _logger.Debug($"Scanning assembly '{assemblyName}' for '{typeof(T).Name}' implementations");
        var initialCount = prioritizedInstances.SelectMany(x => x.Value).Count();

        var concreteTypes = GetConcreteTypesAssignableFrom<T>(assembly);

        foreach (var concreteType in concreteTypes)
        {
            try
            {
                var instance = CreateInstance<T>(concreteType);
                var priority = GetPriority(concreteType);

                AddInstanceToPriorityList(prioritizedInstances, instance, priority);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to instantiate '{concreteType.Name}' from assembly '{assemblyName}': {ex.Message}");
            }
        }

        int finalCount = prioritizedInstances.SelectMany(x => x.Value).Count();
        if (finalCount > initialCount)
        {
            var foundCount = finalCount - initialCount;
            _logger.Debug($"{foundCount} assignable types have been found and resolved through scanning '{assemblyName}'");
        }
        else _logger.Debug($"No assignable types have been through scanning '{assemblyName}'");
    }

    private static IEnumerable<Type> GetConcreteTypesAssignableFrom<T>(Assembly assembly)
    {
        return assembly.GetTypes().Where(type =>
            typeof(T).IsAssignableFrom(type) &&
            !type.IsAbstract &&
            type.IsClass);
    }

    private T CreateInstance<T>(Type concreteType) where T : class
    {
        _logger.Debug($"Found concrete type: '{concreteType.Name}'");

        // Try to find a bindable interface
        var bindableInterface = FindBindableInterface<T>(concreteType);

        T instance;
        if (bindableInterface != null)
        {
            instance = (T)_kernel.Get(bindableInterface);
            _logger.Debug($"Resolved '{concreteType.Name}' via interface '{bindableInterface.Name}'");
        }
        else
        {
            instance = (T)_kernel.Get(concreteType);
            _logger.Debug($"Resolved '{concreteType.Name}' via direct binding (fallback)");
        }

        return instance;
    }

    private Type? FindBindableInterface<T>(Type concreteType)
    {
        return concreteType
            .GetInterfaces()
            .Where(i => i != typeof(T))
            .FirstOrDefault(i => _kernel.GetBindings(i).Length == 1);
    }

    private int GetPriority(Type type)
    {
        var priorityAttr = type.GetCustomAttribute<PriorityAttribute>();
        var priority = priorityAttr?.Priority ?? 0;

        if (priorityAttr != null)
        {
            _logger.Debug($"Priority attribute found with value: {priority}.");
        }

        return priority;
    }

    private static void AddInstanceToPriorityList<T>(
        SortedDictionary<int, List<T>> prioritizedInstances,
        T instance,
        int priority)
    {
        if (!prioritizedInstances.TryGetValue(priority, out var list))
        {
            list = new List<T>();
            prioritizedInstances[priority] = list;
        }

        list.Add(instance);
    }
}