using ImGuiNET;
using PokeSharp.Core.Logging;
using PokeSharp.Editor.Miscellaneous;
using NVec2 = System.Numerics.Vector2;
using NVec4 = System.Numerics.Vector4;

namespace PokeSharp.Editor.Views;

//TODO: Improve this, it lags when many logs are received.
public sealed class ConsoleViewer : IGuiHook
{
    private bool _autoScroll = true;
    private string _inputBuffer = string.Empty;
    private readonly EditorConsoleBuffer _consolebuffer;
    private readonly ILogger _logger;

    private readonly string[] _levelNames;
    private readonly bool[] _levelShowFlags;

    private string _contextFilter = string.Empty;

    public ConsoleViewer(EditorConsoleBuffer consoleBuffer, ILogger logger)
    {
        _logger = logger;
        _consolebuffer = consoleBuffer;

        _levelNames = Enum.GetNames<LogLevel>();
        _levelShowFlags = new bool[_levelNames.Length];

        ShowAll();
    }

    public void DrawGui()
    {
        _consolebuffer.Update();

        if (ImGui.Begin("Output"))
        {
            if (ImGui.Button("Clear"))
                _consolebuffer.Clear();

            ImGui.SameLine();
            ImGui.Checkbox("Auto-scroll", ref _autoScroll);

            ImGui.Separator();

            if (ImGui.CollapsingHeader("Filters"))
            {
                ImGui.Text("Log Levels:");
                for (int i = 0; i < _levelNames.Length; i++)
                {
                    ImGui.SameLine();
                    ImGui.Checkbox(_levelNames[i], ref _levelShowFlags[i]);
                }

                if (ImGui.Button("All"))
                    ShowAll();

                ImGui.SameLine();

                if (ImGui.Button("None"))
                    ShowNone();

                ImGui.Text("Context Filter:");
                ImGui.SameLine();
                ImGui.PushItemWidth(200);
                ImGui.InputText("##ContextFilter", ref _contextFilter, 256);
                ImGui.PopItemWidth();
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Filter logs by context (leave empty to show all)");
                }

                ImGui.Separator();
            }

            float logHeight = ImGui.GetContentRegionAvail().Y - 100;
            ImGui.BeginChild("ScrollRegion", new NVec2(0, logHeight), 0, ImGuiWindowFlags.HorizontalScrollbar);

            var filteredEntries = GetFilteredEntries();
            foreach (var entry in filteredEntries)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, GetColorForLevel(entry.Level));
                ImGui.TextWrapped($"[{entry.TimeStamp:HH:mm:ss}] [{entry.Context}::{entry.Level}] {entry.Message}");
                ImGui.PopStyleColor();
            }

            if (_autoScroll && ImGui.GetScrollY() >= ImGui.GetScrollMaxY() - 5.0f)
                ImGui.SetScrollHereY(1.0f);

            ImGui.EndChild();

            ImGui.Separator();
            ImGui.Dummy(new NVec2(0, 10));
            ImGui.Text("Input");
            ImGui.SameLine();
            ImGui.PushItemWidth(-1);
            if (ImGui.InputText("##ConsoleInput", ref _inputBuffer, 1024, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                if (!string.IsNullOrWhiteSpace(_inputBuffer))
                {
                    _logger.Info(_inputBuffer);
                    _inputBuffer = string.Empty;
                }
            }
            ImGui.PopItemWidth();

            ImGui.End();
        }
    }

    private void ShowAll()
    {
        Array.Fill(_levelShowFlags, true);
    }

    private void ShowNone()
    {
        Array.Fill(_levelShowFlags, false);
    }

    private IEnumerable<LogEntry> GetFilteredEntries()
    {
        return _consolebuffer.Entries.Where(entry =>
        {
            if (!_levelShowFlags[(int)entry.Level])
                return false;

            if (!string.IsNullOrWhiteSpace(_contextFilter))
            {
                return entry.Context?.Contains(_contextFilter, StringComparison.OrdinalIgnoreCase) == true;
            }

            return true;
        });
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
            _ => new NVec4(1f, 1f, 1f, 1f)
        };
    }

}