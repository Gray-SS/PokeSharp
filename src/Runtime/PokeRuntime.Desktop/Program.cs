using PokeCore.Hosting.Abstractions;
using PokeRuntime.Desktop;

IApp app = new PokesharpApp();
await app.StartAsync();
await app.WaitForShutdownAsync();