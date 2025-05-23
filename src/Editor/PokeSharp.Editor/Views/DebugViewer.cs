using ImGuiNET;
using PokeSharp.Core.Logging;
using PokeSharp.Editor.UI;

namespace PokeSharp.Editor.Views;

public sealed class DebugViewer : IGuiHook, ILogOutput
{
    string ILogOutput.Name => "Debug";

    private readonly List<LogEntry> _entries;

    public DebugViewer()
    {
        _entries = new List<LogEntry>();
    }

    public void DrawGui()
    {
        if (ImGui.Begin("Debug viewer"))
        {
            

            ImGui.End();
        }
    }

    public void Log(in LogEntry entry)
    {
        _entries.Add(entry);
    }
}
