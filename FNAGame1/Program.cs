using FNAGame1;

SDL3.SDL.SDL_SetHint("SDL_IME_IMPLEMENTED_UI", "composition");

using (var g = new Game1())
{
    g.Run();
}
