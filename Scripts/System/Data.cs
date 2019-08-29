using SFML.Graphics;
using SFML.Window;
using SFML.System;
using static Collision.Consts;

namespace Collision
{
    static class Data
    {
        public static RenderWindow window = new RenderWindow
        (
            winMode,
            winTitle,
            winStyle,
            winSettings
        );
        public static Event events;
        public static Game game;
    }
}