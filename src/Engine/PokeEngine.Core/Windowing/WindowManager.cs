using Microsoft.Xna.Framework;

namespace PokeEngine.Core.Windowing;

public sealed class WindowManager : IWindowManager
{
    public string Title
    {
        get => _nativeWindow.Title;
        set => _nativeWindow.Title = value;
    }

    public Vector2 Position
    {
        get => _nativeWindow.Position.ToVector2();
        set => _nativeWindow.Position = value.ToPoint();
    }

    public Rectangle Bounds
    {
        get => _nativeWindow.ClientBounds;
    }

    public event EventHandler<WindowSizeChangedArgs>? SizeChanged;
    public event EventHandler<FileDroppedArgs>? FileDropped;

    private readonly GameWindow _nativeWindow;

    public WindowManager(GameWindow nativeWindow)
    {
        _nativeWindow = nativeWindow;
        _nativeWindow.ClientSizeChanged += OnClientSizeChanged;

        _nativeWindow.FileDrop += OnFileDrop;
    }

    private void OnFileDrop(object? sender, FileDropEventArgs e)
    {
        FileDropped?.Invoke(this, new FileDroppedArgs(e.Files));
    }

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        Vector2 newSize = _nativeWindow.ClientBounds.Size.ToVector2();
        SizeChanged?.Invoke(this, new WindowSizeChangedArgs(newSize));
    }
}