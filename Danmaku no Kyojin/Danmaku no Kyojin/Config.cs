using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Danmaku_no_Kyojin
{
    public static class Config
    {
        public static bool Debug = false;
        public static bool Cheat = false;

        public enum Controller
        {
            Keyboard,
            GamePad
        };

        // Debug
        public static bool DisplayCollisionBoxes = false;
        public const bool FpsCapping = true;

        // Screen
        public static bool FullScreen = false;
        public static readonly Point Resolution = new Point(1024, 768);

        // Bullet Time
        public const float DesiredTimeModifier = 2f;

        // Player
        public static int PlayersNumber = 1;

        public static readonly Dictionary<string, Keys> PlayerKeyboardInputs = new Dictionary<string, Keys>()
        {
            {"Up", Keys.Z },
            {"Right", Keys.D },
            {"Down", Keys.S },
            {"Left", Keys.Q },
            {"Slow", Keys.LeftShift },
        };

        public static readonly Buttons[] PlayerGamepadInput = new Buttons[]
        {
            Buttons.LeftTrigger, Buttons.RightTrigger
        };

        public static Controller[] PlayersController = new Controller[]
            {
                Controller.Keyboard,
                Controller.GamePad
            };

        public static readonly TimeSpan PlayerInvicibleTimer = new TimeSpan(0, 0, 3);
        public static readonly TimeSpan DefaultBulletTimeTimer = new TimeSpan(0, 0, 1);
        public static readonly TimeSpan PlayerShootFrequency = new TimeSpan(0, 0, 0, 0, 33);
        public const float PlayerMaxVelocity = 400f;
        public const float PlayerMaxSlowVelocity = 125f;
        public const float PlayerBulletVelocity = 800f;
        public const int PlayerLives = 5;

        // Game
        public static readonly Point GameArea = new Point(1000, 1000);

        public static float GameDifficulty = 0.5f; 
        public static float GameDifficultyDelegate()
        {
            return GameDifficulty;
        }

        public static TimeSpan BossInitialTimer = TimeSpan.FromSeconds(5);

        // Audio
        public static int SoundVolume = 80;
        public static int MusicVolume = 100;
    }
}
