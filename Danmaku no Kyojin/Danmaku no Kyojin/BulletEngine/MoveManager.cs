using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.BulletEngine
{
    /// <summary>
    /// オブジェクトを一括管理する
    /// </summary>
    class MoverManager
    {
        public static List<Mover> movers = new List<Mover>(); //Moverのリスト

        /// <summary>
        /// 新しいMoverを作成
        /// </summary>
        static public Mover CreateMover()
        {
            Mover mover = new Mover();
            movers.Add(mover); //Moverを登録
            mover.Init(); //初期化
            return mover;
        }

        /// <summary>
        /// すべてのMoverの行動を実行する
        /// </summary>
        static public void Update(GameTime gameTime)
        {
            for (int i = 0; i < movers.Count; i++)
            {
                movers[i].Update(gameTime);
            }
        }


        /// <summary>
        /// 使われなくなったMoverを解放する
        /// </summary>
        static public void FreeMovers()
        {
            for (int i = 0; i < movers.Count; i++)
            {
                if (!movers[i].used)
                {
                    movers.Remove(movers[i]);
                    i--;
                }
            }
        }
    }
}
