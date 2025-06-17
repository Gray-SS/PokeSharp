using PokeCore.IO;
using PokeLab.Application.Commands;
using PokeLab.Application.Events;

namespace PokeLab.Application.ContentBrowser.Messages.Handlers;

public sealed class ContentBrowserNavigateToCommandHandler(
    IContentBrowserService contentBrowserService,
    IEventDispatcher eventDispatcher
) : ICommandHandler<ContentBrowserCommands.NavigateTo>
{
    public async Task HandleAsync(ContentBrowserCommands.NavigateTo command)
    {
        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshing());

        VirtualPath from = contentBrowserService.CurrentPath;
        contentBrowserService.NavigateTo(command.Path);

        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Navigated(from, contentBrowserService.CurrentPath));
        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshed(contentBrowserService.Directories, contentBrowserService.Files));
    }
}