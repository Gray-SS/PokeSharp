namespace PokeCore.Hosting.Abstractions;

public interface IWaitableHostedService : IHostedService
{
    Task WaitForShutdownAsync();
}