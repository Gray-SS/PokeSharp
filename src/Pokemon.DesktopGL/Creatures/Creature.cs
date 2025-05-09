using System;
using System.Collections.Generic;
using System.Linq;
using Pokemon.DesktopGL.Moves;

namespace Pokemon.DesktopGL.Creatures;

public sealed class Creature
{
    public CreatureData Data { get; }
    public int Level { get; }
    public int Experience { get; }
    public int HP { get; private set; }

    public int MaxHP => Data.BaseHP + Level * 2;
    public int Attack => Data.BaseAttack + Level;
    public int Defense => Data.BaseDefense + Level;
    public int Speed => Data.BaseSpeed + Level;
    public int SpAtk => Data.BaseSpAtk + Level;
    public int SpDef => Data.BaseSpDef + Level;

    public IReadOnlyList<MoveData> Moves => _moves;

    public bool IsFainted => HP <= 0;

    private readonly List<MoveData> _moves;

    public Creature(CreatureData data, int level)
    {
        Data = data;
        Level = level;
        HP = MaxHP;
        Experience = data.BaseEXP;

        _moves = new List<MoveData>(4);
    }

    public void AddMove(MoveData data)
    {
        EnsureValidMove(data);
        if (_moves.Count >= 4)
            throw new InvalidOperationException("Couldn't add a move to this creature since it already reached the maximum move available (4)");

        _moves.Add(data);
    }

    public void RemoveMove(MoveData data)
    {
        _moves.Remove(data);
    }

    public void ReplaceMove(int index, MoveData data)
    {
        EnsureValidMove(data);
        if (index < 0 || index >= _moves.Count)
            throw new InvalidOperationException($"Couldn't replace the move at index {index}. The index is out of range.");

        _moves[index] = data;
    }

    private void EnsureValidMove(MoveData data)
    {
        // TODO: Implement this function
    }

    public void TakeDamage(int damage)
    {
        HP = Math.Max(0, HP - damage);
    }
}