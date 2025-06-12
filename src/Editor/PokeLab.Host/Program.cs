using PokeLab.Host;

using var app = new EditorApp();
await app.StartAsync();
await app.WaitForShutdownAsync();