using System;

namespace Pokemon.DesktopGL.Creatures;

public sealed class Creature
{
    public CreatureData Data { get; }
    public int Level { get; }
    public int HP { get; private set; }

    public int MaxHP => Data.BaseHP + Level * 2;
    public int Attack => Data.BaseAttack + Level;
    public int Defense => Data.BaseDefense + Level;
    public bool IsFainted => HP <= 0;

    public Creature(CreatureData data, int level)
    {
        Data = data;
        Level = level;
        HP = MaxHP;
    }

    public void TakeDamage(int damage)
    {
        HP = Math.Max(0, HP - damage);
    }
}