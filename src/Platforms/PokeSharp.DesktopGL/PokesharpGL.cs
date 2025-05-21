using Microsoft.Xna.Framework;
using PokeSharp.Core;
using PokeSharp.Core.Resolutions;

namespace PokeSharp.DesktopGL;

public class PokesharpGL : Engine, IEngineHook
{
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
    }
}