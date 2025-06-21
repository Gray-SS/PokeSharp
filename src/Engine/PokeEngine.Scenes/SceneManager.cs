using PokeEngine.Core.Exceptions;
using PokeEngine.Scenes.Events;

namespace PokeEngine.Scenes;

public sealed class SceneManager : ISceneManager
{
    public bool HasActiveScene => _scenes.Count > 0;
    public IScene ActiveScene => GetActiveScene();

    public event EventHandler<SceneChangedArgs>? SceneChanged;

    private readonly Stack<IScene> _scenes;

    public SceneManager()
    {
        _ = SceneChanged;
        _scenes = new Stack<IScene>();
    }

    public bool CanPushScene()
    {
        throw new NotImplementedException();
    }

    public bool CanPopScene()
    {
        throw new NotImplementedException();
    }

    public IScene PopScene()
    {
        if (!_scenes.TryPop(out IScene? scene))
            throw new EngineException("Pop scene failed - No active scene have been found");

        return scene;
    }

    public void PushScene(IScene scene)
    {
    }

    public void SetScene(IScene scene)
    {
    }

    public IScene GetActiveScene()
    {
        if (!HasActiveScene)
            throw new EngineException("No scene have been registered.");

        return _scenes.Peek();
    }
}