using Ninject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Core.Coroutines;
using PokeSharp.Core.Modules;
using PokeSharp.Core.Resolutions;
using PokeSharp.Core.Services;
using PokeSharp.Core.Windowing;

namespace PokeSharp.Core;

public class EngineModule : Module
{
    public override string Name => "Engine";

    public override void Configure(IKernel kernel)
    {
        kernel.Bind<IEngineHookDispatcher>().To<EngineHookDispatcher>().InSingletonScope();
        kernel.Bind<ICoroutineManager>().To<CoroutineManager>().InSingletonScope();
        kernel.Bind<IResolutionManager>().To<ResolutionManager>().InSingletonScope();
        kernel.Bind<IDynamicTypeResolver>().To<DynamicTypeResolver>().InSingletonScope();

        kernel.Bind<IWindowManager>().To<WindowManager>();
    }

    public override void Load()
    {
        IKernel kernel = App.Kernel;
        Engine engine = kernel.Get<Engine>();

        kernel.Bind<GameWindow>().ToConstant(engine.Window);
        kernel.Bind<ContentManager>().ToConstant(engine.Content);
        kernel.Bind<GraphicsDeviceManager>().ToConstant(engine.Graphics);
        kernel.Bind<GraphicsDevice>().ToConstant(engine.GraphicsDevice);

        engine.InjectDispatcher(kernel.Get<IEngineHookDispatcher>());
    }
}