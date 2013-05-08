using Microsoft.Xna.Framework;
using System;

namespace Danmaku_no_Kyojin
{
    public static class Config
    {
        // Debug
        public const bool DisplayCollisionBoxes = false;

        // Screen
        public const bool FullScreen = false;
        public static readonly Point Resolution = new Point(800, 600);

        // Bullet Time
        public const float DesiredTimeModifier = 5f;

        public static readonly TimeSpan PlayerBulletFrequence = new TimeSpan(0, 0, 0, 0, 50);
    }
}
