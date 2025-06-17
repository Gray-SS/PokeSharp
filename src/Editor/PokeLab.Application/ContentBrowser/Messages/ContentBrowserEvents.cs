using PokeCore.IO;
using PokeLab.Application.Events;

namespace PokeLab.Application.ContentBrowser.Messages;

public static class ContentBrowserEvents
{
    public sealed record Navigated(VirtualPath From, VirtualPath To) : IEvent;

    public sealed record Refreshing : IEvent;
    public sealed record Refreshed(IReadOnlyCollection<IVirtualDirectory> Directories, IReadOnlyCollection<IVirtualFile> Files) : IEvent;
}