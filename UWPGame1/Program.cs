using SDL3;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UWPGame1
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static unsafe class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            realArgs = args;
            SDL.SDL_SetHint("SDL_WINRT_HANDLE_BACK_BUTTON", "1");

            SDL.SDL_RunApp(0, IntPtr.Zero, FakeMain, IntPtr.Zero);
        }

        static string[] realArgs;

        static int FakeMain(int argc, IntPtr argv)
        {
            RealMain(realArgs);
            return 0;
        }

        static void RealMain(string[] args)
        {
            using (var g = new Game1())
            {
                g.Run();
            }
        }
    }
}
