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

    public bool ConfirmPrompt(string prompt)
    {
        return AnsiConsole.Prompt(
            new ConfirmationPrompt(prompt)
                .ChoicesStyle("bold dim")
                .HideDefaultValue()
        );
    }

    public T TextPrompt<T>(string prompt)
    {
        return AnsiConsole.Prompt(new TextPrompt<T>(prompt, StringComparer.OrdinalIgnoreCase));
    }

    public string SelectionPrompt(string title, string helperText, string[] choices)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(10)
                .MoreChoicesText(helperText)
                .AddChoices(choices)
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

    public void StatusText(string initialStatus, Action<StatusContext> action)
    {
        var status = AnsiConsole.Status();
        status.Spinner = Spinner.Known.Arc;

        status.Start(initialStatus, action);
    }

    public Task StatusTextAsync(string initialStatus, Func<StatusContext, Task> action)
    {
        var status = AnsiConsole.Status();
        status.Spinner = Spinner.Known.Arc;

        return status.StartAsync(initialStatus, action);
    }
}