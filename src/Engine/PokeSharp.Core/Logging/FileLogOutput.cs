namespace PokeSharp.Core.Logging;

public sealed class FileLogOutput : ILogOutput, IDisposable
{
    public string Name => "File";

    private bool _disposed;
    private string _logDirectory;
    private DateTime _currentDate;
    private StreamWriter _writer = null!;

    public FileLogOutput(string targetDirectory)
    {
        _logDirectory = targetDirectory;
        _currentDate = DateTime.UtcNow.Date;

        Directory.CreateDirectory(targetDirectory);
        OpenLogFile();
    }

    public void Log(in LogEntry entry)
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