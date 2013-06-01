using System;
using System.Collections.Generic;

namespace Danmaku_no_Kyojin
{
    static class Improvements
    {
        // Improvements's data: Dictionary<Value, Price>
        public enum ShootTypeEnum
        {
            Single,
            Twofold,
            Behind,
            Threefold,
            Fivefold,
            Eightfold
        }

        public static Dictionary<ShootTypeEnum, int> DataShootType = new Dictionary<ShootTypeEnum, int>()
            {
                { ShootTypeEnum.Single, 0 },
                { ShootTypeEnum.Twofold, 500 },
                { ShootTypeEnum.Behind, 1000 },
                { ShootTypeEnum.Threefold, 2000 },
                { ShootTypeEnum.Fivefold, 5000 },
                { ShootTypeEnum.Single, 10000 },
            };

        public static Dictionary<double, int> DataShootFrequency = new Dictionary<double, int>()
            {
                {1, 0},
                {1.5, 150},
                {2, 500},
                {2.5, 1000},
                {3, 2000},
                {3.5, 5000},
                {4, 10000}
            };

        public static Dictionary<int, int> DataLivesNumber = new Dictionary<int, int>
            {
                {1, 0},
                {2, 1},
                {3, 2},
                {5, 3}
            };

        public static Dictionary<TimeSpan, int> DataBulletTimeTimer = new Dictionary<TimeSpan, int>()
            {
                {TimeSpan.FromSeconds(1), 0},
                {TimeSpan.FromSeconds(3), 1},
                {TimeSpan.FromSeconds(5), 2}, 
                {TimeSpan.FromSeconds(10), 3},
                {TimeSpan.FromSeconds(20), 4}
            };

        public static Dictionary<int, int> DataBulletTimeNumber = new Dictionary<int, int>
            {
                {1, 0},
                {2, 1},
                {3, 2},
                {4, 3},
                {5, 4}
            };

        public static Dictionary<double, int> DataShootDivisor = new Dictionary<double, int>()
            {
                {1/2, 0},
                {1/3, 150},
                {1/4, 500},
                {1/8, 1000}
            };

        public static Dictionary<double, int> DataSpeed = new Dictionary<double, int>()
            {
                {1, 0},
                {1.5, 150},
                {2, 500}
            };

        public static Dictionary<double, int> DataHitBoxSize = new Dictionary<double, int>()
            {
                {1, 0},
                {1/2, 150},
                {1/3, 500},
                {1/4, 1000}
            };

        public static Dictionary<TimeSpan, int> DataTimerInitialTime = new Dictionary<TimeSpan, int>()
            {
                {TimeSpan.FromSeconds(15), 0},
                {TimeSpan.FromSeconds(30), 1},
                {TimeSpan.FromSeconds(60), 2}, 
                {TimeSpan.FromSeconds(180), 3},
                {TimeSpan.FromSeconds(300), 4}
            };

        public static Dictionary<TimeSpan, int> DataTimerExtraTime = new Dictionary<TimeSpan, int>()
            {
                {TimeSpan.FromSeconds(15), 0},
                {TimeSpan.FromSeconds(30), 1},
                {TimeSpan.FromSeconds(45), 2}, 
                {TimeSpan.FromSeconds(60), 3},
                {TimeSpan.FromSeconds(75), 4},
                {TimeSpan.FromSeconds(90), 5}
            };

        public static Dictionary<TimeSpan, int> DataInvicibleTime = new Dictionary<TimeSpan, int>()
            {
                {TimeSpan.FromSeconds(1), 0},
                {TimeSpan.FromSeconds(2), 1},
                {TimeSpan.FromSeconds(3), 2}, 
                {TimeSpan.FromSeconds(4), 3},
                {TimeSpan.FromSeconds(5), 4}
            };

        public static Dictionary<double, int> DataScoreToMoneyFactor = new Dictionary<double, int>()
            {
                {1, 0},
                {1.25, 150},
                {1.5, 500},
                {1.75, 1000},
                {2, 5000}
            };

        public static Dictionary<int, int> DataScoreByHit = new Dictionary<int, int>
            {
                {1, 0},
                {2, 1},
                {3, 2},
                {5, 3},
                {10, 4},
                {20, 5}
            };

        public static Dictionary<int, int> DataScoreByEnemy = new Dictionary<int, int>
            {
                {50, 0},
                {75, 1},
                {100, 2},
                {125, 3},
                {150, 4}
            };

        // Improvements
        public static bool BulletTimeEnabled = false; // true|false
        public static bool SlowModeEnabled = false; // true|false
        public static bool HitBoxDisplay = false; // true|false

        public static ShootTypeEnum ShootType = ShootTypeEnum.Single; // 0: simple, 1: double, 2: behind, 3: triple, 4: quintuple, 5: octotuple
        public static double ShootFrequency = 1; // x1, x1.5, x2, x2.5, x3, x3.5, x4
        public static int LivesNumber = 1; // 1, 2, 3, 5
        public static TimeSpan BulletTimeTimer = TimeSpan.FromSeconds(1); // 1, 3, 5, 10, 20
        public static int BulletTimeNumber = 1; // 1, 2, 3, 4, 5
        public static double BulletTimeDivisor = 0.5; // 1/2, 1/3, 1/4, 1/8
        public static double Speed = 1; // x1, x1.5, x2
        public static double HitBoxSize = 1; // 1, 1/2, 1/3, 1/4
        public static TimeSpan TimerInitialTime = TimeSpan.FromSeconds(15); // 15s, 30s, 1min, 3min, 5min
        public static TimeSpan TimerExtraTime = TimeSpan.FromSeconds(15); // 15s, 30s, 45s, 1min, 1min15s
        public static TimeSpan InvicibleTime = TimeSpan.FromSeconds(1); // 1s, 2s, 3s, 4s, 5s
        public static double ScoreToMoneyMultiplicator = 1; // x1, x1.25, x1.5, x2
        public static int ScoreByHit = 1; // 1, 2, 3, 5, 10, 20
        public static int ScoreByEnemy = 50; // 50, 75, 100, 125, 150

        // Future features ?
        public static bool ShieldEnabled = false; // true|false
        public static int ShieldNumber = 1; // 1, 2, 3, 4, 5
        public static TimeSpan ShieldDuration = TimeSpan.FromSeconds(1); // 1s, 2s, 3s, 4s, 5s

        // Player data
        public static int Money = 0; // Money to buy improvements
    }
}
