// using Microsoft.Xna.Framework;
// using PokeCore.Common;
// using PokeCore.Logging;

// namespace PokeEngine.Core.Services;

// public sealed class EngineHookDispatcher : IEngineHookDispatcher
// {
//     private readonly Logger _logger;
//     private readonly IGameLoop[] _hooks;

//     public EngineHookDispatcher(Logger<EngineHookDispatcher> logger, IReflectionManager reflectionManager)
//     {
//         _logger = logger;
//         _hooks = reflectionManager.InstantiateClassesOfType<IGameLoop>();

//         if (_hooks.Length == 0)
//             _logger.Warn("No engine hooks found. Ensure you've loaded your modules correcty.");
//     }

//     public void Initialize()
//     {
//         if (_hooks.Length == 0)
//             return;

//         _logger.Debug("Initializing engine hooks...");
//         foreach (IGameLoop hook in _hooks)
//         {
//             _logger.Trace($"Initializing engine hook: {hook.GetType().Name}");
//             hook.Initialize();
//         }
//     }

//     public void Update(GameTime gameTime)
//     {
//         foreach (IGameLoop hook in _hooks)
//         {
//             hook.Update(gameTime);
//         }
//     }

//     public void Draw(GameTime gameTime)
//     {
//         foreach (IGameLoop hook in _hooks)
//         {
//             hook.Draw(gameTime);
//         }
//     }
// }