using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Entities;
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
        public Player Player { get; set; }
        private Boss _enemy;

        // Audio
        AudioEngine _audioEngine;
        WaveBank _waveBank;
        SoundBank _soundBank;

        Cue music = null;
        private SoundEffect hit = null;

        // Random
        public static Random Rand = new Random();

        // Bullets
        private List<BaseBullet> _bullets;

        public GameplayScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _bullets = new List<BaseBullet>();

            Player = new Player(GameRef, ref _bullets, new Vector2(GameRef.Graphics.GraphicsDevice.Viewport.Width / 2, GameRef.Graphics.GraphicsDevice.Viewport.Height - 150));
            _enemy = new Boss(GameRef);
        }

        public override void Initialize()
        {
            Player.Initialize();
            _enemy.Initialize();

            _audioEngine = new AudioEngine(@"Content/Audio/DnK.xgs");
            _waveBank = new WaveBank(_audioEngine, @"Content/Audio/Wave Bank.xwb");
            _soundBank = new SoundBank(_audioEngine, @"Content/Audio/Sound Bank.xsb");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            if (music == null)
            {
                music = _soundBank.GetCue("Background");
                //music.Play();
            }

            if (hit == null)
            {
                hit = GameRef.Content.Load<SoundEffect>(@"Audio/SE/hit");
            }

            base.LoadContent();
        }

        protected override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Player.IsAlive)
            {
                for (int i = 0; i < _bullets.Count; i++)
                {
                    _bullets[i].Update(gameTime);

                    if (_enemy.IsAlive && _enemy.CheckCollision(_bullets[i].GetPosition(),
                                              new Point(_bullets[i].Sprite.Width, _bullets[i].Sprite.Height)))
                    {
                        _enemy.TakeDamage(_bullets[i].Power);
                        _bullets.Remove(_bullets[i]);
                        //hit.Play();
                    }
                    else
                    {
                        if (_bullets[i].GetPosition().X < 0 || _bullets[i].GetPosition().X > Config.Resolution.X ||
                            _bullets[i].GetPosition().Y < 0 || _bullets[i].GetPosition().Y > Config.Resolution.Y)
                        {
                            _bullets.Remove(_bullets[i]);
                        }
                    }
                }

                Player.Update(gameTime);
            }

            if (Player.BulletTime)
            {
                GameTime newGameTime = new GameTime(gameTime.TotalGameTime,
                                                    new TimeSpan(
                                                        (long)
                                                        (gameTime.ElapsedGameTime.Ticks / Config.DesiredTimeModifier)));
                gameTime = newGameTime;
            }

            if (_enemy.IsAlive)
            {
                _enemy.Update(gameTime);
            }

            if (!Player.IsInvincible)
            {
                foreach (Mover m in _enemy.MoverManager.movers)
                {
                    Player.CheckCollision(m.pos, new Point(18, 18));
                }
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
        }

        public override void Draw(GameTime gameTime)
        {
            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                null,
                GameRef.Camera.GetTransformation());

            if (Player.IsAlive)
            {
                foreach (var bullet in _bullets)
                {
                    bullet.Draw(gameTime);
                }

                Player.Draw(gameTime);
            }

            if (_enemy.IsAlive)
            {
                _enemy.Draw(gameTime);
            }

            // Text
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Boss bullets: " + _enemy.MoverManager.movers.Count.ToString(), new Vector2(1, 21), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Boss bullets: " + _enemy.MoverManager.movers.Count.ToString(), new Vector2(0, 20), Color.White);

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player bullets: " + _bullets.Count.ToString(), new Vector2(1, 41), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player bullets: " + _bullets.Count.ToString(), new Vector2(0, 40), Color.White);

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
