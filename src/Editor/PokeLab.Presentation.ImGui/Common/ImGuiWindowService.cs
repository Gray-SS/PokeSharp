using NativeFileDialogSharp;
using PokeLab.Presentation.Common;

namespace PokeLab.Presentation.ImGui.Common;

public sealed class ImGuiWindowService : IWindowService
{
    public async Task<string?> ShowOpenDirectoryDialogAsync(string defaultPath)
    {
        return await Task.Run(() =>
        {
            DialogResult result = Dialog.FolderPicker(defaultPath);
            return result.IsOk ? result.Path : null;
        });
    }

    public async Task<string?> ShowOpenFileDialogAsync(string defaultPath, string filter)
    {
        return await Task.Run(() =>
        {
            DialogResult result = Dialog.FileOpen(filter, defaultPath);
            return result.IsOk ? result.Path : null;
        });
    }

    public async Task<string?> ShowOpenSaveDialogAsync(string defaultPath, string filter)
    {
        return await Task.Run(() =>
        {
            DialogResult result = Dialog.FileSave(filter, defaultPath);
            return result.IsOk ? result.Path : null;
        });
    }
}