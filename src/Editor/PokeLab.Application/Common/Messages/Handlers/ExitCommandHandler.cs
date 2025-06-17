using PokeCore.Hosting.Abstractions;
using PokeLab.Application.Commands;

namespace PokeLab.Application.Common.Messages.Handlers;

public sealed class ExitCommandHandler(
    IApp app
) : ICommandHandler<CommonCommands.Exit>
{
    public async Task HandleAsync(CommonCommands.Exit command)
    {
        await app.StopAsync();
    }
}