using System;

namespace FNAImGuiDemo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using(var g = new Game1())
                g.Run();
        }
    }
}
