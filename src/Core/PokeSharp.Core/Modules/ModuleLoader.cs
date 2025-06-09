using Ninject;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules.Events;
using PokeSharp.Core.Modules.Exceptions;
using PokeSharp.Core.Services;

namespace PokeSharp.Core.Modules;

public sealed class ModuleLoader : IModuleLoader
{
    public bool IsConfigured => _state >= State.Configured;
    public bool IsLoaded => _state >= State.Loaded;

    public IEnumerable<IModule> LoadedModules => _modules.Where(x => x.State == ModuleState.Loaded);
    public IEnumerable<IModule> RegisteredModules => _modules;

    private State _state;
    private readonly IKernel _kernel;
    private readonly Logger _logger;
    private readonly List<IModuleInternal> _modules = new();

    public event EventHandler<ModuleStateChangedArgs>? ModuleStateChanged;

    public ModuleLoader(IKernel kernel, Logger logger)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _state = State.Setup;
    }

    public void RegisterModule(IModule module)
    {
        if (module == null)
        {
            string msg = "Attempted to register a null module.";
            _logger.Fatal(msg);
            throw new ModuleException(msg);
        }

        if (string.IsNullOrEmpty(module.Name))
        {
            string msg = $"Attempted to register a module with a null or empty name: '{module.GetType().Name}'.";
            _logger.Fatal(msg);
            throw new ModuleException(msg);
        }

        if (_state != State.Setup)
        {
            string msg = $"Cannot register module '{module.Name}'. Module registration is only allowed during the setup phase.";
            _logger.Fatal(msg);
            throw new ModuleException(msg);
        }

        if (module.State != ModuleState.NotInitialized)
        {
            _logger.Warn($"Cannot register module '{module.Name}'. Module registration is only allowed during the setup phase");
        }

        if (_modules.Any(x => x.Name == module.Name))
        {
            _logger.Warn($"Module '{module.Name}' is already registered. Skipping.");
            return;
        }

        if (module is not IModuleInternal internalModule)
        {
            string msg = $"Module '{module.Name}' isn't implementing the required '{typeof(IModuleInternal).Name}' interface";
            _logger.Fatal(msg);
            throw new ModuleException(msg);
        }

        _logger.Debug($"Registering module '{module.Name}'...");
        module.RegisterSubModules(this);
        module.StateChanged += OnModuleStateChanged;

        _modules.Add(internalModule);
        internalModule.SetState(ModuleState.Registered);
    }

    private void OnModuleStateChanged(object? sender, ModuleStateChangedArgs e)
    {
        ModuleStateChanged?.Invoke(this, e);
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
            throw new ModuleException("No module has been registered. Cannot continue without modules.");

        if (_modules.Count == 1)
        {
            // Note: This means that only the Core module was registered.
            // This is not an error in itself, but it should not happen, so we are logging a warning.
            _logger.Warn("The core module was the only registered module. " +
                         $"If this is not intentional, please ensure that you're registering your modules inside your App via RegisterModules(IModuleLoader loader)." +
                         "Otherwise, please ensure that you are not using any module-related content (e.g. Inputs, Coroutines, ect).");
        }

        _logger.Debug("Configuring modules...");
        foreach (IModuleInternal module in _modules)
        {
            if (module.State != ModuleState.Registered)
            {
                _logger.Warn($"The module '{module.Name}' is not in the correct state. Expected '{ModuleState.Registered}' but was '{module.State}'. Skipping.");
                continue;
            }

            _logger.Trace($"Configuring module '{module.Name}'...");
            module.Configure(_kernel);

            Type moduleType = module.GetType();
            _kernel.Bind(moduleType).ToConstant(module);
            _kernel.Bind<IModule>().ToConstant(module);
        }

        foreach (IModuleInternal module in _modules.Where(x => x.State == ModuleState.Registered))
        {
            LoggerSettings settings = _kernel.Get<LoggerSettings>();
            module.ConfigureLogging(settings, _kernel);

            module.SetState(ModuleState.Configured);
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

        _logger.Debug("Loading modules assemblies...");
        foreach (IModuleInternal module in _modules)
        {
            if (module.State != ModuleState.Configured)
            {
                _logger.Warn($"The module '{module.Name}' is not in the correct state. Expected '{ModuleState.Configured}' but was '{module.State}'. Skipping.");
                continue;
            }

            Type moduleType = module.GetType();
            IReflectionManager reflectionManager = _kernel.Get<IReflectionManager>();
            reflectionManager.RegisterAssembly(moduleType.Assembly);
        }

        _logger.Debug("Loading modules...");
        foreach (IModuleInternal module in _modules.Where(x => x.State == ModuleState.Configured))
        {
            _logger.Trace($"Loading module '{module.Name}'...");
            module.Load();
            _logger.Info($"Module loaded: '{module.Name}'");

            module.SetState(ModuleState.Loaded);
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
