using System.Collections;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core.Tweening;
using Pokemon.DesktopGL.Core.Renderers;
using Pokemon.DesktopGL.Core.Managers;

namespace Pokemon.DesktopGL.Battles;

public sealed class BattleCreatureRenderer
{
    public Vector2 Offset
    {
        get => _offset;
        set => _offset = value;
    }

    public Combatant Combatant { get; set; }

    private float _opacity = 1.0f;
    private Color _color = Color.White;
    private Vector2 _offset;
    private Vector2 _size = new Vector2(160, 160) * 2.5f;

    public BattleCreatureRenderer(Combatant combatant)
    {
        Combatant = combatant;
    }

    public IEnumerator PlayAttackAnimation()
    {
        float scale = Combatant.IsPlayer ? 1f : -1f;

        yield return Tween.To((v) => _offset.X = v, () => _offset.X, 30f * scale, 0.1f, Easing.InOutQuad);
        yield return Tween.To((v) => _offset.X = v, () => _offset.X, 0f, 0.1f, Easing.InOutQuad);
    }

    public IEnumerator PlayTakeDamageAnimation()
    {
        yield return Tween.To((v) => _color = v, () => _color, Color.Red * 0.5f, 0.1f, Easing.InOutQuad);
        yield return Tween.To((v) => _color = v, () => _color, Color.White, 0.1f, Easing.InOutQuad);
    }

    public IEnumerator PlayFaintAnimation()
    {
        _opacity = 1.0f;

        CoroutineManager.Start(Tween.To((v) => _offset = v, () => _offset, Vector2.UnitY * 30, 0.3f, Easing.InOutQuad));
        CoroutineManager.Start(Tween.To((v) => _opacity = v, () => _opacity, 0.0f, 0.3f, Easing.InOutQuad));

        yield return null;
    }

    public void Draw(UIRenderer renderer, Rectangle bounds)
    {
        if (Combatant == null)
            return;

        var sprite = Combatant.IsPlayer ? Combatant.ActiveCreature.Data.BackSprite : Combatant.ActiveCreature.Data.FrontSprite;
        // var scale = Combatant.IsPlayer ? 1.0f : 0.95f;
        var offsetY = !Combatant.IsPlayer ? 60 : 0;

        int width = (int)_size.X;
        int height = width;
        int x = (int)(bounds.X + _offset.X + (bounds.Width - width) * 0.5f);
        int y = (int)(bounds.Y + _offset.Y - 140 + offsetY + (bounds.Height - height) * 0.5f);
        renderer.Draw(sprite, new Rectangle(x, y, width, height), _color * _opacity);
    }
}