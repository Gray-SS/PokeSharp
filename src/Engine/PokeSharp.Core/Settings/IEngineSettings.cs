namespace PokeSharp.Core.Settings;

public interface IEngineSettings
{
    bool EditorEnabled { get; }

    void EnableEditor(bool enabled);
}