using System.Linq;
using PokeCore.Hosting;
using PokeRuntime.Desktop;
using PokeLab;

bool editor = args.Contains("--editor");

IApp app = editor ? new EditorApp() : new PokesharpApp();
app.Run();