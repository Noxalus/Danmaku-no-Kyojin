using System;
using System.Collections.Generic;
using System.Collections.Specialized;

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

        /*
        public static List<KeyValuePair<double, int>> ShootFrequencyData = new List<KeyValuePair<double, int>>
            {
                new KeyValuePair<double, int>(1, 0), 
                new KeyValuePair<double, int>(1.5, 500), 
                new KeyValuePair<double, int>(2, 1000), 
                new KeyValuePair<double, int>(2.5, 2000), 
                new KeyValuePair<double, int>(3, 5000), 
                new KeyValuePair<double, int>(3.5, 10000), 
                new KeyValuePair<double, int>(4, 20000)
            };
        */

        public static List<KeyValuePair<double, int>> ShootFrequencyData = new List<KeyValuePair<double, int>>
            {
                new KeyValuePair<double, int>(10, 0), 
                new KeyValuePair<double, int>(7, 200), 
                new KeyValuePair<double, int>(5, 500), 
                new KeyValuePair<double, int>(4, 1000), 
                new KeyValuePair<double, int>(3, 2000), 
                new KeyValuePair<double, int>(2, 5000), 
                new KeyValuePair<double, int>(1, 20000)
            };

        public static readonly List<KeyValuePair<int, int>> ShootPowerData = new List<KeyValuePair<int, int>> 

            {
                new KeyValuePair<int, int>(1, 0),
                new KeyValuePair<int, int>(2, 1),
                new KeyValuePair<int, int>(3, 2),
                new KeyValuePair<int, int>(4, 3),
                new KeyValuePair<int, int>(5, 4)
            };

        public static readonly List<KeyValuePair<int, int>> LivesNumberData = new List<KeyValuePair<int, int>> 

            {
                new KeyValuePair<int, int>(1, 0),
                new KeyValuePair<int, int>(2, 1),
                new KeyValuePair<int, int>(3, 2),
                new KeyValuePair<int, int>(5, 3)
            };

        public static readonly List<KeyValuePair<TimeSpan, int>> BulletTimeTimerData = new List<KeyValuePair<TimeSpan, int>> ()

            {
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(1), 0),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(3), 1),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(5), 2), 
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(10), 3),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(20), 4)
            };

        public static readonly List<KeyValuePair<int, int>> BulletTimeNumberData = new List<KeyValuePair<int, int>> 

            {
                new KeyValuePair<int, int>(1, 0),
                new KeyValuePair<int, int>(2, 1),
                new KeyValuePair<int, int>(3, 2),
                new KeyValuePair<int, int>(4, 3),
                new KeyValuePair<int, int>(5, 4)
            };

        public static readonly List<KeyValuePair<double, int>> ShootDivisorData = new List<KeyValuePair<double, int>> ()

            {
                new KeyValuePair<double, int>(0.5, 0),
                new KeyValuePair<double, int>(0.3, 150),
                new KeyValuePair<double, int>(0.25, 500),
                new KeyValuePair<double, int>(0.125, 1000)
            };

        public static readonly List<KeyValuePair<double, int>> SpeedData = new List<KeyValuePair<double, int>> ()

            {
                new KeyValuePair<double, int>(1, 0),
                new KeyValuePair<double, int>(1.5, 150),
                new KeyValuePair<double, int>(2, 500)
            };

        public static readonly List<KeyValuePair<double, int>> HitBoxSizeData = new List<KeyValuePair<double, int>> ()

            {
                new KeyValuePair<double, int>(1, 0),
                new KeyValuePair<double, int>(0.5, 150),
                new KeyValuePair<double, int>(0.3, 500),
                new KeyValuePair<double, int>(0.25, 1000)
            };

        public static readonly List<KeyValuePair<TimeSpan, int>> TimerInitialTimeData = new List<KeyValuePair<TimeSpan, int>> ()

            {
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(15), 0),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(30), 1),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(60), 2), 
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(180), 3),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(300), 4)
            };

        public static readonly List<KeyValuePair<TimeSpan, int>> TimerExtraTimeData = new List<KeyValuePair<TimeSpan, int>> ()

            {
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(15), 0),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(30), 1),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(45), 2), 
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(60), 3),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(75), 4),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(90), 5)
            };

        public static readonly List<KeyValuePair<TimeSpan, int>> InvicibleTimeData = new List<KeyValuePair<TimeSpan, int>> ()

            {
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(1), 0),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(2), 1),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(3), 2), 
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(4), 3),
                new KeyValuePair<TimeSpan, int>(TimeSpan.FromSeconds(5), 4)
            };

        public static readonly List<KeyValuePair<double, int>> ScoreToMoneyFactorData = new List<KeyValuePair<double, int>> ()

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
                new KeyValuePair<int, int>(2, 1),
                new KeyValuePair<int, int>(3, 2),
                new KeyValuePair<int, int>(5, 3),
                new KeyValuePair<int, int>(10, 4),
                new KeyValuePair<int, int>(20, 5)
            };

        public static readonly List<KeyValuePair<int, int>> ScoreByEnemyData = new List<KeyValuePair<int, int>> 

            {
                new KeyValuePair<int, int>(50, 0),
                new KeyValuePair<int, int>(75, 1),
                new KeyValuePair<int, int>(100, 2),
                new KeyValuePair<int, int>(125, 3),
                new KeyValuePair<int, int>(150, 4)
            };
    }
}
