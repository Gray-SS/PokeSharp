using PokeCore.DependencyInjection.Abstractions;

namespace PokeCore.Hosting.Abstractions;

public interface IAppBuilder
{
    IAppBuilder UseDIProviderFactory(Func<IServiceCollections> factory);

    IServiceCollections Build();
}