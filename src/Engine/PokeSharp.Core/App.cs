using Ninject;
using Ninject.Infrastructure;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Logging;
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
    private ILogger _logger = null!;

    public void Run()
    {
        try
        {
            _kernel = ConfigureContainer();
            ConfigureLogging();

            _logger.Debug("Running application...");

            ServiceLocator.Initialize(this);
            ConfigureModules();

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
        kernel.Bind<ILogger>().ToProvider<LoggerProvider>();
        kernel.Bind<TEngine>().ToSelf().InSingletonScope();

        return kernel;
    }

    private void ConfigureLogging()
    {
        LoggerSettings loggerSettings = _kernel.Get<LoggerSettings>();
        loggerSettings.AddOutput(new FileLogOutput(targetDirectory: "logs"));

        _logger = new ContextedLogger(loggerSettings, "App");
    }

    private void ConfigureModules()
    {
        ModuleLoader = Kernel.Get<IModuleLoader>();
        ModuleLoader.RegisterModule(new CoreModule());
        RegisterModules(ModuleLoader);

        ModuleLoader.ConfigureModules();
        if (!ModuleLoader.IsConfigured)
            throw new AppException("The module loader have not been configured correctly.");
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