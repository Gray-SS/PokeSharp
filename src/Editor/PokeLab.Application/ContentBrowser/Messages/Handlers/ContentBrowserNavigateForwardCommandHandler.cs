using PokeCore.IO;
using PokeLab.Application.Events;
using PokeLab.Application.Commands;

namespace PokeLab.Application.ContentBrowser.Messages.Handlers;

public sealed class ContentBrowserNavigateForwardCommandHandler(
    IContentBrowserService contentBrowserService,
    IEventDispatcher eventDispatcher
) : ICommandHandler<ContentBrowserCommands.NavigateForward>
{
    public async Task HandleAsync(ContentBrowserCommands.NavigateForward command)
    {
        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshing());

        VirtualPath from = contentBrowserService.CurrentPath;
        contentBrowserService.NavigateForward();

        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Navigated(from, contentBrowserService.CurrentPath));
        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshed(contentBrowserService.Directories, contentBrowserService.Files));
    }
}