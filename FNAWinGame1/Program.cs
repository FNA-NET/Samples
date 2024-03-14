using System;
using FNAWinGame1;
using Microsoft.Xna.Framework.Input;

public static class Program
{
    [STAThread]
    static void Main()
    {
        TextInputEXT.ShowOSImeWindow = false;
        using (var g = new Game1()) {
            g.Run();
        }
    }
}