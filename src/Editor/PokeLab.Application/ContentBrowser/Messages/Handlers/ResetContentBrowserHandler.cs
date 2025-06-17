using PokeLab.Application.Events;
using PokeLab.Application.ProjectManagement.Messages;

namespace PokeLab.Application.ContentBrowser.Messages.Handlers;

public sealed class ResetContentBrowserHandler(
    IContentBrowserService contentBrowserService,
    IEventDispatcher eventDispatcher
) : IEventHandler<ProjectEvents.Closed>
{
    public async Task HandleAsync(ProjectEvents.Closed ev)
    {
        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshing());

        contentBrowserService.Reset();

        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshed(contentBrowserService.Directories, contentBrowserService.Files));
    }
}