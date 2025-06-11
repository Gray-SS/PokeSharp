using PokeCore.DependencyInjection.Abstractions;

namespace PokeCore.Logging.Extensions;

public interface ILoggerConfiguration
{
    ILoggerConfiguration AddConsoleLog();
    ILoggerConfiguration AddFileLog(string logDirectory);
    ILoggerConfiguration AddMemoryLog();

    ILoggerConfiguration UseContextLogger();
    ILoggerConfiguration UseEmptyLogger();
}

public sealed class LoggerConfiguration : ILoggerConfiguration
{
    private readonly LoggerSettings _loggerSettings;
    private readonly IServiceCollections _services;

    public LoggerConfiguration(IServiceCollections services)
    {
        _services = services;

        _loggerSettings = new LoggerSettings();
        _services.AddSingleton(sc => _loggerSettings);
    }

    public ILoggerConfiguration AddConsoleLog()
    {
        _loggerSettings.AddOutput(new ConsoleLogSink());
        return this;
    }

    public ILoggerConfiguration AddFileLog(string logDirectory)
    {
        _loggerSettings.AddOutput(new FileLogSink(logDirectory));
        return this;
    }

    public ILoggerConfiguration AddMemoryLog()
    {
        _loggerSettings.AddOutput(new MemoryLogSink());
        return this;
    }

    public ILoggerConfiguration UseContextLogger()
    {
        _services.AddTransient(typeof(Logger<>), typeof(ContextLogger<>));

        _services.AddTransient<Logger>(sc =>
        {
            var settings = sc.GetService<LoggerSettings>();
            return new ContextLogger(settings, "Default");
        });

        return this;
    }

    public ILoggerConfiguration UseEmptyLogger()
    {
        _services.AddTransient<Logger>(sc => new EmptyLogger());
        return this;
    }
}