namespace PokeLab.Presentation.MainMenu;

public sealed class CreateProjectFormViewModel
{
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectPath { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }

    public bool IsVisible { get; private set; }

    public void Show() => IsVisible = true;
    public void Hide()
    {
        IsVisible = false;
        ErrorMessage = null;
    }

    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(ProjectName) || string.IsNullOrWhiteSpace(ProjectPath))
        {
            ErrorMessage = "Project name and path must not be empty.";
            return false;
        }

        ErrorMessage = null;
        return true;
    }

    public void Reset()
    {
        ProjectName = string.Empty;
        ProjectPath = string.Empty;
        ErrorMessage = null;
        IsVisible = false;
    }
}