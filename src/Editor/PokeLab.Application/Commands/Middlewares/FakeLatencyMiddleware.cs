namespace PokeLab.Application.Commands.Middlewares;

public sealed class FakeLatencyMiddleware : ICommandMiddleware
{
    private readonly int _latency;

    public FakeLatencyMiddleware(int latencyInMilliseconds)
    {
        _latency = latencyInMilliseconds;
    }

    public async Task InvokeAsync(ICommand command, Func<Task> next)
    {
        await Task.Delay(_latency);
        await next();
    }
}