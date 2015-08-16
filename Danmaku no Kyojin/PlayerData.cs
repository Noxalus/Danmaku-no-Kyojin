using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Danmaku_no_Kyojin
{
    static class PlayerData
    {
        /** Player data **/
        public static int Credits = 0; // Money to buy improvements

        public static bool BulletTimeEnabled = false; // true|false
        public static bool SlowModeEnabled = false; // true|false
        public static bool HitBoxDisplay = false; // true|false

        // Improvements index
        public static int ShootTypeIndex = 0;
        public static int ShootFrequencyIndex = 0;
        public static int ShootPowerIndex = 0;
        public static int LivesNumberIndex = 0;
        public static int BulletTimeTimerIndex = 0;
        public static int BulletTimeNumberIndex = 0;
        public static int BulletTimeDivisorIndex = 0;
        public static int SpeedIndex = 0;
        public static int HitBoxSizeIndex = 0;
        public static int TimerInitialTimeIndex = 0;
        public static int TimerExtraTimeIndex = 0;
        public static int InvicibleTimeIndex = 0;
        public static int ScoreToMoneyMultiplicatorIndex = 0;
        public static int ScoreByHitIndex = 0;
        public static int ScoreByEnemyIndex = 0;

        // Future features ?
        public static bool ShieldEnabled = false; // true|false
        public static int ShieldNumberIndex = 0;
        public static int ShieldDurationIndex = 0;
    }
}
