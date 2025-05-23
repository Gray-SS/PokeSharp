namespace PokeSharp.Core.Logging;

public interface ILogOutput
{
    string Name { get; }

    void Log(in LogEntry entry);
}