using Danmaku_no_Kyojin.BulletML;
using Danmaku_no_Kyojin.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin
{
    /// <summary>
    /// BulletMLLibから呼ばれる関数群を実装
    /// </summary>
    class BulletFunctions : IBulletMLManager
    {
        public float GetRandom() { return (float)GameplayScreen.Rand.NextDouble(); }

        public float GetRank() { return 0.5f; }

        public float GetPlayerPosX() { return GameplayScreen.Player.GetPosition().X; } //自機の座標を返す

        public float GetPlayerPosY() { return GameplayScreen.Player.GetPosition().Y; } //自機の座標を返す
    }
}
