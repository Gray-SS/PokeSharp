using PokeLab.Host;

using var app = new PokeLabApplication();
await app.StartAsync();
await app.WaitForShutdownAsync();