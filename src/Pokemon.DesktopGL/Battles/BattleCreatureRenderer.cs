using System.Collections;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core.Tweening;
using Pokemon.DesktopGL.Core.Renderers;

namespace Pokemon.DesktopGL.Battles;

public sealed class BattleCreatureRenderer
{
    public Vector2 Offset
    {
        get => _offset;
        set => _offset = value;
    }

    public Combatant Combatant { get; set; }

    private Vector2 _offset;
    private Vector2 _size = new Vector2(160, 160) * 2.5f;

    public BattleCreatureRenderer(Combatant combatant)
    {
        Combatant = combatant;
    }

    public IEnumerator PlayAttackAnimation()
    {
        var tween = Tween.To((v) => _offset.X = v, () => _offset.X, 10f, 0.1f, Easing.InOutQuad);
        yield return tween.StartRoutine();
    }

    public void Draw(UIRenderer renderer, Rectangle bounds)
    {
        if (Combatant == null)
            return;

        var sprite = Combatant.IsPlayer ? Combatant.ActiveCreature.Data.BackSprite : Combatant.ActiveCreature.Data.FrontSprite;
        var scale = Combatant.IsPlayer ? 1.0f : 0.95f;

        int width = (int)(_size.X * scale);
        int height = width;
        int x = (int)(bounds.X + (bounds.Width - width) * 0.5f);
        int y = (int)(bounds.Y - 140 * scale + (bounds.Height - height) * 0.5f);
        renderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);

        // else
        // {
        //     var sprite = Combatant.ActiveCreature.Data.FrontSprite;

        //     int width = (int)(bounds.Width * 0.6f);
        //     int height = width;
        //     int x = (int)(bounds.X width * 0.5f);
        //     int y = (int)(bounds.Y - height * 0.5f - 80);
        //     renderer.Draw(sprite, new Rectangle(x, y, width, height), Color.White);
        // }
    }
}