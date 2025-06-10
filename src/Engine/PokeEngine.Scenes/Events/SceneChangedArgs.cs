namespace PokeEngine.Scenes.Events;

public sealed class SceneChangedArgs : EventArgs
{
    public IScene NewScene { get; }

    public IScene? OldScene { get; }

    public ISceneManager SceneManager { get; }

    public SceneChangedArgs(ISceneManager manager, IScene newScene, IScene? oldScene)
    {
        NewScene = newScene;
        OldScene = oldScene;
        SceneManager = manager;
    }
}