using PokeLab.Application.Events;
using PokeLab.Domain;

namespace PokeLab.Application.ProjectManagement.Messages;

public static class ProjectEvents
{
    public sealed record Closed(Project Project) : IEvent;
    public sealed record CloseFailed(string Reason) : IEvent;

    public sealed record Created(Project Project) : IEvent;
    public sealed record CreationFailed(string Reason) : IEvent;

    public sealed record Deleted(Project Project) : IEvent;
    public sealed record DeletionFailed(string Reason) : IEvent;

    public sealed record Saved(Project Project) : IEvent;
    public sealed record SaveFailed(string Reason) : IEvent;

    public sealed record Opened(Project Project) : IEvent;
    public sealed record OpenFailed(string Reason) : IEvent;
}