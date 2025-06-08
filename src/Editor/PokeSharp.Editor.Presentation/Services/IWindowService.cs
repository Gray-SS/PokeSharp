namespace PokeSharp.Editor.Presentation.Services;

public interface IWindowService
{
    string ShowOpenFileDialog(string title, string filter);
    Task<string> ShowOpenFileDialogAsync(string title, string filter);

    string ShowOpenDirectory(string title, string filter);
    Task<string> ShowOpenDirectoryDialogAsync(string title, string filter);

    string ShowOpenSaveDialog(string title, string defaultFileName, string filter);
    Task<string> ShowOpenSaveDialogAsync(string title, string defaultFileName, string filter);
}