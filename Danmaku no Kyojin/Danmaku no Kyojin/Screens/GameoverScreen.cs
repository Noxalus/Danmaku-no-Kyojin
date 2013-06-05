using System;
using System.Collections.Generic;
using System.Globalization;
using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Danmaku_no_Kyojin.Screens
{
    public class GameOverScreen : BaseGameState
    {
        #region Field region

        private string _title;
        private List<string> _content;
        private string[] _actions;
        public bool Died { get; set; }
        public TimeSpan Time { get; set; }
        public int WaveNumber { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public int TotalScore { get; set; }

        private Texture2D _background;
        private SpriteFont _titleFont;
        private int _menuIndex;

        #endregion

        #region Constructor region

        public GameOverScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _content = new List<string>();
        }

        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            MediaPlayer.Stop();

            _menuIndex = 0;

            _title = (Died) ? "YOU DIED !" : "TIME'S UP !";

            // Scores
            _content.Clear();
            _content.Add("Time: " + Math.Round(Time.TotalSeconds, 2).ToString(CultureInfo.InvariantCulture) + " second(s)");
            _content.Add("Wave number: " + WaveNumber.ToString(CultureInfo.InvariantCulture));
            _content.Add("P1 Score: " + Player1Score.ToString(CultureInfo.InvariantCulture));
            if (Config.PlayersNumber == 2)
                _content.Add("P2 Score: " + Player2Score.ToString(CultureInfo.InvariantCulture));

            _content.Add("Total Score: " + TotalScore.ToString(CultureInfo.InvariantCulture));

            _content.Add("Total credits: " + PlayerData.Credits.ToString(CultureInfo.InvariantCulture));

            _actions = new string[] { "Try again", "Back to title screen" };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _background = Game.Content.Load<Texture2D>("Graphics/Pictures/background");
            _titleFont = Game.Content.Load<SpriteFont>("Graphics/Fonts/TitleFont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyPressed(Keys.Escape) || InputHandler.KeyPressed(Keys.Enter))
                StateManager.ChangeState(GameRef.TitleScreen);

            if (InputHandler.PressedUp())
            {
                _menuIndex--;

                if (_menuIndex < 0)
                    _menuIndex = _actions.Length - 1;
            }

            if (InputHandler.PressedDown())
            {
                _menuIndex = (_menuIndex + 1) % _actions.Length;
            }

            if (InputHandler.PressedAction())
            {
                switch (_menuIndex)
                {
                    case 0:
                        StateManager.ChangeState(GameRef.GameplayScreen);
                        break;
                    case 1:
                        StateManager.ChangeState(GameRef.TitleScreen);
                        break;
                    default:
                        StateManager.ChangeState(GameRef.TitleScreen);
                        break;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.Begin();

            GameRef.SpriteBatch.Draw(_background, new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y), Color.Red);

            GameRef.SpriteBatch.DrawString(_titleFont, _title,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - _titleFont.MeasureString(_title).X / 2 + 5,
                    Game.GraphicsDevice.Viewport.Height / 2f - (_titleFont.MeasureString(_title).Y * 2) + 5),
                Color.Black);
            GameRef.SpriteBatch.DrawString(_titleFont, _title,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - _titleFont.MeasureString(_title).X / 2,
                    Game.GraphicsDevice.Viewport.Height / 2f - (_titleFont.MeasureString(_title).Y * 2)),
                Color.White);

            for (int i = 0; i < _content.Count; i++)
            {
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _content[i],
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_content[i]).X / 2 + 1,
                    Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(_content[i]).Y / 2 + 20 * i - ((20 * _content.Count) / 2f) + 1),
                Color.Black);

                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _content[i],
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_content[i]).X / 2,
                    Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(_content[i]).Y / 2 + 20 * i - ((20 * _content.Count) / 2f)),
                Color.White);
            }

            for (int i = 0; i < _actions.Length; i++)
            {
                Color color = Color.White;
                if (i == _menuIndex)
                    color = Color.Red;

                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _actions[i],
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_actions[i]).X / 2 + 1,
                    Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(_actions[i]).Y / 2 + 20 * _content.Count + 20 * i + 1),
                Color.Black);

                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _actions[i],
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_actions[i]).X / 2,
                    Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(_actions[i]).Y / 2 + 20 * _content.Count + 20 * i),
                color);
            }

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
