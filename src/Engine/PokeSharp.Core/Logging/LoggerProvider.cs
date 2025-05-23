using Ninject.Activation;

namespace PokeSharp.Core.Logging;

public class LoggerProvider : Provider<ILogger>
{
    protected override ILogger CreateInstance(IContext context)
    {
        var type = context.Request?.Target?.Member?.DeclaringType ?? typeof(object);
        return LoggerFactory.GetLogger(type);
    }
}
