namespace PokeCore.Hosting.Modules.Events;

public sealed class ModuleStateChangedArgs : EventArgs
{
    public Module Module { get; }
    public ModuleState NewState { get; }
    public ModuleState OldState { get; }

    public ModuleStateChangedArgs(Module module, ModuleState newState, ModuleState oldState)
    {
        Module = module;
        NewState = newState;
        OldState = oldState;
    }
}