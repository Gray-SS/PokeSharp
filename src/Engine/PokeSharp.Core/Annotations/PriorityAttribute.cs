namespace PokeSharp.Core.Annotations;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class PriorityAttribute : Attribute
{
    public int Priority { get; }

    public PriorityAttribute(int priority)
    {
        Priority = priority;
    }
}