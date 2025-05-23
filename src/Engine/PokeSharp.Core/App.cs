using Ninject;
using Ninject.Infrastructure;
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

            ServiceLocator.Initialize(this);

            ConfigureLogging();

            _logger.Debug("Starting application...");
            RegisterAppModules();

            _logger.Debug("Contextual logger configured.");

            using TEngine engine = _kernel.Get<TEngine>();
            engine.Run();
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
        kernel.Bind<IReflectionManager>().To<ReflectionManager>().InSingletonScope();
        kernel.Bind<IModuleLoader>().To<ModuleLoader>().InSingletonScope();

        return kernel;
    }

    private void ConfigureLogging()
    {
        _kernel.Bind<LoggerSettings>().ToSelf().InSingletonScope();

        LoggerSettings loggerSettings = _kernel.Get<LoggerSettings>();
        loggerSettings.AddOutput(new FileLogOutput(targetDirectory: "logs"));

        _kernel.Bind<ILogger>().To<ContextedLogger>().WithConstructorArgument("context", "App");

        _logger = _kernel.Get<ILogger>();
        _logger.Debug("Logging configured");
    }

    private void RegisterAppModules()
    {
        ModuleLoader = Kernel.Get<IModuleLoader>();

        _logger.Debug("Registering modules...");
        ModuleLoader.RegisterModule(new CoreModule<TEngine>());
        RegisterModules(ModuleLoader);
    }

    protected virtual void RegisterModules(IModuleLoader loader)
    {
    }

    private void Cleanup()
    {
        _logger?.Info("Shutting down application...");
        _kernel?.Dispose();
        ServiceLocator.Cleanup();
    }
}