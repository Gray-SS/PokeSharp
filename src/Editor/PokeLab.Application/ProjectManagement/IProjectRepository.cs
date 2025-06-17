using PokeLab.Domain;

namespace PokeLab.Application.ProjectManagement;

public interface IProjectRepository
{
    Task SaveAsync(Project project);
    Task<Project> LoadAsync(string projectFilePath);
}