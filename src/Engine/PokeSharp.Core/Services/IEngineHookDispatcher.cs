using Microsoft.Xna.Framework;

namespace PokeSharp.Core.Services;

/// <summary>
/// This interface is responsible for operating the <see cref="IEngineHook"/>. It allows you to initialize, update and draw them.
/// </summary>
public interface IEngineHookDispatcher
{
    /// <summary>
    /// Initializes all loaded hooks
    /// </summary>
    void Initialize();

    /// <summary>
    /// Updates all loaded hooks
    /// </summary>
    /// <param name="gameTime">Monogame class for game time</param>
    void Update(GameTime gameTime);

    /// <summary>
    /// Draw all loaded hooks
    /// </summary>
    /// <param name="gameTime">Monogame class for game time</param>
    void Draw(GameTime gameTime);
}