using Ninject;
using PokeSharp.Core.Logging;

namespace PokeSharp.Core.Services;

public sealed class DynamicTypeResolver : IDynamicTypeResolver
{
    private readonly IKernel _kernel;
    private readonly Logger _logger;
    private readonly Dictionary<string, Type> _resolvedTypes;

    public DynamicTypeResolver(IKernel kernel, Logger logger)
    {
        _kernel = kernel;
        _logger = logger;
        _resolvedTypes = new Dictionary<string, Type>();
    }

    public Type ResolveType(string assemblyQualifiedTypeName)
    {
        ThrowHelper.AssertNotNullOrWhitespace(assemblyQualifiedTypeName);

        _logger.Trace("Resolving dynamic type");
        if (_resolvedTypes.TryGetValue(assemblyQualifiedTypeName, out Type? cachedType))
        {
            _logger.Trace($"Type already resolved. Returning cached type ({cachedType.Name}).");
            return cachedType;
        }

        Type? type = Type.GetType(assemblyQualifiedTypeName);
        if (type == null)
            throw new InvalidOperationException($"Couldn't find any type with assembly qualified name '{assemblyQualifiedTypeName}'. Please make sure all your modules are loaded before using {nameof(DynamicTypeResolver)}.");

        _resolvedTypes.Add(assemblyQualifiedTypeName, type);
        _logger.Trace($"Type cached with key '{assemblyQualifiedTypeName}' -> {type.Name}");
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