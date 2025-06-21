using PokeCore.Logging;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.Common.Extensions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeCore.Common;

public sealed class DynamicTypeResolver : IDynamicTypeResolver
{
    private readonly IServiceResolver _services;
    private readonly Logger<DynamicTypeResolver> _logger;
    private readonly Dictionary<string, Type> _resolvedTypesCache;

    public DynamicTypeResolver(IServiceResolver services, Logger<DynamicTypeResolver> logger)
    {
        _logger = logger;
        _services = services;
        _resolvedTypesCache = new Dictionary<string, Type>();
    }

    public Type ResolveType(string assemblyQualifiedTypeName)
    {
        _logger.Trace($"Dynamicly resolving type '{assemblyQualifiedTypeName}'");
        if (_resolvedTypesCache.TryGetValue(assemblyQualifiedTypeName, out Type? cachedType))
        {
            _logger.Trace($"Cache hit");
            _logger.Trace($"Type '{assemblyQualifiedTypeName}' has been resolved dynamicly to '{cachedType.Name}'");
            return cachedType;
        }

        Type? type = Type.GetType(assemblyQualifiedTypeName);
        if (type == null)
            throw new InvalidOperationException($"Couldn't find any type with assembly qualified name '{assemblyQualifiedTypeName}'. Please make sure all your modules are loaded before using {nameof(DynamicTypeResolver)}.");

        _logger.Trace($"Type has been resolved dynamicly to '{type.Name}'");
        _resolvedTypesCache.Add(assemblyQualifiedTypeName, type);

        _logger.Trace($"Type cached with key '{assemblyQualifiedTypeName}'");
        return type;
    }

    public object InstantiateFromTypeName(string assemblyQualifiedTypeName)
    {
        Type resolvedType = ResolveType(assemblyQualifiedTypeName);
        Type? boundInterface = _services.GetUnderlyingServiceType(resolvedType);

        if (boundInterface != null)
            return _services.GetRequiredService(boundInterface);

        return _services.GetRequiredService(resolvedType);
    }

    public T InstantiateFromTypeName<T>(string assemblyQualifiedTypeName)
    {
        return (T)InstantiateFromTypeName(assemblyQualifiedTypeName);
    }
}