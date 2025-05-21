namespace PokeSharp.ROM.Events;

public sealed class RomLoadFailedArgs : EventArgs
{
    public string RomPath { get; }
    public string ErrorMessage { get; }

    public RomLoadFailedArgs(string romPath, string errorMessage)
    {
        RomPath = romPath;
        ErrorMessage = errorMessage;
    }
}