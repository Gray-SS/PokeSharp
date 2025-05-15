using Microsoft.Xna.Framework;
using PokeSharp.Engine.Renderers;
using System;

namespace Pokemon.DesktopGL.Dialogues;

public class DialogueBoxRenderer
{
    private readonly DialogueManager _dialogueManager;

    private readonly Vector2 _textPadding = new Vector2(24, 12) * 1.4f;
    private readonly Vector2 _boxSize = new Vector2(512, 96) * 1.2f;

    private float _animationTimer = 0f;
    private const float AnimationDuration = 0.5f;

    private enum AnimationState { Hidden, SlidingIn, Visible, SlidingOut }
    private AnimationState _state = AnimationState.Hidden;

    public DialogueBoxRenderer(DialogueManager manager)
    {
        _dialogueManager = manager;

        _dialogueManager.DialogueStarted += () => {
            _animationTimer = 0f;
            _state = AnimationState.SlidingIn;
        };

        _dialogueManager.DialogueEnded += () => {
            _animationTimer = 0f;
            _state = AnimationState.SlidingOut;
        };
    }

    public void Update(float dt)
    {
        if (_state == AnimationState.SlidingIn || _state == AnimationState.SlidingOut)
        {
            _animationTimer += dt;
            if (_animationTimer >= AnimationDuration)
            {
                _animationTimer = AnimationDuration;

                _state = _state == AnimationState.SlidingIn
                    ? AnimationState.Visible
                    : AnimationState.Hidden;
            }
        }
    }

    public void Draw(UIRenderer renderer)
    {
        if (_state == AnimationState.Hidden)
            return;

        var assetsManager = PokemonGame.Instance.AssetsManager;
        var windowManager = PokemonGame.Instance.WindowManager;

        var boxSprite = assetsManager.Sprite_Dialogue_Overlay;

        float progress = _animationTimer / AnimationDuration;
        float eased = EaseInOutSine(progress);

        float baseY = (windowManager.WindowHeight - _boxSize.Y) * 0.95f;
        float hiddenY = windowManager.WindowHeight + 20f;

        float y = _state switch
        {
            AnimationState.SlidingIn => Lerp(hiddenY, baseY, eased),
            AnimationState.SlidingOut => Lerp(baseY, hiddenY, eased),
            _ => baseY
        };

        var bounds = new Rectangle(
            (int)((windowManager.WindowWidth - _boxSize.X) * 0.5f),
            (int)y,
            (int)_boxSize.X,
            (int)_boxSize.Y
        );

        renderer.Draw(boxSprite, bounds, Color.White);

        if (_state != AnimationState.SlidingOut || progress < 0.8f)
        {
            var font = assetsManager.Font_PowerGreen.GetFont(20);
            Vector2 textPos = new Vector2(bounds.Left, bounds.Top) + _textPadding;
            renderer.DrawWrappedText(font, _dialogueManager.DisplayedText, textPos, Color.Black, bounds.Width - 2 * _textPadding.X);
        }
    }

    private static float Lerp(float from, float to, float t) => from + (to - from) * t;
    private static float EaseInOutSine(float t) => -(float)Math.Cos(Math.PI * t) * 0.5f + 0.5f;
}
