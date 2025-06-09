using Ninject.Activation;

namespace PokeSharp.Core.Logging;

/// <summary>
/// Ninject provider that supplies <see cref="Logger"/> instances with automatic context binding.
/// </summary>
/// <remarks>
/// This provider resolves the target type from the injection context and delegates logger creation
/// to the <see cref="LoggerFactory"/>.
/// <br/><br/>
/// <b>Note:</b> If <see cref="Logger"/> is injected into an abstract base class, the inferred context might refer to the base type rather than the actual derived class.
/// <i>For example</i>, if <c>Foo</c> is abstract and defines a constructor <c>Foo(ILogger logger)</c>, and <c>Bar</c> inherits <c>Foo</c>, the resolved logger in <c>Bar(ILogger logger)</c> may be bound to <c>Foo</c> instead of <c>Bar</c>.
/// <br/><br/>
/// <i>You may refer to how the <see cref="Core"/> module handles context-aware resolution in such inheritance scenarios, typically by resolving the <see cref="Logger"/> using <see cref="LoggerFactory.GetLogger(Type)"/>.</i>
/// </remarks>
public class LoggerProvider : Provider<Logger>
{
    /// <summary>
    /// Creates an <see cref="Logger"/> instance using the type requesting the injection as context.
    /// </summary>
    /// <param name="context">The Ninject injection context.</param>
    /// <returns>An instance of <see cref="Logger"/> with appropriate context.</returns>
    protected override Logger CreateInstance(IContext context)
    {
        var type = context.Request?.Target?.Member?.DeclaringType ?? typeof(object);
        return LoggerFactory.GetLogger(type);
    }
}