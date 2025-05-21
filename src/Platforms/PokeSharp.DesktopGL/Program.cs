using PokeSharp.DesktopGL;

#if DEBUG
using var game = new PokesharpGL();
game.Run();
#else
try
{
    using var game = new PokesharpGL();
    game.Run();
}
catch (EngineException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error occured: {ex.GetType().Name}: {ex.Message}");
    Console.ResetColor();
    Environment.Exit(1);
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Unexpected error occured: {ex.GetType().Name}: {ex.Message}");
    Console.ResetColor();
    Environment.Exit(1);
}
#endif