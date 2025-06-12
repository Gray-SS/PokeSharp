namespace PokeLab.Presentation.Common;

public interface IWindowService
{
    Task<string?> ShowOpenFileDialogAsync(string defaultPath, string filter);
    Task<string?> ShowOpenDirectoryDialogAsync(string defaultPath);
    Task<string?> ShowOpenSaveDialogAsync(string defaultPath, string filter);
}