using System.Reflection;
using PokeSharp.Core.Attributes;

namespace PokeSharp.Core.Services;

public sealed class ReflectionManager
{
    private readonly Assembly[] _assemblies;

    public ReflectionManager()
    {
        _assemblies = AppDomain.CurrentDomain.GetAssemblies();
    }

    public T[] InstantiateClassesOfType<T>() where T : class
    {
        var prioritizedInstances = new SortedDictionary<int, List<T>>(Comparer<int>.Create((x,y) => y.CompareTo(x)));

        foreach (Assembly assembly in _assemblies)
        {
            foreach (Type type in assembly.GetTypes().Where(x =>
                typeof(T).IsAssignableFrom(x) &&
                !x.IsAbstract &&
                x.IsClass))
            {
                var service = (T)S.GetService(type);
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