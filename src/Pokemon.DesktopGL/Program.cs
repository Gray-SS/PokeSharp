using System;
using System.IO;
using Pokemon.DesktopGL;

string romPath = null;

if (args.Length >= 1)
{
    romPath = args[0];

    if (!File.Exists(romPath))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: The specified ROM path doesn't exists: {romPath}.");
        Console.ResetColor();

        // Exit with error status code if the specified ROM path doesn't exists
        Environment.Exit(1);
    }

    string extension = Path.GetExtension(romPath).ToLower();
    if (extension != ".gba")
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Warning: The file doesn't look like to be a supported ROM.");
        Console.ResetColor();
        Console.WriteLine("Would you still like to continue? (Y/N)");

        ConsoleKey key;
        Console.Write("> ");
        do key = Console.ReadKey(true).Key;
        while (key != ConsoleKey.Y && key != ConsoleKey.N);
        Console.WriteLine(key);

        if (key == ConsoleKey.N)
        {
            Console.WriteLine("Exiting...");
            Environment.Exit(1);
        }
    }
}

using var game = new PokemonGame(romPath);
game.Run();