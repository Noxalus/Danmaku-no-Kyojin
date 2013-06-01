using System.Globalization;
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

        private int _waveNumber;

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

        // Bloom
        private readonly BloomComponent _bloom;
        private int _bloomSettingsIndex = 0;

        // Background
        private Texture2D _background;
        private float _counter = 0;

        public GameplayScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {

            Players = new List<Player>();

            _enemy = new Boss(GameRef);

            // Timer
            _timer = new Timer(Game, Improvements.TimerInitialTimeData[PlayerData.TimerInitialTimeIndex].Key);

            // Bloom effect
            _bloom = new BloomComponent(this.Game);

            Components.Add(_bloom);
        }

        public override void Initialize()
        {
            Players.Clear();

            for (int i = 1; i <= Config.PlayersNumber; i++)
            {
                var player = new Player(GameRef, i, Config.PlayersController[i - 1],
                                        new Vector2(GameRef.Graphics.GraphicsDevice.Viewport.Width / 2f,
                                                    GameRef.Graphics.GraphicsDevice.Viewport.Height - 150));
                player.Initialize();
                Players.Add(player);
            }

            _enemy.Initialize();
            _waveNumber = 0;

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

            _bloom.Initialize();
        }

        protected override void LoadContent()
        {
            if (hit == null)
            {
                hit = GameRef.Content.Load<SoundEffect>(@"Audio/SE/hit");
            }

            _background = Game.Content.Load<Texture2D>("Graphics/Pictures/background");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            _music.Stop(AudioStopOptions.AsAuthored);
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput();

            _timer.Update(gameTime);
            _counter += 0.01f;

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
                    if (p.BulletTime)
                    {
                        var newGameTime = new GameTime(
                            gameTime.TotalGameTime,
                            new TimeSpan((long)(gameTime.ElapsedGameTime.Ticks / Config.DesiredTimeModifier))
                        );

                        gameTime = newGameTime;
                    }

                    for (int i = 0; i < p.GetBullets().Count; i++)
                    {
                        p.GetBullets()[i].Update(gameTime);

                        if (_enemy.IsAlive && _enemy.GetBoundingElement().Intersects(p.GetBullets()[i].GetBoundingElement()))
                        {
                            _enemy.TakeDamage(p.GetBullets()[i].Power);
                            p.GetBullets().Remove(p.GetBullets()[i]);
                            hit.Play();
                            p.AddScore(Improvements.ScoreByHitData[PlayerData.ScoreByHitIndex].Key);
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

                    p.Update(gameTime);
                }
            }

            if (_enemy.IsAlive)
            {
                _enemy.Update(gameTime);
            }
            else
            {
                _waveNumber++;

                _timer.AddTime(Improvements.TimerExtraTimeData[PlayerData.TimerExtraTimeIndex].Key);

                _enemy.Initialize();
            }

            // Game Over
            if ((!Players[0].IsAlive && (Config.PlayersNumber == 1 || (Config.PlayersNumber == 2 && !Players[1].IsAlive))) || _timer.IsFinished)
            {
                UnloadContent();

                GameRef.GameOverScreen.WaveNumber = _waveNumber;
                GameRef.GameOverScreen.Player1Score = Players[0].Score;
                if (Config.PlayersNumber == 2)
                    GameRef.GameOverScreen.Player2Score = Players[1].Score;

                PlayerData.Money += 
                    GameRef.GameOverScreen.Player1Score +
                    GameRef.GameOverScreen.Player2Score +
                    (Improvements.ScoreByEnemyData[PlayerData.ScoreByEnemyIndex].Key * GameRef.GameOverScreen.WaveNumber);


                StateManager.ChangeState(GameRef.GameOverScreen);
            }

            if (InputHandler.KeyPressed(Keys.C))
                Config.DisplayCollisionBoxes = !Config.DisplayCollisionBoxes;
        }

        public override void Draw(GameTime gameTime)
        {
            _bloom.BeginDraw();

            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.Begin(0, BlendState.Opaque);

            GameRef.SpriteBatch.Draw(_background, new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y),
                                     Color.White);

            /*
            GameRef.SpriteBatch.Draw(_background, new Rectangle(
                (int)(Config.Resolution.X / 2f), (int)(Config.Resolution.Y / 2f), 
                Config.Resolution.X * 2, Config.Resolution.Y * 2), 
                null, Color.White, _counter, new Vector2(_background.Width / 2f, _background.Height / 2f), SpriteEffects.None, 0f);
            */

            GameRef.SpriteBatch.End();

            GameRef.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

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

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);

            GameRef.SpriteBatch.Begin();

            _timer.Draw(gameTime);

            foreach (Player p in Players)
            {
                if (p.IsAlive)
                {
                    p.DrawString(gameTime);
                }
            }

            // Text
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Boss bullets: " + _enemy.MoverManager.movers.Count.ToString(CultureInfo.InvariantCulture),
                new Vector2(1, Config.Resolution.Y - 59), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Boss bullets: " + _enemy.MoverManager.movers.Count.ToString(CultureInfo.InvariantCulture),
                new Vector2(0, Config.Resolution.Y - 60), Color.White);

            string waveNumber = "Wave #" + _waveNumber.ToString(CultureInfo.InvariantCulture);

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, waveNumber,
                new Vector2(Config.Resolution.X / 2f - ControlManager.SpriteFont.MeasureString(waveNumber).X / 2f + 1, Config.Resolution.Y - 29), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, waveNumber,
                new Vector2(Config.Resolution.X / 2f - ControlManager.SpriteFont.MeasureString(waveNumber).X / 2f, Config.Resolution.Y - 30), Color.White);

            /*
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player bullets: " + p.GetBullets().Count.ToString(), new Vector2(1, 41), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Player bullets: " + p.GetBullets().Count.ToString(), new Vector2(0, 40), Color.White);
            */

            GameRef.SpriteBatch.End();
        }

        /// <summary>
        /// Handles input for quitting or changing the bloom settings.
        /// </summary>
        private void HandleInput()
        {
            // Switch to the next bloom settings preset?
            if (InputHandler.KeyPressed(Keys.I))
            {
                _bloomSettingsIndex = (_bloomSettingsIndex + 1) %
                                     BloomSettings.PresetSettings.Length;

                _bloom.Settings = BloomSettings.PresetSettings[_bloomSettingsIndex];
                _bloom.Visible = true;
            }

            // Toggle bloom on or off?
            if (InputHandler.KeyPressed(Keys.O))
            {
                _bloom.Visible = !_bloom.Visible;
            }

            // Cycle through the intermediate buffer debug display modes?
            if (InputHandler.KeyPressed(Keys.P))
            {
                _bloom.Visible = true;
                _bloom.ShowBuffer++;

                if (_bloom.ShowBuffer > BloomComponent.IntermediateBuffer.FinalResult)
                    _bloom.ShowBuffer = 0;
            }
        }
    }
}
