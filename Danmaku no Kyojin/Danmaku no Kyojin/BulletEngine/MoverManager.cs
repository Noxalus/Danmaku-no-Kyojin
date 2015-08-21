using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Danmaku_no_Kyojin.BulletEngine
{
    /// <summary>
    /// オブジェクトを一括管理する
    /// </summary>
    class MoverManager : IBulletManager
    {
        public List<Mover> movers = new List<Mover>(); //Moverのリスト

        public PositionDelegate GetPlayerPosition;

        private DnK _gameRef;

        private SoundEffect _shoot;

        public MoverManager(DnK game)
        {
            _gameRef = game;
        }

        public void Initialize(PositionDelegate playerDelegate)
        {
            Debug.Assert(null != playerDelegate);
            GetPlayerPosition = playerDelegate;
            _shoot = _gameRef.Content.Load<SoundEffect>(@"Audio/SE/boss_shoot");
        }

        /// <summary>
        /// a mathod to get current position of the player
        /// This is used to target bullets at that position
        /// </summary>
        /// <returns>The position to aim the bullet at</returns>
        /// <param name="targettedBullet">the bullet we are getting a target for</param>
        public Vector2 PlayerPosition(Bullet targettedBullet)
        {
            //just give the player's position
            Debug.Assert(null != GetPlayerPosition);
            return GetPlayerPosition();
        }

        /// <summary>
        /// 新しい弾(Mover)を作成するときライブラリから呼ばれる
        /// </summary>
        public Bullet CreateBullet()
        {
            _shoot?.Play();

            var mover = new Mover(_gameRef, this);
            movers.Add(mover); //Moverを登録
            mover.Initialize(); //初期化
            return mover;
        }

        /// <summary>
        /// 弾が消えたときにライブラリから呼び出される
        /// </summary>
        public void RemoveBullet(Bullet deadBullet)
        {
            Mover myMover = deadBullet as Mover;
            if (myMover != null)
            {
                myMover.Used = false;
            }
        }

        /// <summary>
        /// すべてのMoverの行動を実行する
        /// </summary>
        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < movers.Count; i++)
            {
                movers[i].Update(gameTime);
            }
        }

        /// <summary>
        /// 使われなくなったMoverを解放する
        /// </summary>
        public void FreeMovers()
        {
            for (int i = 0; i < movers.Count; i++)
            {
                if (!movers[i].Used)
                {
                    movers.Remove(movers[i]);
                    i--;
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var mover in movers)
                mover.Draw(gameTime);
        }
    }
}
