using PokeCore.Logging;
using PokeCore.DependencyInjection.Abstractions;
using PokeCore.Hosting.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeCore.Hosting;

public abstract class App : IApp, IDisposable
{
    public abstract string AppName { get; }
    public virtual Version AppVersion => new(1, 0, 0);

    public bool IsDisposed { get; private set; }
    public bool IsRunning => _isRunning;
    public IServiceResolver Services => _services;

    private Logger _logger = null!;
    private static readonly Lock _lock = new();

    private bool _isRunning;
    private IServiceResolver _services = null!;
    private IEnumerable<IHostedService> _hostedServices = null!;

    private readonly CancellationTokenSource _cts;

    public App()
    {
        _cts = new CancellationTokenSource();
    }

    public async Task<bool> StartAsync()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        using (_lock.EnterScope())
        {
            if (_isRunning)
            {
                _logger?.Error("Started the application twice.");
                return false;
            }

            _isRunning = true;
        }

        try
        {
            var builder = new AppBuilder();
            ConfigureApplication(builder);

            IServiceCollections serviceCollections = builder.Build();
            serviceCollections.AddSingleton<IApp>(this);
            serviceCollections.AddSingleton<IServiceResolver>(sc => sc);
            ConfigureServices(serviceCollections);

            IServiceResolver services = serviceCollections.Build();
            ServiceLocator.Initialize(services);

            _logger = services.GetRequiredService<Logger<App>>();
            _logger.Info($"Starting {AppName} v{AppVersion}...");
            _logger.Info($"Configuring application...");
            Configure(services);

            _hostedServices = services.GetServices<IHostedService>();

            _logger.Info($"Starting hosted services...");
            Task[] tasks = _hostedServices.Select(x => x.StartAsync(_cts.Token)).ToArray();
            await Task.WhenAll(tasks);

            _logger.Info($"Application started successfully.");
            _services = services;
            return true;
        }
        catch (Exception ex)
        {
            if (_logger != null)
                _logger.Fatal("Unexpected error occured while starting application.", ex);
            else
                Console.Error.WriteLine($"Fatal: {ex}");

            _cts.Cancel();
            await StopAsync();
            return false;
        }
    }

    public async Task StopAsync()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        using (_lock.EnterScope())
        {
            if (!_isRunning)
                return;

            _isRunning = false;
        }

        _logger.Info("Stopping application...");
        _cts.Cancel();

        if (_hostedServices != null)
        {
            foreach (var service in _hostedServices.Reverse())
            {
                try
                {
                    await service.StopAsync(_cts.Token);
                }
                catch (Exception ex)
                {
                    _logger?.Error($"Error stopping service {service.GetType().Name}.", ex);
                }
            }
        }

        _logger?.Info("Application stopped.");
    }

    public async Task WaitForShutdownAsync()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        using (_lock.EnterScope())
        {
            if (!_isRunning)
                return;
        }

        try
        {
            Task[] waitables = _hostedServices
                .OfType<IWaitableHostedService>()
                .Select(x => x.WaitForShutdownAsync())
                .ToArray();

            await Task.WhenAll(waitables);
        }
        catch (Exception ex)
        {
            _logger?.Fatal("Application terminated unexpectedly.", ex);
            _cts.Cancel();
        }
        finally
        {
            await StopAsync();
        }
    }

    protected abstract void Configure(IServiceResolver services);
    protected abstract void ConfigureServices(IServiceCollections services);

    protected virtual void ConfigureApplication(IAppBuilder builder)
    {
    }

    public void Dispose()
    {
        // The lock may not be needed here
        using (_lock.EnterScope())
        {
            if (IsDisposed)
                return;
        }

        IsDisposed = true;
        Dispose(disposing: true);
        ServiceLocator.Cleanup();

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cts.Cancel();
            _cts.Dispose();

            _logger?.Info("Cleaning services...");
            (_services as IDisposable)?.Dispose();
            _services = null!;

#if DEBUG
            // This is used to finalize disposable but non-disposed object.
            // It's very convenient when working with unmanaged resources.
            _logger?.Info("Waiting for finalizers...");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            _logger?.Info("Finalizers completed.");
#endif

            _logger?.Info("Application shutdown");
        }
    }
}