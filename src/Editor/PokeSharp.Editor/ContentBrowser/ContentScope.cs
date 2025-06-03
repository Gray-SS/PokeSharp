namespace PokeSharp.Editor.ContentBrowser;

[Flags]
public enum ContentScope : byte
{
    None = 0,
    CurrentDirectory = 1 << 0,
    Volumes = 1 << 1,
    All = CurrentDirectory | Volumes
}