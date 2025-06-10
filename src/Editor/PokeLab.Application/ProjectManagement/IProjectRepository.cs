using PokeLab.Domain;

namespace PokeLab.Application.ProjectManagement;

public interface IProjectRepository
{
    Project Load(string basePath);
    void Save(Project project, string basePath);
}