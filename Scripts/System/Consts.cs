using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace Collision
{
    static class Consts
    {
        public const int winFrameLimit = 60;
        public const uint winWdt = 800;
        public const uint winHgt = 600;
        public static Color winBackColor = new Color(32, 32, 32);
        public static VideoMode winMode = new VideoMode(winWdt, winHgt);
        public static string winTitle = "교차 판정";
        public static Styles winStyle = Styles.Titlebar;
        public static ContextSettings winSettings = new ContextSettings(1, 1, 8);
    }
}