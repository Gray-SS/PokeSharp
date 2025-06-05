using Ninject;
using Ninject.Infrastructure;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Logging.Outputs;
using PokeSharp.Core.Modules;
using PokeSharp.Core.Services;

namespace PokeSharp.Core;

public interface IApp : IHaveKernel
{
    string AppName { get; }
    Version AppVersion{ get; }

    IModuleLoader ModuleLoader { get; }

    void Run();
}

public abstract class App<TEngine> : IApp where TEngine : Engine
{
    public abstract string AppName { get; }
    public virtual Version AppVersion => new Version(1, 0, 0);

    public IModuleLoader ModuleLoader { get; private set; } = null!;

    public IKernel Kernel => _kernel;

    private IKernel _kernel = null!;
    private Logger _logger = null!;

    public void Run()
    {
        try
        {
            _kernel = ConfigureContainer();
            ConfigureLogging();

            _logger.Info("Starting application...");

            ServiceLocator.Initialize(this);
            ConfigureModules();
            LogLoggerSettings();

            RunEngine();
        }
        catch (Exception ex)
        {
            _logger?.Fatal($"Critical application failure: {ex.Message}");
            _logger?.Debug($"{ex.StackTrace ?? "No stack trace available."}");
            throw;
        }
        finally
        {
            Cleanup();
        }
    }

    private IKernel ConfigureContainer()
    {
        IKernel kernel = new StandardKernel();

        // App specific services
        kernel.Bind<IApp>().ToConstant(this);
        kernel.Bind<TEngine>().ToSelf().InSingletonScope();
        kernel.Bind<IReflectionManager>().To<ReflectionManager>().InSingletonScope();
        kernel.Bind<IModuleLoader>().To<ModuleLoader>().InSingletonScope();
        kernel.Bind<LoggerSettings>().ToSelf().InSingletonScope();
        kernel.Bind<Logger>().ToProvider<LoggerProvider>();

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
        ModuleLoader.RegisterModule(new CoreModule());
        RegisterModules(ModuleLoader);

        ModuleLoader.ConfigureModules();
        if (!ModuleLoader.IsConfigured)
        {
            throw new AppException("The module loader have not been configured correctly.");
        }
    }

    private void LogLoggerSettings()
    {
        LoggerSettings settings = _kernel.Get<LoggerSettings>();

        _logger.Info("Logger settings:");
        _logger.Info($"\tMinimum log level: {settings.LogLevel}");
        _logger.Info("\tOutputs:");
        foreach (ILogSink output in settings.Outputs)
        {
            _logger.Info($"\t- {output.GetType().Name}:{output.Name}");
        }
    }

    private void RunEngine()
    {
        using TEngine engine = _kernel.Get<TEngine>();
        _kernel.Bind<Engine>().ToConstant(engine);

        engine.Run();
    }

    protected abstract void RegisterModules(IModuleLoader loader);

    private void Cleanup()
    {
        _logger?.Info("Shutting down application...");
        _kernel?.Dispose();
        ServiceLocator.Cleanup();
    }
}