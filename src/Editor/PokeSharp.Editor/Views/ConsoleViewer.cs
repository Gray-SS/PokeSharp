using ImGuiNET;
using PokeSharp.Core.Logging;
using PokeSharp.Editor.Miscellaneous;
using PokeSharp.Editor.UI;

using NVec2 = System.Numerics.Vector2;
using NVec4 = System.Numerics.Vector4;

namespace PokeSharp.Editor.Views;

public sealed class ConsoleViewer : IGuiHook
{
    private bool _autoScroll = true;
    private string _inputBuffer = string.Empty;
    private readonly EditorConsoleBuffer _consolebuffer;
    private readonly ILogger _logger;

    public ConsoleViewer(EditorConsoleBuffer consoleBuffer, ILogger logger)
    {
        _logger = logger;
        _consolebuffer = consoleBuffer;
    }

    public void DrawGui()
    {
        if (ImGui.Begin("Console"))
        {
            if (ImGui.Button("Clear"))
                _consolebuffer.Clear();

            ImGui.SameLine();
            ImGui.Checkbox("Auto-scroll", ref _autoScroll);

            ImGui.Separator();

            float logHeight = ImGui.GetContentRegionAvail().Y - 100;
            ImGui.BeginChild("ScrollRegion", new NVec2(0, logHeight), 0, ImGuiWindowFlags.HorizontalScrollbar);
            foreach (var entry in _consolebuffer.Entries)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, GetColorForLevel(entry.Level));
                ImGui.TextUnformatted($"[{entry.TimeStamp:HH:mm:ss}] [{entry.Context}::{entry.Level}] {entry.Message}");
                ImGui.PopStyleColor();
            }

            if (_autoScroll && ImGui.GetScrollY() >= ImGui.GetScrollMaxY() - 5.0f)
                ImGui.SetScrollHereY(1.0f);

            ImGui.EndChild();

            // Input area
            ImGui.Separator();

            ImGui.Dummy(new NVec2(0, 10));

            ImGui.Text("Input");
            ImGui.SameLine();

            ImGui.PushItemWidth(-1); // Full width input
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

    private static NVec4 GetColorForLevel(LogLevel level)
    {
        return level switch
        {
            // LogLevel.Trace => new NVec4(0.6f, 0.6f, 0.6f, 1f),
            LogLevel.Debug => new NVec4(0.6f, 0.6f, 0.6f, 1f),
            LogLevel.Info => new NVec4(0.9f, 0.9f, 0.9f, 1f),
            LogLevel.Warn => new NVec4(1.0f, 1.0f, 0.3f, 1f),
            LogLevel.Error => new NVec4(1.0f, 0.3f, 0.3f, 1f),
            LogLevel.Fatal => new NVec4(1.0f, 0.1f, 0.1f, 1f),
            _ => new NVec4(1f, 1f, 1f, 1f),
        };
    }
}
