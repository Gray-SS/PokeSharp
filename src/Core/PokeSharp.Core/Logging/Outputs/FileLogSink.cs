namespace PokeSharp.Core.Logging.Outputs;

/// <summary>
/// Writes log entries to a file in a specified directory, with one file per day (rolling by date).
/// </summary>
/// <remarks>
/// The constructor takes a folder path (e.g. <c>"Logs"</c>). Each day’s entries go into a file named <c>YYYY_MM_DD.txt</c>.
/// </remarks>
public sealed class FileLogSink : ILogSink, IDisposable
{
    public string Name => "File";

    private bool _disposed;
    private string _logDirectory;
    private DateTime _currentDate;
    private StreamWriter _writer = null!;

    public FileLogSink(string targetDirectory)
    {
        _logDirectory = targetDirectory;
        _currentDate = DateTime.UtcNow.Date;

        Directory.CreateDirectory(targetDirectory);
        OpenLogFile();
    }

    public void Log(LogEntry entry)
    {
        var date = DateTime.UtcNow.Date;
        if (date != _currentDate)
        {
            _writer.Dispose();
            _currentDate = date;

            OpenLogFile();
        }

        _writer.WriteLine($"[{entry.TimeStamp:HH:mm:ss}] [{entry.Context}::{entry.Level}] {entry.Message}");
        _writer.Flush();
    }

    private void OpenLogFile()
    {
        string path = Path.Combine(_logDirectory, $"{_currentDate:yyyy-MM-dd}.txt");
        _writer = new StreamWriter(path, append: true)
        {
            AutoFlush = true
        };
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _writer.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}