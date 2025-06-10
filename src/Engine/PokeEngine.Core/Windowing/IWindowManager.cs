using Microsoft.Xna.Framework;

namespace PokeEngine.Core.Windowing;

public sealed class FileDroppedArgs : EventArgs
{
    public string[] Files { get; }

    public FileDroppedArgs(string[] files)
    {
        Files = files;
    }
}

public sealed class WindowSizeChangedArgs : EventArgs
{
    public Vector2 Size { get; }

    public WindowSizeChangedArgs(Vector2 size)
    {
        Size = size;
    }
}

public interface IWindowManager
{
    string Title { get; set; }
    Vector2 Position { get; set; }
    Rectangle Bounds { get; }

    event EventHandler<FileDroppedArgs>? FileDropped;
    event EventHandler<WindowSizeChangedArgs>? SizeChanged;
}