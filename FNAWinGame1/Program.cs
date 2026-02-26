using System;
using FNAWinGame1;
using Microsoft.Xna.Framework.Input;

public static class Program
{
    [STAThread]
    static void Main()
    {
        SDL3.SDL.SDL_SetHint("SDL_IME_IMPLEMENTED_UI", "composition,candidates");
        using (var g = new Game1()) {
            g.Run();
        }
    }
}