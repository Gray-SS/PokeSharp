using PokeLab.Application.Commands;

namespace PokeLab.Application.ProjectManagement.Messages;

public static class ProjectCommands
{
    public sealed record Create(string Name, string BasePath) : ICommand;
    public sealed record Open(string FileProjectPath) : ICommand;
    public sealed record Delete(string FileProjectPath) : ICommand;
    public sealed record Save : ICommand;
    public sealed record Close : ICommand;
}