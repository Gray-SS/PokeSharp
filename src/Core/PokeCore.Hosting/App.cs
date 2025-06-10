using Ninject;
using Ninject.Infrastructure;
using PokeCore.Hosting.Exceptions;
using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Logging.Outputs;
using PokeCore.Hosting.Modules;
using PokeCore.Hosting.Services;

namespace PokeCore.Hosting;

public interface IApp : IDisposable, IHaveKernel
{
    string AppName { get; }
    Version AppVersion { get; }

    IModuleLoader ModuleLoader { get; }

    void Run();
}

public abstract class App : IApp
{
    public abstract string AppName { get; }
    public virtual Version AppVersion => new(1, 0, 0);

    public IModuleLoader ModuleLoader { get; private set; } = null!;

    public IKernel Kernel => _kernel;

    private bool _isDisposed;
    private IKernel _kernel = null!;
    private Logger _logger = null!;

    public void Run()
    {
        try
        {
            ValidateSingleInstance();

            _instance = this;
            _kernel = ConfigureContainer();
            ConfigureLogging();

            _logger.Info("Starting application...");

            ServiceLocator.Initialize(this);
            ConfigureModules();

            OnRun();
        }
        catch (Exception ex)
        {
            _logger?.Fatal($"Critical application failure: {ex.Message}");
            _logger?.Debug($"{ex.StackTrace ?? "No stack trace available."}");
            throw;
        }
        finally
        {
            Dispose();
        }
    }

    private IKernel ConfigureContainer()
    {
        IKernel kernel = new StandardKernel();

        // App required services
        kernel.Bind<IApp>().ToConstant(this);
        kernel.Bind<IReflectionManager>().To<ReflectionManager>().InSingletonScope();
        kernel.Bind<IDynamicTypeResolver>().To<DynamicTypeResolver>().InSingletonScope();
        kernel.Bind<IModuleLoader>().To<ModuleLoader>().InSingletonScope();
        kernel.Bind<LoggerSettings>().ToSelf().InSingletonScope();
        kernel.Bind<Logger>().ToProvider<LoggerProvider>();

        // Configure app-specific services
        this.ConfigureServices(kernel);

        return kernel;
    }

    private void ConfigureLogging()
    {
        LoggerSettings loggerSettings = _kernel.Get<LoggerSettings>();
        ConfigureLogging(loggerSettings);

        _logger = new ContextLogger(loggerSettings, "App");
    }

    protected virtual void ConfigureLogging(LoggerSettings settings)
    {
        settings.AddOutput(new FileLogSink(targetDirectory: "logs"));
    }

    private void ConfigureModules()
    {
        ModuleLoader = Kernel.Get<IModuleLoader>();
        ConfigureModules(ModuleLoader);

        ModuleLoader.ConfigureModules();
        if (!ModuleLoader.IsConfigured)
        {
            throw new AppException("The module loader have not been configured correctly.");
        }
    }

    protected abstract void OnRun();
    protected abstract void ConfigureServices(IKernel kernel);
    protected abstract void ConfigureModules(IModuleLoader loader);

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _logger?.Info("Shutting down application...");

            Dispose(disposing: false);

            _kernel?.Dispose();
            ServiceLocator.Cleanup();
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    private static void ValidateSingleInstance()
    {
        if (_instance != null)
        {
            throw new AppException($"Only one instance of '{nameof(App)}' is allowed to be started. " +
                                    $"You tried to create and start a second instance of '{nameof(App)}'. " +
                                    $"Ensure your application only instantiates a single '{nameof(App)}'.");
        }
    }

    public static App Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new AppException($"""
                    The App has not been instantied yet.
                    Make sure to instantiate your App before accessing '{nameof(App.Instance)}'.
                """);
            }

            return _instance;
        }
    }

    private static App _instance = null!;
}