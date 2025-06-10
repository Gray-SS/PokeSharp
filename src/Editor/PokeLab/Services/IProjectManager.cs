using System.Diagnostics.CodeAnalysis;

namespace PokeLab.Services;

public interface IProjectManager
{
    bool HasActiveProject { get; }

    [MemberNotNullWhen(true, nameof(HasActiveProject))]
    Project? ActiveProject { get; }

    event EventHandler<Project>? ProjectOpened;

    bool TryOpenProject(string projectPath, [NotNullWhen(true)] out Project? openedProject);
    bool TryCreateProject(string projectName, string projectPath, bool openOnCreation, [NotNullWhen(true)] out Project? createdProject);
    bool TryDeleteProject(string projectPath);
}