using Spectre.Console;

namespace PokeTools.Assets.CLI.Services;

public interface ICliConsole
{
    void WriteVerbose(string message);
    void WriteInfo(string message);
    void WriteWarn(string message);
    void WriteSuccess(string message);
    void WriteError(string message);

    bool Confirm(string prompt);
    void Progress(Action<ProgressContext> action);
    void StatusText(Action<StatusContext> action);
}