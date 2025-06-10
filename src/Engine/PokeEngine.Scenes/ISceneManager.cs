using PokeEngine.Scenes.Events;

namespace PokeEngine.Scenes;

public interface ISceneManager
{
    bool HasActiveScene { get; }
    IScene ActiveScene { get; }

    event EventHandler<SceneChangedArgs>? SceneChanged;

    void SetScene(IScene scene);
    IScene GetActiveScene();

    bool CanPushScene();
    bool CanPopScene();

    void PushScene(IScene scene);
    IScene PopScene();
}