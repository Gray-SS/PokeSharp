using Ninject;

namespace PokeSharp.Core.Services;

public sealed class DynamicTypeResolver : IDynamicTypeResolver
{
    private readonly IKernel _kernel;
    private readonly Dictionary<string, Type> _resolvedTypes;

    public DynamicTypeResolver(IKernel kernel)
    {
        _kernel = kernel;
        _resolvedTypes = new Dictionary<string, Type>();
    }

    public Type ResolveType(string assemblyQualifiedTypeName)
    {
        if (_resolvedTypes.TryGetValue(assemblyQualifiedTypeName, out Type? cachedType))
            return cachedType;

        Type? type = Type.GetType(assemblyQualifiedTypeName);
        if (type == null)
            throw new InvalidOperationException($"Couldn't find any type with assembly qualified name '{assemblyQualifiedTypeName}'. Please make sure all your modules are loaded before using this interface.");

        _resolvedTypes.Add(assemblyQualifiedTypeName, type);
        return type;
    }

    public object InstantiateFromTypeName(string assemblyQualifiedTypeName)
    {
        Type resolvedType = ResolveType(assemblyQualifiedTypeName);
        Type? boundInterface = FindBindableInterface(resolvedType);

        if (boundInterface != null)
            return _kernel.Get(boundInterface);

        return _kernel.Get(resolvedType);
    }

    public T InstantiateFromTypeName<T>(string assemblyQualifiedTypeName)
    {
        return (T)InstantiateFromTypeName(assemblyQualifiedTypeName);
    }

    private Type? FindBindableInterface(Type concreteType)
    {
        return concreteType
            .GetInterfaces()
            .FirstOrDefault(i => _kernel.GetBindings(i).Length == 1);
    }
}