namespace PokeSharp.Core.Windowing.Events;

public sealed class FileDroppedArgs : EventArgs
{
    public string[] Files { get; }

    public FileDroppedArgs(string[] files)
    {
        Files = files;
    }
}