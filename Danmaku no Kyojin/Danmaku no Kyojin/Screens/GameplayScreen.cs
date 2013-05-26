using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Collisions;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Entities;
using Danmaku_no_Kyojin.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Danmaku_no_Kyojin.Screens
{
    public class GameplayScreen : BaseGameState
    {
        public List<Player> Players { get; set; }
        private Boss _enemy;

        // Audio
        AudioEngine _audioEngine;
        WaveBank _waveBank;
        SoundBank _soundBank;
        AudioCategory _musicCategory;

        Cue _music;
        private SoundEffect hit = null;

        // Random
        public static Random Rand = new Random();

        // Timer
        private readonly Timer _timer;

        public GameplayScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {

            Players = new List<Player>();

            _enemy = new Boss(GameRef);

            // Timer
            _timer = new Timer(Game, 102);
            Components.Add(_timer);
        }

        public override void Initialize()
        {
            Players.Clear();

            for (int i = 1; i <= Config.PlayersNumber; i++)
            {
                var player = new Player(GameRef, i,
                                           new Vector2(GameRef.Graphics.GraphicsDevice.Viewport.Width / 2f,
                                                       GameRef.Graphics.GraphicsDevice.Viewport.Height - 150));
                player.Initialize();
                Players.Add(player);
            };

            _enemy.Initialize();

            _audioEngine = new AudioEngine(@"Content/Audio/DnK.xgs");
            _waveBank = new WaveBank(_audioEngine, @"Content/Audio/Wave Bank.xwb");
            _soundBank = new SoundBank(_audioEngine, @"Content/Audio/Sound Bank.xsb");

            _musicCategory = _audioEngine.GetCategory("Music");

            _musicCategory.SetVolume(1);

            SoundEffect.MasterVolume = 0.25f;

            _timer.Initialize();

            base.Initialize();

            _music = _soundBank.GetCue("Background");
            _music.Play();
        }

        protected override void LoadContent()
        {
            if (hit == null)
            {
                hit = GameRef.Content.Load<SoundEffect>(@"Audio/SE/hit");
            }

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            _music.Stop(AudioStopOptions.AsAuthored);
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHandler.KeyDown(Keys.Escape))
            {
                UnloadContent();
                StateManager.ChangeState(GameRef.TitleScreen);
            }

            base.Update(gameTime);

            _audioEngine.Update();

            foreach (Player p in Players)
            {
                if (p.IsAlive)
                {
                    for (int i = 0; i < p.GetBullets().Count; i++)
                    {
                        p.GetBullets()[i].Update(gameTime);

                        if (_enemy.IsAlive && _enemy.GetBoundingElement().Intersects(p.GetBullets()[i].GetBoundingElement()))
                        {
                            _enemy.TakeDamage(p.GetBullets()[i].Power);
                            p.GetBullets().Remove(p.GetBullets()[i]);
                            hit.Play();
                            p.AddScore(5);
                        }
                        else
                        {
                            if (p.GetBullets()[i].X < 0 || p.GetBullets()[i].X > Config.Resolution.X ||
                                p.GetBullets()[i].Y < 0 || p.GetBullets()[i].Y > Config.Resolution.Y)
                            {
                                p.GetBullets().Remove(p.GetBullets()[i]);
                            }
                        }
                    }

                    p.Update(gameTime);
                }


                if (p.BulletTime)
                {
                    var newGameTime = new GameTime(
                        gameTime.TotalGameTime,
                        new TimeSpan((long)(gameTime.ElapsedGameTime.Ticks / Config.DesiredTimeModifier))
                    );

                    gameTime = newGameTime;
                }

                if (!p.IsInvincible)
                {
                    if (p.Intersects(_enemy))
                    {
                        p.Hit();
                    }

                    foreach (Mover m in _enemy.MoverManager.movers)
                    {
                        if (p.Intersects(m))
                            p.Hit();
                    }
                }
            }

            if (_enemy.IsAlive)
            {
                _enemy.Update(gameTime);
            }

            // Adjust zoom if the mouse wheel has moved
            if (InputHandler.ScrollUp())
                GameRef.Camera.Zoom += 0.1f;
            else if (InputHandler.ScrollDown())
                GameRef.Camera.Zoom -= 0.1f;

            // Move the camera when the arrow keys are pressed
            Vector2 movement = Vector2.Zero;
            Viewport vp = GameRef.GraphicsDevice.Viewport;

            if (InputHandler.KeyDown(Keys.Left))
                movement.X -= 0.1f;
            if (InputHandler.KeyDown(Keys.Right))
                movement.X += 0.1f;
            if (InputHandler.KeyDown(Keys.Up))
                movement.Y -= 0.1f;
            if (InputHandler.KeyDown(Keys.Down))
                movement.Y += 0.1f;

            GameRef.Camera.Pos += movement * 20;

            if (InputHandler.KeyPressed(Keys.C))
                Config.DisplayCollisionBoxes = !Config.DisplayCollisionBoxes;
        }

        public override void Draw(GameTime gameTime)
        {
            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.Begin();

            foreach (Player p in Players)
            {
                if (p.IsAlive)
                {
                    foreach (var bullet in p.GetBullets())
                    {
                        bullet.Draw(gameTime);
                    }

                    p.Draw(gameTime);
                }
            }

            if (_enemy.IsAlive)
            {
                _enemy.Draw(gameTime);
            }

            // Text
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Boss bullets: " + _enemy.MoverManager.movers.Count.ToString(), new Vector2(1, 21), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Boss bullets: " + _enemy.MoverManager.movers.Count.ToString(), new Vector2(0, 20), Color.White);

            /*
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player bullets: " + p.GetBullets().Count.ToString(), new Vector2(1, 41), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player bullets: " + p.GetBullets().Count.ToString(), new Vector2(0, 40), Color.White);
            */

            base.Draw(gameTime);

            GameRef.SpriteBatch.End();
        }
    }
}
