using PokeCore.IO;
using PokeLab.Application.Commands;

namespace PokeLab.Application.ContentBrowser.Messages;

public static class ContentBrowserCommands
{
    public sealed record NavigateBack : ICommand;
    public sealed record NavigateForward : ICommand;
    public sealed record NavigateTo(VirtualPath Path) : ICommand;
    public sealed record Refresh : ICommand;
}