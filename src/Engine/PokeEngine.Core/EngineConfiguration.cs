using PokeCore.Logging;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.Hosting.Abstractions;

namespace PokeEngine.Core;

public sealed class EngineConfiguration
{
    public IApp App { get; }
    public IServiceContainer Services { get; }
    public Logger<BaseEngine> Logger { get; }

    public EngineConfiguration(IApp app, IServiceContainer services, Logger<BaseEngine> logger)
    {
        App = app;
        Services = services;
        Logger = logger;
    }
}