using PokeLab.Domain;

namespace PokeLab.Application.ProjectManagement;

public interface IProjectService
{
    bool IsOpen { get; }
    Project Current { get; }

    Task<Result<Project>> CreateAsync(string name, string basePath);
    Task<Result<Project>> OpenAsync(string projectFilePath);
    Task<Result<Project>> SaveAsync();
    Task<Result<Project>> DeleteAsync(string projectFilePath);
    Task<Result<Project>> CloseAsync();
}