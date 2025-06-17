namespace PokeLab.Presentation.Common;

public interface ITaskDispatcher
{
    void RunOnUIThread(Action? callback);
    void FireAndForget(Action? backgroundTask);
}