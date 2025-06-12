using PokeCore.Diagnostics;
using PokeLab.Application.ProjectManagement;
using PokeLab.Domain;

namespace PokeLab.Infrastructure.ProjectManagement;

public sealed class FileProjectRepository : IProjectRepository
{
    public async Task<Project> LoadAsync(string projectFilePath)
    {
        ThrowHelper.AssertNotNullOrWhitespace(projectFilePath);

        string yaml = await File.ReadAllTextAsync(projectFilePath);

        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().IgnoreFields().Build();
        ProjectData projectData = deserializer.Deserialize<ProjectData>(yaml);
        return Project.FromData(projectData);
    }

    public async Task SaveAsync(Project project)
    {
        ThrowHelper.AssertNotNull(project);

        ProjectData projectData = project.GetData();

        var serializer = new YamlDotNet.Serialization.SerializerBuilder().IgnoreFields().Build();
        string yaml = serializer.Serialize(projectData);

        string filePath = Path.Combine(project.RootPath, $"{project.Name}.pkproj");
        await File.WriteAllTextAsync(filePath, yaml);
    }
}