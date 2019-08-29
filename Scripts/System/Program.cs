using System;
using static Collision.Data;

namespace Collision
{
    static class MainApp
    {
        static void Main(string[] args)
        {
            events = new Event();
            game = new Game();
            events.InitializeOnce();
            game.InitializeOnce();

            game.Run();
        }
    }
}
