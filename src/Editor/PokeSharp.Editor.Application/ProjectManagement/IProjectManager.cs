using PokeSharp.Editor.Domain;

namespace PokeSharp.Editor.Application.ProjectManagement;

public interface IProjectManager
{
    bool HasProject { get; }
    Project Current { get; }

    Task NewAsync(string name, string basePath);
    Task LoadAsync(string basePath);
    Task DeleteAsync(string basePath);
    Task SaveAsync();
    Task CloseAsync();
}