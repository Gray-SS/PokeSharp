using PokeLab.Domain;

namespace PokeLab.Application.ProjectManagement;

public interface IProjectManager
{
    bool IsOpen { get; }
    Project Current { get; }

    Task NewAsync(string name, string basePath);
    Task OpenAsync(string projectFilePath);
    Task DeleteAsync(string projectFilePath);
    Task SaveAsync();
    Task CloseAsync();
}