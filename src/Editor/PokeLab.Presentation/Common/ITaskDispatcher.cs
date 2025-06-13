namespace PokeLab.Presentation.Common;

public interface ITaskDispatcher
{
    void RunOnUIThread(Action? deferredAction);
    void FireAndForget(Action? backgroundTask);
}