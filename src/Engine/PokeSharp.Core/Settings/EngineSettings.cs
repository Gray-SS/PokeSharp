namespace PokeSharp.Core.Settings;

public sealed class EngineSettings : IEngineSettings
{
    public bool EditorEnabled { get; private set; }

    public void EnableEditor(bool enabled)
    {
        throw new NotImplementedException();
    }
}