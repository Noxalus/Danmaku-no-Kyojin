using System;
using System.Diagnostics;
using System.Globalization;
using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.Entities
{
    public class Timer : DrawableGameComponent
    {
        private readonly TimeSpan _initTime;
        private TimeSpan _currentTime;
        private SpriteBatch _spriteBatch;
        private bool _active;

        // Fonts
        private SpriteFont _secondsFont;
        private SpriteFont _millisecondsFont;

        public Timer(Game game, int seconds)
            : base(game)
        {
            _initTime = new TimeSpan(0, 0, seconds);
        }

        public override void Initialize()
        {
            _currentTime = _initTime;
            _active = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _secondsFont = Game.Content.Load<SpriteFont>("Graphics/Fonts/TimerSeconds");
            _millisecondsFont = Game.Content.Load<SpriteFont>("Graphics/Fonts/TimerMilliseconds");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (_currentTime > TimeSpan.Zero)
                _currentTime -= gameTime.ElapsedGameTime;
            else
                _currentTime = TimeSpan.Zero;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            string seconds = Math.Round(_currentTime.TotalSeconds).ToString(CultureInfo.InvariantCulture);
            string milliseconds = string.Format("{0:00}", (_currentTime.Milliseconds / 10));

            Debug.Print((_millisecondsFont.MeasureString(milliseconds).Y).ToString());

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_secondsFont, seconds, new Vector2(
                Config.Resolution.X / 2f - _secondsFont.MeasureString(seconds).X / 2f,
                -10), Color.White);
            _spriteBatch.DrawString(_millisecondsFont, milliseconds, new Vector2(
                Config.Resolution.X / 2f + _secondsFont.MeasureString(seconds).X / 2f,
                20), Color.White);
            _spriteBatch.End();
        }

        public void Play()
        {
            _active = true;
        }

        public void Stop()
        {
            _active = false;
        }
    }
}
