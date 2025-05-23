using System.Linq;
using PokeSharp.Core;
using PokeSharp.DesktopGL;
using PokeSharp.Editor;

bool editor = args.Contains("--editor");

IApp app = editor ? new EditorApp() : new PokesharpApp();
app.Run();