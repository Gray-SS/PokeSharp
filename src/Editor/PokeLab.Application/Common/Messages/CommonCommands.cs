using PokeLab.Application.Commands;

namespace PokeLab.Application.Common.Messages;

public static class CommonCommands
{
    public sealed record Exit : ICommand;
}