using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.BulletML;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Screens
{
    public class GameplayScreen : BaseGameState
    {
        private List<Texture2D> _logos;
        private Texture2D _bulletSprite;

        public static Ship Ship;
        private Enemy _enemy;

        // Audio
        AudioEngine _audioEngine;
        WaveBank _waveBank;
        SoundBank _soundBank;

        Cue music = null;

        // Random
        public static Random Rand = new Random();

        // Bullet engine
        static public BulletMLParser parser = new BulletMLParser();
        int timer = 0;
        Mover mover;

        public GameplayScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        public override void Initialize()
        {
            Ship = new Ship(GameRef, new Vector2(GameRef.Graphics.GraphicsDevice.Viewport.Width / 2, GameRef.Graphics.GraphicsDevice.Viewport.Height - 150));
            _enemy = new Enemy(GameRef);

            GameRef.Components.Add(_enemy);
            GameRef.Components.Add(Ship);

            _audioEngine = new AudioEngine("Content\\Audio\\DnK.xgs");
            _waveBank = new WaveBank(_audioEngine, "Content\\Audio\\Wave Bank.xwb");
            _soundBank = new SoundBank(_audioEngine, "Content\\Audio\\Sound Bank.xsb");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _logos = new List<Texture2D>();

            for (int i = 0; i < 1; i++)
            {
                _logos.Add(GameRef.Content.Load<Texture2D>(@"Graphics/Pictures/logo"));
            }

            _bulletSprite = GameRef.Content.Load<Texture2D>(@"Graphics/Sprites/ball");
            parser.ParseXML(@"Content/XML/sample.xml");

            BulletMLManager.Init(new MyBulletFunctions());

            if (music == null)
            {
                music = _soundBank.GetCue("Background");
                music.Play();
            }

            base.LoadContent();
        }

        protected override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            /*
            timer++;
            if (timer > 1)
            {
                timer = 0;
                if (mover.used == false)
                {
                    mover = MoverManager.CreateMover();
                    mover.pos = new Vector2(40 + (800 * (float)Rand.NextDouble()), 40 + (600 * (float)Rand.NextDouble()));
                    mover.SetBullet(parser.tree);
                }
            }
            */


            if (MoverManager.movers.Count < 1)
            {
                mover = MoverManager.CreateMover();
                mover.pos = new Vector2(390, 200);
                mover.SetBullet(parser.tree);
            }

            if (Ship.SlowMode)
            {
                float DESIRED_TIME_MODIFIER = 5f;

                GameTime newGameTime = new GameTime(gameTime.TotalGameTime,
                    new TimeSpan((long)(gameTime.ElapsedGameTime.Ticks / DESIRED_TIME_MODIFIER)));
                gameTime = newGameTime;
            }

            MoverManager.Update(gameTime);
            MoverManager.FreeMovers();

        }

        public override void Draw(GameTime gameTime)
        {
            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.Begin();

            /*
            Random rand = new Random();

            foreach (Texture2D texture in _logos)
            {
                GameRef.SpriteBatch.Draw(texture, new Vector2(
                    rand.Next(50, (GameRef.Graphics.GraphicsDevice.Viewport.Width) - (texture.Width / 2)),
                    rand.Next(50, (GameRef.Graphics.GraphicsDevice.Viewport.Height) - (texture.Height / 2))), Color.White);
            }
            */
            /*
            GameRef.SpriteBatch.Draw(_logos[0], new Vector2(
                    (GameRef.Graphics.GraphicsDevice.Viewport.Width / 2) - (_logos[0].Width / 2),
                    0), Color.White);
            */

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Bullets: " + MoverManager.movers.Count.ToString(), new Vector2(1, 21), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Bullets: " + MoverManager.movers.Count.ToString(), new Vector2(0, 20), Color.White);

            foreach (Mover mover in MoverManager.movers)
                GameRef.SpriteBatch.Draw(_bulletSprite, mover.pos, Color.White);

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
