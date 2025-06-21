using PokeCore.DependencyInjection.Abstractions;
using PokeCore.DependencyInjection.Abstractions.Extensions;

namespace PokeCore.Logging.Extensions;

public interface ILoggerConfiguration
{
    LoggerSettings Settings { get; }

    ILoggerConfiguration AddOutput(ILogSink output);
    ILoggerConfiguration AddOutput<TOutput>() where TOutput : ILogSink, new();

    ILoggerConfiguration AddConsoleLog();
    ILoggerConfiguration AddFileLog(string logDirectory);
    ILoggerConfiguration AddMemoryLog();

    ILoggerConfiguration UseLogLevel(LogLevel level = LogLevel.Debug);
    ILoggerConfiguration UseContextLogger();
    ILoggerConfiguration UseEmptyLogger();
}

public sealed class LoggerConfiguration : ILoggerConfiguration
{
    public LoggerSettings Settings => _settings;

    private readonly LoggerSettings _settings;
    private readonly IServiceCollections _services;

    public LoggerConfiguration(IServiceCollections services)
    {
        _services = services;

        _settings = new LoggerSettings();
        _services.AddSingleton(sc => _settings);
    }

    public ILoggerConfiguration AddOutput<TOutput>() where TOutput : ILogSink, new()
    {
        _settings.AddOutput(new TOutput());
        return this;
    }

    public ILoggerConfiguration AddOutput(ILogSink output)
    {
        _settings.AddOutput(output);
        return this;
    }

    public ILoggerConfiguration AddConsoleLog()
    {
        _settings.AddOutput(new ConsoleLogSink());
        return this;
    }

    public ILoggerConfiguration AddFileLog(string logDirectory)
    {
        _settings.AddOutput(new FileLogSink(logDirectory));
        return this;
    }

    public ILoggerConfiguration AddMemoryLog()
    {
        _settings.AddOutput(new MemoryLogSink());
        return this;
    }

    public ILoggerConfiguration UseLogLevel(LogLevel logLevel)
    {
        _settings.SetLogLevel(logLevel);
        return this;
    }

    public ILoggerConfiguration UseContextLogger()
    {
        _services.AddTransient(typeof(Logger<>), typeof(ContextLogger<>));

        _services.AddTransient<Logger>(sc =>
        {
            var settings = sc.GetRequiredService<LoggerSettings>();
            return new ContextLogger(settings, "Default");
        });

        return this;
    }

    public ILoggerConfiguration UseEmptyLogger()
    {
        _services.AddTransient(typeof(Logger<>), typeof(EmptyLogger<>));
        _services.AddTransient<Logger>(sc => new EmptyLogger());
        return this;
    }
}