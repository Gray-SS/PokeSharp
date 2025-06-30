using Spectre.Console;

namespace PokeTools.Assets.CLI.Services;

public interface ICliConsole
{
    void WriteVerbose(string message);
    void WriteInfo(string message);
    void WriteWarn(string message);
    void WriteSuccess(string message);
    void WriteError(string message);

    T TextPrompt<T>(string prompt);
    bool ConfirmPrompt(string prompt);
    string SelectionPrompt(string title, string helperText, string[] choices);

    void Progress(Action<ProgressContext> action);
    void StatusText(string initialStatus, Action<StatusContext> action);
    Task StatusTextAsync(string initialStatus, Func<StatusContext, Task> action);
}