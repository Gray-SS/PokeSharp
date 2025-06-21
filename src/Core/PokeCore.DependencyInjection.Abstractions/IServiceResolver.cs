namespace PokeCore.DependencyInjection.Abstractions;

public interface IServiceResolver : IServiceProvider
{
    IEnumerable<object> GetServices(Type serviceType);
}