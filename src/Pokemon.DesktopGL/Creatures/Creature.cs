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
    public Stats BaseStats => Data.BaseStats;

    public int MaxHP => CalculateStat(BaseStats.HP, IV.HP, EV.HP, Level, true);
    public int Attack => CalculateStat(BaseStats.Attack, IV.Attack, EV.Attack, Level);
    public int Defense => CalculateStat(BaseStats.Defense, IV.Defense, EV.Defense, Level);
    public int Speed => CalculateStat(BaseStats.Speed, IV.Speed, EV.Speed, Level);
    public int SpAtk => CalculateStat(BaseStats.SpAtk, IV.SpAtk, EV.SpAtk, Level);
    public int SpDef => CalculateStat(BaseStats.SpDef, IV.SpDef, EV.SpDef, Level);

    public Stats IV { get; }
    public Stats EV { get; }

    public IReadOnlyList<MoveData> Moves => _moves;

    public bool IsFainted => HP <= 0;

    private readonly List<MoveData> _moves;

    public Creature(CreatureData data, int level) : this(data, level, Stats.Zero, Stats.GenerateRandom())
    {
    }

    public Creature(CreatureData data, int level, Stats ev, Stats iv)
    {
        EV = ev;
        IV = iv;
        Data = data;
        Level = level;
        HP = MaxHP;
        Experience = data.BaseEXP;

        _moves = new List<MoveData>(4);
    }

    public void AddMove(MoveData data)
    {
        EnsureValidMove(data);
        if (_moves.Contains(data))
            return;

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

    private int CalculateStat(int baseStat, int iv, int ev, int level, bool isHP = false)
    {
        int stat = ((2 * baseStat + iv + ev / 4) * level) / 100;
        if (isHP)
            return stat + level + 10;
        return stat + 5;
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