namespace PokeLab.Presentation;

/// <summary>
/// Ability to subscribe to an event that is called at each tick.
/// </summary>
/// <remarks>
/// This interface is useful when you need to perform deferred operations.
/// </remarks>
public interface ITickSource
{
    /// <summary>
    /// Event called on each tick
    /// </summary>
    event Action? OnTick;

    /// <summary>
    /// Run the tick source
    /// </summary>
    void Run();
}