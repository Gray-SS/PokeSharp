using Ninject;
using Microsoft.Xna.Framework;
using PokeSharp.Assets.Extensions;
using PokeSharp.Core;
using PokeSharp.Core.Resolutions;
using PokeSharp.Editor.Extensions;
using PokeSharp.Inputs.Extensions;
using PokeSharp.Rendering.Extensions;

namespace PokeSharp.DesktopGL;

public class PokesharpGame : Engine<PokesharpGame>, IEngineHook
{
    public bool EditorEnabled { get; }

    public PokesharpGame(bool editorEnabled)
    {
        EditorEnabled = editorEnabled;
    }

    protected override void LoadModules(IKernel kernel)
    {
        kernel.LoadAssetsModule();
        kernel.LoadInputsModule();
        kernel.LoadRenderingModule();

        if (EditorEnabled)
        {
            kernel.LoadEditorModule();
        }
    }

    void IEngineHook.Initialize()
    {
        // Sets the resolution to 1280x720 by default
        Resolution.SetResolution(ResolutionSize.R1280x720);
    }

    void IEngineHook.Update(GameTime gameTime)
    {
    }

    void IEngineHook.Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
    }
}