using Spectre.Console;

namespace PokeTools.Assets.CLI.Services;

public sealed class CliConsole : ICliConsole
{
    public void WriteVerbose(string message)
        => AnsiConsole.MarkupLine($"[dim]{message}[/]");

    public void WriteInfo(string message)
        => AnsiConsole.MarkupLine($"[white]{message}[/]");

    public void WriteSuccess(string message)
        => AnsiConsole.MarkupLine($"[bold green]✔ {message}[/]");

    public void WriteWarn(string message)
        => AnsiConsole.MarkupLine($"[bold yellow]⚠ {message}[/]");

    public void WriteError(string message)
        => AnsiConsole.MarkupLine($"[bold red]✖ {message}[/]");

    public bool Confirm(string prompt)
    {
        return AnsiConsole.Prompt(
            new ConfirmationPrompt(prompt)
                .ChoicesStyle("bold dim")
                .HideDefaultValue()
        );
    }

    public void Progress(Action<ProgressContext> action)
    {
        var progress = AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn(Spinner.Known.Arc)
            );

        progress.AutoClear = false;
        progress.Start(action);
    }

    public void StatusText(Action<StatusContext> action)
    {
        var status = AnsiConsole.Status();
        status.Spinner = Spinner.Known.Arc;

        status.Start("My status", action);
    }
}