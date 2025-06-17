using PokeCore.IO;
using PokeLab.Application.ContentBrowser.Messages;
using PokeLab.Application.Events;
using PokeLab.Presentation.Common;

namespace PokeLab.Presentation.ContentBrowser;

public sealed class ContentBrowserViewModelHandler(
    ContentBrowserViewModel viewModel,
    ITaskDispatcher taskDispatcher
) : IEventHandler<ContentBrowserEvents.Refreshed>,
    IEventHandler<ContentBrowserEvents.Refreshing>
{
    public Task HandleAsync(ContentBrowserEvents.Refreshed e)
    {
        taskDispatcher.RunOnUIThread(() =>
        {
            viewModel.IsLoading = false;

            viewModel.Items.Clear();
            foreach (IVirtualDirectory directory in e.Directories)
            {
                var item = new ContentBrowserFolderItem(directory.Name, directory.Path);
                viewModel.Items.Add(item);
            }
        });

        return Task.CompletedTask;
    }

    public Task HandleAsync(ContentBrowserEvents.Refreshing e)
    {
        taskDispatcher.RunOnUIThread(() =>
        {
            viewModel.IsLoading = true;
        });

        return Task.CompletedTask;
    }
}