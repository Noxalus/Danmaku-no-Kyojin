using System;

namespace Danmaku_no_Kyojin
{
    static class Improvements
    {
        // Improvements
        public static int ShootType = 0; // 0: simple, 1: double, 2: behind, 3: triple, 4: quintuple, 5: octotuple
        public static float ShootFrequency = 1; // x1, x1.5, x2, x2.5, x3, x3.5, x4
        public static int LivesNumber = 1; // 1, 2, 3, 5
        public static bool BulletTimeEnabled = false; // true|false
        public static TimeSpan BulletTimeTimer = TimeSpan.FromSeconds(1); // 1, 3, 5, 10, 20
        public static int BulletTimeNumber = 1; // 1, 2, 3, 4, 5
        public static bool SlowModeEnabled = false; // true|false
        public static float Speed = 1; // x1, x1.5, x2
        public static float HitBoxSize = 1; // 1, 1/2, 1/3, 1/4
        public static bool HitBoxDisplay = false; // true|false
        public static double BulletTimeDivisor = 0.5; // 1/2, 1/3, 1/4, 1/8
        public static TimeSpan TimerInitialTime = TimeSpan.FromSeconds(15); // 15s, 30s, 1min, 3min, 5min
        public static TimeSpan TimerExtraTime = TimeSpan.FromSeconds(15); // 15s, 45s, 1min, 1min15s
        public static TimeSpan InvicibleTime = TimeSpan.FromSeconds(1); // 1s, 2s, 3s, 4s, 5s
        public static float ScoreToMoney = 1; // x1, x1.25, x1.5, x2

        // Future features ?
        public static bool ShieldEnabled = false; // true|false
        public static int ShieldNumber = 1; // 1, 2, 3, 4, 5
        public static TimeSpan ShieldDuration = TimeSpan.FromSeconds(1); // 1s, 2s, 3s, 4s, 5s

        // Player data
        public static int Money = 0; // Money to buy improvements
    }
}
