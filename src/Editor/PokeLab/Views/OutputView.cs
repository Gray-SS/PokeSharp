using ImGuiNET;
using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Logging.Outputs;
using NVec2 = System.Numerics.Vector2;
using NVec4 = System.Numerics.Vector4;

namespace PokeLab.Views;

public sealed class OutputView : IEditorView
{
    private bool _autoScroll = true;
    private string _inputBuffer = string.Empty;
    private readonly MemoryLogSink _consoleBuffer;
    private readonly Logger _logger;

    private readonly string[] _levelNames;
    private readonly bool[] _levelShowFlags;
    private string _contextFilter = string.Empty;

    private bool _mustRecompute = true;
    private readonly List<LogEntry> _filteredEntries = new();

    public OutputView(MemoryLogSink consoleBuffer, Logger logger)
    {
        _consoleBuffer = consoleBuffer;
        _logger = logger;

        _levelNames = Enum.GetNames<LogLevel>();
        _levelShowFlags = new bool[_levelNames.Length];
        ShowAll();

        consoleBuffer.OnEntryCollected += OnEntryCollected;
    }

    private void OnEntryCollected(object? sender, MemoryLogSinkEventArgs e)
    {
        _mustRecompute = true;
    }

    public unsafe void DrawGui()
    {
        // Update the console buffer
        _consoleBuffer.Update();

        // if necessary recompute the filtered entries
        if (_mustRecompute)
        {
            RecomputeFilteredEntries();
            _mustRecompute = false;
        }

        if (ImGui.Begin("Output"))
        {
            if (ImGui.Button("Clear"))
            {
                _consoleBuffer.Clear();
                _mustRecompute = true;
            }

            ImGui.SameLine();
            ImGui.Checkbox("Auto-scroll", ref _autoScroll);

            ImGui.Separator();

            if (ImGui.CollapsingHeader("Filters"))
            {
                ImGui.Text("Log Levels:");
                for (int i = 0; i < _levelNames.Length; i++)
                {
                    ImGui.SameLine();
                    if (ImGui.Checkbox(_levelNames[i], ref _levelShowFlags[i]))
                        _mustRecompute = true;
                }

                if (ImGui.Button("All"))
                {
                    ShowAll();
                    _mustRecompute = true;
                }

                ImGui.SameLine();
                if (ImGui.Button("None"))
                {
                    ShowNone();
                    _mustRecompute = true;
                }

                ImGui.Text("Context Filter:");
                ImGui.SameLine();
                ImGui.PushItemWidth(200);
                if (ImGui.InputText("##ContextFilter", ref _contextFilter, 256))
                    _mustRecompute = true;

                ImGui.PopItemWidth();
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Filter logs by context (leave empty to show all)");

                ImGui.Separator();
            }

            float logHeight = ImGui.GetContentRegionAvail().Y - 100;
            ImGui.BeginChild("ScrollRegion", new NVec2(0, logHeight), 0, ImGuiWindowFlags.HorizontalScrollbar);

            int totalItems = _filteredEntries.Count;
            if (totalItems > 0)
            {
                var listClipper = new ImGuiListClipperPtr(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
                listClipper.Begin(totalItems, ImGui.GetTextLineHeightWithSpacing());

                while (listClipper.Step())
                {
                    int max = Math.Min(listClipper.DisplayEnd, totalItems);
                    for (int i = listClipper.DisplayStart; i < max; i++)
                    {
                        LogEntry entry = _filteredEntries[i];
                        var formatted = $"[{entry.TimeStamp:HH:mm:ss}] [{entry.Context}::{entry.Level}] {entry.Message}";
                        ImGui.PushStyleColor(ImGuiCol.Text, GetColorForLevel(entry.Level));
                        ImGui.TextUnformatted(formatted);
                        ImGui.PopStyleColor();
                    }
                }

                listClipper.End();
                listClipper.Destroy();
            }

            if (_autoScroll && ImGui.GetScrollY() >= ImGui.GetScrollMaxY() - 5.0f)
                ImGui.SetScrollHereY(1.0f);

            ImGui.EndChild();

            ImGui.Separator();
            ImGui.Dummy(new NVec2(0, 10));

            // Console Input
            ImGui.Text("Input");
            ImGui.SameLine();
            ImGui.PushItemWidth(-1);
            if (ImGui.InputText("##ConsoleInput", ref _inputBuffer, 1024,
                                 ImGuiInputTextFlags.EnterReturnsTrue))
            {
                if (!string.IsNullOrWhiteSpace(_inputBuffer))
                {
                    _logger.Info(_inputBuffer);
                    _inputBuffer = string.Empty;
                }
            }
            ImGui.PopItemWidth();
        }

        ImGui.End();
    }

    private void ShowAll()
    {
        Array.Fill(_levelShowFlags, true);
    }

    private void ShowNone()
    {
        Array.Fill(_levelShowFlags, false);
    }

    private void RecomputeFilteredEntries()
    {
        _filteredEntries.Clear();
        var entries = _consoleBuffer.CollectedEntries.ToArray();
        foreach (LogEntry entry in entries)
        {
            if (!_levelShowFlags[(int)entry.Level])
                continue;

            if (!string.IsNullOrWhiteSpace(_contextFilter) &&
                !entry.Context.Contains(_contextFilter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            _filteredEntries.Add(entry);
        }
    }

    private static NVec4 GetColorForLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => new NVec4(0.5f, 0.5f, 0.8f, 1f),
            LogLevel.Debug => new NVec4(0.4f, 0.8f, 1f, 1f),
            LogLevel.Info => new NVec4(0.8f, 0.8f, 0.8f, 1f),
            LogLevel.Warn => new NVec4(1.0f, 0.75f, 0.0f, 1f),
            LogLevel.Error => new NVec4(1.0f, 0.2f, 0.2f, 1f),
            LogLevel.Fatal => new NVec4(0.8f, 0f, 0f, 1f),
            _ => new NVec4(1f, 1f, 1f, 1f),
        };
    }
}