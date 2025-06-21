using PokeCore.DependencyInjection.Abstractions;

namespace PokeCore.Hosting.Abstractions;

public interface IApp
{
    string AppName { get; }
    Version AppVersion { get; }

    bool IsRunning { get; }
    IServiceResolver Services { get; }

    Task<bool> StartAsync();
    Task StopAsync();

    Task WaitForShutdownAsync();
}