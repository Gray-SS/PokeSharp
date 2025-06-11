using System.Diagnostics.CodeAnalysis;

namespace PokeCore.DependencyInjection.Abstractions;

public interface IServiceContainer
{
    bool HasService(Type service);
    bool HasService<T>() where T : class;

    object GetService(Type service);
    T GetService<T>() where T : class;
    IEnumerable<T> GetServices<T>() where T : class;

    bool TryGetService(Type service, [NotNullWhen(true)] out object? implementation);
    bool TryGetService<T>([NotNullWhen(true)] out T? implementation) where T : class;
    bool TryGetServices<T>([NotNullWhen(true)] out IEnumerable<T>? implementation) where T : class;
}