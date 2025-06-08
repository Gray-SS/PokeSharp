using PokeSharp.Editor.Domain;

namespace PokeSharp.Editor.Application.ProjectManagement;

public interface IProjectRepository
{
    Project Load(string basePath);
    void Save(Project project, string basePath);
}