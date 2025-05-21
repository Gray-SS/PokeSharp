namespace PokeSharp.Core.Resolutions.Events;

public sealed class ResolutionChangedArgs : EventArgs
{
    public ResolutionManager ResolutionManager { get; }
    public ResolutionSize NewResolution { get; }
    public ResolutionSize OldResolution { get; }

    public ResolutionChangedArgs(ResolutionManager resolutionManager, ResolutionSize newResolution, ResolutionSize oldResolution)
    {
        ArgumentNullException.ThrowIfNull(resolutionManager, nameof(resolutionManager));

        ResolutionManager = resolutionManager;
        NewResolution = newResolution;
        OldResolution = oldResolution;
    }
}