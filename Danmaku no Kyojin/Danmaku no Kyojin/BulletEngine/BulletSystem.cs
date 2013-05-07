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

        public float GetShipPosX() { return GameplayScreen.Ship.Position.X; } //自機の座標を返す

        public float GetShipPosY() { return GameplayScreen.Ship.Position.Y; } //自機の座標を返す
    }
}
