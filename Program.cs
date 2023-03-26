using System;

namespace Xylem
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Source())
                game.Run();
        }
    }
}
