namespace PokeCore.DependencyInjection.Abstractions;

public interface IServiceCollections
{
    IServiceCollections Add(ServiceDescriptor descriptor);

    IServiceResolver Build();
}