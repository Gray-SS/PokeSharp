namespace PokeCore.Common;

public interface IDynamicTypeResolver
{
    Type ResolveType(string assemblyQualifiedTypeName);

    object InstantiateFromTypeName(string assemblyQualifiedTypeName);

    T InstantiateFromTypeName<T>(string assemblyQualifiedTypeName);
}