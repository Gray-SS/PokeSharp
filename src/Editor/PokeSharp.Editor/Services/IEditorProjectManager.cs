using System.Diagnostics.CodeAnalysis;

namespace PokeSharp.Editor.Services;

public interface IEditorProjectManager
{
    bool HasActiveProject { get; }
    EditorProject? ActiveProject { get; }

    event EventHandler<EditorProject>? ProjectOpened;

    bool TryOpenProject(string projectPath, [NotNullWhen(true)] out EditorProject? openedProject);
    bool TryCreateProject(string projectName, string projectPath, bool openOnCreation, [NotNullWhen(true)] out EditorProject? createdProject);
    bool TryDeleteProject(string projectPath);
}