using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Danmaku_no_Kyojin.Utils;

namespace Danmaku_no_Kyojin
{
    static class Improvements
    {
        /** Improvements **/
        public enum ShootTypeEnum { Single, Twofold, Behind, Threefold, Fivefold, Eightfold }

        public static readonly List<KeyValuePair<ShootTypeEnum, int>> ShootTypeData = new List<KeyValuePair<ShootTypeEnum, int>>()
            {
                new KeyValuePair<ShootTypeEnum, int>(ShootTypeEnum.Single, 0),
                new KeyValuePair<ShootTypeEnum, int>(ShootTypeEnum.Twofold, 500),
                new KeyValuePair<ShootTypeEnum, int>(ShootTypeEnum.Behind, 1000),
                new KeyValuePair<ShootTypeEnum, int>(ShootTypeEnum.Threefold, 2000),
                new KeyValuePair<ShootTypeEnum, int>(ShootTypeEnum.Fivefold, 5000),
                new KeyValuePair<ShootTypeEnum, int>(ShootTypeEnum.Eightfold, 10000),
            };

        public static List<KeyValuePair<double, int>> ShootFrequencyData = new List<KeyValuePair<double, int>>
            {
                new KeyValuePair<double, int>(10, 0), 
                new KeyValuePair<double, int>(7, 1000), 
                new KeyValuePair<double, int>(5, 5000), 
                new KeyValuePair<double, int>(4, 10000), 
                new KeyValuePair<double, int>(3, 20000), 
                new KeyValuePair<double, int>(2, 50000), 
                new KeyValuePair<double, int>(1, 200000)
            };

        public static readonly List<KeyValuePair<int, int>> ShootPowerData = new List<KeyValuePair<int, int>> 

            {
                new KeyValuePair<int, int>(1, 0),
                new KeyValuePair<int, int>(2, 1000),
                new KeyValuePair<int, int>(3, 10000),
                new KeyValuePair<int, int>(4, 30000),
                new KeyValuePair<int, int>(5, 50000)
            };

        public static readonly List<KeyValuePair<int, int>> LivesNumberData = new List<KeyValuePair<int, int>> 
            {
                new KeyValuePair<int, int>(1, 0),
                new KeyValuePair<int, int>(2, 500),
                new KeyValuePair<int, int>(3, 25000),
                new KeyValuePair<int, int>(5, 75000),
                new KeyValuePair<int, int>(10, 2000000)
            };

        public static readonly List<KeyValuePair<TimeSpan, int>> BulletTimeTimerData = new List<KeyValuePair<TimeSpan, int>>()

            {
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(1), 0),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(3), 5000),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(5), 10000), 
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(10), 50000),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(20), 2000000)
            };

        public static readonly List<KeyValuePair<int, int>> BulletTimeNumberData = new List<KeyValuePair<int, int>> 

            {
                new KeyValuePair<int, int>(1, 0),
                new KeyValuePair<int, int>(2, 2500),
                new KeyValuePair<int, int>(3, 8000),
                new KeyValuePair<int, int>(4, 15000),
                new KeyValuePair<int, int>(5, 50000)
            };

        public static readonly List<KeyValuePair<double, int>> BulletTimeDivisorData = new List<KeyValuePair<double, int>>()

            {
                new KeyValuePair<double, int>(2, 0),
                new KeyValuePair<double, int>(3, 10000),
                new KeyValuePair<double, int>(4, 50000),
                new KeyValuePair<double, int>(8, 1000000)
            };

        public static readonly List<KeyValuePair<double, int>> SpeedData = new List<KeyValuePair<double, int>>()

            {
                new KeyValuePair<double, int>(1, 0),
                new KeyValuePair<double, int>(1.5, 2500),
                new KeyValuePair<double, int>(2, 10000)
            };

        public static readonly List<KeyValuePair<double, int>> HitBoxSizeData = new List<KeyValuePair<double, int>>()

            {
                new KeyValuePair<double, int>(1, 0),
                new KeyValuePair<double, int>(0.5, 3000),
                new KeyValuePair<double, int>(0.3, 7500),
                new KeyValuePair<double, int>(0.25, 15000)
            };

        public static readonly List<KeyValuePair<TimeSpan, int>> TimerInitialTimeData = new List<KeyValuePair<TimeSpan, int>>()

            {
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(15), 0),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(30), 100),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(60), 2000), 
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(180), 10000),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(300), 50000)
            };

        public static readonly List<KeyValuePair<TimeSpan, int>> TimerExtraTimeData = new List<KeyValuePair<TimeSpan, int>>()

            {
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(15), 0),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(30), 1000),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(45), 5000), 
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(60), 10000),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(75), 50000),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(90), 100000)
            };

        public static readonly List<KeyValuePair<TimeSpan, int>> InvicibleTimeData = new List<KeyValuePair<TimeSpan, int>>()

            {
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(1), 0),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(2), 500),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(3), 2500), 
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(4), 5000),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(5), 10000)
            };

        public static readonly List<KeyValuePair<double, int>> ScoreToMoneyFactorData = new List<KeyValuePair<double, int>>()

            {
                new KeyValuePair<double, int>(1, 0),
                new KeyValuePair<double, int>(1.25, 150),
                new KeyValuePair<double, int>(1.5, 500),
                new KeyValuePair<double, int>(1.75, 1000),
                new KeyValuePair<double, int>(2, 5000)
            };

        public static readonly List<KeyValuePair<int, int>> ScoreByHitData = new List<KeyValuePair<int, int>> 

            {
                new KeyValuePair<int, int>(1, 0),
                new KeyValuePair<int, int>(2, 1000),
                new KeyValuePair<int, int>(3, 2500),
                new KeyValuePair<int, int>(5, 5000),
                new KeyValuePair<int, int>(10, 10000),
                new KeyValuePair<int, int>(20, 25000)
            };

        public static readonly List<KeyValuePair<int, int>> ScoreByEnemyData = new List<KeyValuePair<int, int>> 

            {
                new KeyValuePair<int, int>(50, 0),
                new KeyValuePair<int, int>(75, 1000),
                new KeyValuePair<int, int>(100, 2500),
                new KeyValuePair<int, int>(125, 5000),
                new KeyValuePair<int, int>(150, 10000)
            };

        public const int SlowModePrice = 50;
        public const int BulletTimePrice = 5000;
    }
}
