using PokeLab.Domain;
using PokeLab.Application.Events;
using PokeLab.Application.ProjectManagement.Messages;

namespace PokeLab.Application.ContentBrowser.Messages.Handlers;

public sealed class InitializeContentBrowserHandler(
    IContentBrowserService contentBrowserService,
    IEventDispatcher eventDispatcher
) : IEventHandler<ProjectEvents.Opened>
{
    public async Task HandleAsync(ProjectEvents.Opened ev)
    {
        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshing());

        ev.Deconstruct(out Project project);

        contentBrowserService.Initialize(project);

        await eventDispatcher.PublishAsync(new ContentBrowserEvents.Refreshed(contentBrowserService.Directories, contentBrowserService.Files));
    }
}