using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Pokemon.DesktopGL.Moves;

public sealed class MoveRegistry
{
    private readonly Dictionary<string, MoveData> _moves;

    public MoveRegistry()
    {
        _moves = new Dictionary<string, MoveData>();
    }

    public void Load()
    {
        LoadMoves("Content/Data/Moves/moves.json");
    }

    public MoveData GetData(string id)
        => _moves[id];

    private void LoadMoves(string path)
    {
        string text = File.ReadAllText(path);
        MoveData[] moves = JsonSerializer.Deserialize<MoveData[]>(text);

        foreach (MoveData data in moves)
            _moves[data.Id] = data;
    }
}