using PokeCore.Hosting.Abstractions;
using PokeLab.Application.Commands;

namespace PokeLab.Application.Common;

public record class ExitCommand : ICommand;
public sealed class ExitCommandHandler : ICommandHandler<ExitCommand>
{
    private readonly IApp _app;

    public ExitCommandHandler(IApp app)
    {
        _app = app;
    }

    public async Task ExecuteAsync(ExitCommand command)
    {
        await _app.StopAsync();
    }
}