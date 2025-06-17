using PokeCore.IO;
using PokeLab.Application.Commands;
using PokeLab.Application.Events;

namespace PokeLab.Application.ContentBrowser.Messages.Handlers;

public sealed class ContentBrowserNavigateBackCommandHandler(
    IContentBrowserService contentBrowserService,
    IEventDispatcher eventDispatcher
) : ICommandHandler<ContentBrowserCommands.NavigateBack>
{
    public async Task HandleAsync(ContentBrowserCommands.NavigateBack command)
    {
        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshing());

        VirtualPath from = contentBrowserService.CurrentPath;
        contentBrowserService.NavigateBack();

        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Navigated(from, contentBrowserService.CurrentPath));
        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshed(contentBrowserService.Directories, contentBrowserService.Files));
    }
}
