using Ninject;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules.Exceptions;
using PokeSharp.Core.Services;

namespace PokeSharp.Core.Modules;

public sealed class ModuleLoader : IModuleLoader
{
    public bool IsConfigured => _state >= State.Configured;
    public bool IsLoaded => _state >= State.Loaded;

    public IReadOnlyCollection<IModule> LoadedModules => _modules.AsReadOnly();

    private State _state;
    private readonly IKernel _kernel;
    private readonly ILogger _logger;
    private readonly List<IModule> _modules = new();

    public ModuleLoader(IKernel kernel, ILogger logger)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _state = State.Setup;
    }

    public void RegisterModule(IModule module)
    {
        if (_state != State.Setup)
        {
            string msg = $"Cannot register module '{module?.ModuleName}'. Module registration is only allowed during the setup phase.";
            _logger.Fatal(msg);
            throw new ModuleException(msg);
        }

        if (module == null)
        {
            string msg = "Attempted to register a null module.";
            _logger.Fatal(msg);
            throw new ModuleException(msg);
        }

        if (_modules.Any(x => x.ModuleName == module.ModuleName))
        {
            _logger.Warn($"Module '{module.ModuleName}' is already registered. Skipping.");
            return;
        }

        _logger.Debug($"Registering module '{module.ModuleName}'...");
        _modules.Add(module);
    }

    public void ConfigureModules()
    {
        if (_state != State.Setup)
        {
            string msg = $"Cannot configure modules. Make sure to call {nameof(ConfigureModules)}() only after registering all modules.";
            _logger.Fatal(msg);
            throw new ModuleException(msg);
        }

        if (_modules.Count == 0)
            throw new ModuleException("No module has been loaded. Cannot continue without modules.");

        // if (!loadedModules.Any(x => x is CoreModule<TEngine>))
        //     throw new AppException("The core module wasn't loaded. This is required.");

        if (_modules.Count == 1)
        {
            // Note: This means that only the Core module was loaded.
            // This is not an error in itself, but it should not happen, so we are logging a warning.
            _logger.Warn("The core module was the only loaded module. If this is intentional, please ensure that you are not using any module-related content.");
        }

        _logger.Debug("Configuring modules...");
        foreach (IModule module in _modules)
        {
            _logger.Debug($"Configuring module '{module.ModuleName}'...");
            module.Configure(_kernel);

            Type moduleType = module.GetType();
            _kernel.Bind(moduleType).ToConstant(module);
            _kernel.Bind<IModule>().ToConstant(module);
        }

        _state = State.Configured;
    }

    public void LoadModules()
    {
        if (_state != State.Configured)
        {
            string msg = $"Cannot load modules. Make sure to call {nameof(LoadModules)}() after {nameof(ConfigureModules)}().";
            _logger.Fatal(msg);
            throw new ModuleException(msg);
        }

        _logger.Debug("Loading modules...");
        foreach (IModule module in _modules)
        {
            _logger.Debug($"Loading module '{module.ModuleName}'...");

            Type moduleType = module.GetType();
            IReflectionManager reflectionManager = _kernel.Get<IReflectionManager>();
            reflectionManager.RegisterAssembly(moduleType.Assembly);

            module.Load();

            _logger.Info($"Module loaded: '{module.ModuleName}'");
        }

        _state = State.Loaded;
    }

    private enum State
    {
        Setup,
        Configured,
        Loaded
    }
}
