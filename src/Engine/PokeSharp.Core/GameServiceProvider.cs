using Ninject;

namespace PokeSharp.Core;

public static class S
{
    public static object GetService(Type type)
    {
        EnsureSingleBinding(type);
        return Engine.Instance.Kernel.Get(type);
    }

    public static T GetService<T>() where T : class
    {
        EnsureSingleBinding(typeof(T));
        return Engine.Instance.Kernel.Get<T>();
    }

    public static bool TryGetService(Type type, out object? service)
    {
        if (HasSingleBinding(type))
        {
            service = Engine.Instance.Kernel.Get(type);
            return true;
        }

        service = null;
        return false;
    }

    public static bool TryGetService<T>(out T? service) where T : class
    {
        if (HasSingleBinding(typeof(T)))
        {
            service = Engine.Instance.Kernel.Get<T>();
            return true;
        }

        service = null;
        return false;
    }

    private static void EnsureSingleBinding(Type type)
    {
        var bindings = Engine.Instance.Kernel.GetBindings(type);
        if (bindings.Length == 0)
        {
            throw new InvalidOperationException($"No service registered for '{type.Name}'. Make sure it is bound in the kernel.");
        }

        if (bindings.Length > 1)
        {
            throw new InvalidOperationException($"Multiple bindings found for '{type.Name}'. Only one binding is allowed.");
        }
    }

    private static bool HasSingleBinding(Type type) =>
        Engine.Instance.Kernel.GetBindings(type).Length == 1;
}
