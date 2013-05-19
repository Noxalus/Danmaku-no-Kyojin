using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Danmaku_no_Kyojin
{
    public static class Config
    {
        // Debug
        public static bool DisplayCollisionBoxes = false;
        public const bool FpsCapping = true;

        // Screen
        public const bool FullScreen = false;
        public static readonly Point Resolution = new Point(800, 600);

        // Bullet Time
        public const float DesiredTimeModifier = 5f;

        // Player
        public const int PlayersNumber = 1;

        public static readonly Keys[] PlayerKeyboardInput = new Keys[]
        {
            Keys.Z, Keys.D, Keys.S, Keys.Q, Keys.LeftShift
        };

        public static readonly Buttons[] PlayerGamepadInput = new Buttons[]
        {
            Buttons.LeftTrigger, Buttons.RightTrigger
        };

        public static readonly TimeSpan PlayerInvicibleTimer = new TimeSpan(0, 0, 0, 3);
        public static readonly TimeSpan PlayerBulletFrequence = new TimeSpan(0, 0, 0, 0, 50);
        public const float PlayerMaxVelocity = 400f;
        public const float PlayerMaxSlowVelocity = 125f;
        public const float PlayerBulletVelocity = 600f;
        public const int PlayerLives = 10;

        // Game
        public static readonly Point GameArea = new Point(1000, 1000);
    }
}
