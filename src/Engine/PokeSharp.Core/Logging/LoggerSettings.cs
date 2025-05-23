namespace PokeSharp.Core.Logging;

public sealed class LoggerSettings
{
    public bool IsBootingLogEnabled { get; set; }
    public LogLevel LogLevel { get; set; }
    public List<ILogOutput> Outputs => _outputs;

    private readonly List<ILogOutput> _outputs;

    public LoggerSettings()
    {
        _outputs = new List<ILogOutput>();
    }

    public void SetLogLevel(LogLevel level)
    {
        LogLevel = level;
    }

    public void ClearOutputs()
    {
        _outputs.Clear();
    }

    public void AddOutput(ILogOutput output)
    {
        _outputs.Add(output);
    }

    public void RemoveOutput(ILogOutput output)
    {
        _outputs.Remove(output);
    }
}