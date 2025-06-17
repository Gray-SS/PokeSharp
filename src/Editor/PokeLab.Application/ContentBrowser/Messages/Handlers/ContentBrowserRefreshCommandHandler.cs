using PokeCore.IO;
using PokeLab.Application.Commands;
using PokeLab.Application.Events;

namespace PokeLab.Application.ContentBrowser.Messages.Handlers;

public sealed class ContentBrowserRefreshCommandHandler(
    IContentBrowserService contentBrowserService,
    IEventDispatcher eventDispatcher
) : ICommandHandler<ContentBrowserCommands.Refresh>
{
    public async Task HandleAsync(ContentBrowserCommands.Refresh command)
    {
        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshing());

        contentBrowserService.Refresh();

        IReadOnlyCollection<IVirtualFile> files = contentBrowserService.Files;
        IReadOnlyCollection<IVirtualDirectory> directories = contentBrowserService.Directories;

        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshed(directories, files));
    }
}