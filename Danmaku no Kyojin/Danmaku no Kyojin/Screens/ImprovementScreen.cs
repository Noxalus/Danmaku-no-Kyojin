using System.Collections.Generic;
using System.Globalization;
using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Screens
{
    public class ImprovementScreen : BaseGameState
    {
        #region Field region

        private string _title;
        private string _credits;
        private string _error;
        private string[] _menuText;
        private int _menuIndex;
        private Dictionary<string, bool> _finished;
        private Texture2D _background;
        private SpriteFont _titleFont;

        #endregion

        #region Constructor region

        public ImprovementScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            _title = "Shop";

            UpdateMenuText();

            _menuIndex = 0;

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

            if (InputHandler.KeyPressed(Keys.Escape))
                StateManager.ChangeState(GameRef.TitleScreen);

            if (InputHandler.PressedUp())
            {
                _menuIndex--;

                if (_menuIndex < 0)
                    _menuIndex = _menuText.Length - 1;
            }

            if (InputHandler.PressedDown())
            {
                _menuIndex = (_menuIndex + 1) % _menuText.Length;
            }

            if (InputHandler.PressedAction())
            {
                bool error = false;
                switch (_menuIndex)
                {
                    // Lives
                    case 0:
                        if (!_finished["livesNumber"] && PlayerData.Credits >= Improvements.LivesNumberData[PlayerData.LivesNumberIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.LivesNumberData[PlayerData.LivesNumberIndex + 1].Value;
                            PlayerData.LivesNumberIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 1:
                        if (!_finished["shootPower"] && PlayerData.Credits >= Improvements.ShootPowerData[PlayerData.ShootPowerIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.ShootPowerData[PlayerData.ShootPowerIndex + 1].Value;
                            PlayerData.ShootPowerIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 2:
                        if (!_finished["shootFrequency"] && PlayerData.Credits >= Improvements.ShootFrequencyData[PlayerData.ShootFrequencyIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.ShootFrequencyData[PlayerData.ShootFrequencyIndex + 1].Value;
                            PlayerData.ShootFrequencyIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    default:
                        error = true;
                        break;
                }

                if (error)
                    _error = "You don't have enought credits to buy this !";
                else
                    _error = "";

                UpdateMenuText();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.Begin();

            GameRef.SpriteBatch.Draw(_background, new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y), Color.GreenYellow);

            // Title
            GameRef.SpriteBatch.DrawString(_titleFont, _title,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - _titleFont.MeasureString(_title).X / 2 + 5,
                    Game.GraphicsDevice.Viewport.Height / 2f - (_titleFont.MeasureString(_title).Y * 3) + 5),
                Color.Black);
            GameRef.SpriteBatch.DrawString(_titleFont, _title,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - _titleFont.MeasureString(_title).X / 2,
                    Game.GraphicsDevice.Viewport.Height / 2f - (_titleFont.MeasureString(_title).Y * 3)),
                Color.White);

            // Credits
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _credits,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_credits).X / 2 + 1,
                    100 + 1),
                Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _credits,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_credits).X / 2,
                    100), 
                Color.White);

            var menuTextOrigin = new Point(300, 150);
            for (int i = 0; i < _menuText.Length; i++)
            {
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _menuText[i], new Vector2(menuTextOrigin.X, menuTextOrigin.Y + 20 * i), Color.White);

                Color color = Color.White;
                if (_menuIndex == i)
                    color = Color.Red;

                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Buy", new Vector2(menuTextOrigin.X + 500, menuTextOrigin.Y + 20 * i), color);
            }

            // Errors
            if (_error != null)
            {
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _error,
                                               new Vector2(
                                                   Game.GraphicsDevice.Viewport.Width/2f -
                                                   ControlManager.SpriteFont.MeasureString(_error).X/2 + 1,
                                                   Config.Resolution.Y - 40 + 1),
                                               Color.Black);
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _error,
                                               new Vector2(
                                                   Game.GraphicsDevice.Viewport.Width/2f -
                                                   ControlManager.SpriteFont.MeasureString(_error).X/2,
                                                   Config.Resolution.Y - 40),
                                               Color.White);
            }

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        private void UpdateMenuText()
        {
            _credits = "You currently have " + PlayerData.Credits.ToString(CultureInfo.InvariantCulture) + "$";

            _finished = new Dictionary<string, bool>();

            string livesNumber = "Lives numbers: ";
            if (PlayerData.LivesNumberIndex < Improvements.LivesNumberData.Count - 1)
            {
                livesNumber += (Improvements.LivesNumberData[PlayerData.LivesNumberIndex + 1].Key + " (" +
                                 Improvements.LivesNumberData[PlayerData.LivesNumberIndex + 1].Value + "$)");

                _finished.Add("livesNumber", false);
            }
            else
            {
                _finished.Add("livesNumber", true);
                livesNumber += "FINISHED";
            }

            string shootPower = "Shoot power: ";
            if (PlayerData.ShootPowerIndex < Improvements.ShootPowerData.Count - 1)
            {
                shootPower += "x" + Improvements.ShootPowerData[PlayerData.ShootPowerIndex + 1].Key + " (" +
                              Improvements.ShootPowerData[PlayerData.ShootPowerIndex + 1].Value + "$)";
                _finished.Add("shootPower", false);
            }
            else
            {
                _finished.Add("shootPower", true);
                shootPower += "FINISHED";
            }

            string shootFrequency = "Shoot frequency: ";
            if (PlayerData.ShootFrequencyIndex < Improvements.ShootFrequencyData.Count - 1)
            {
                shootFrequency += "x" + Improvements.ShootFrequencyData[PlayerData.ShootFrequencyIndex + 1].Key + " (" +
                                  Improvements.ShootFrequencyData[PlayerData.ShootFrequencyIndex + 1].Value + "$)";
                _finished.Add("shootFrequency", false);
            }
            else
            {
                _finished.Add("shootFrequency", true);
                shootFrequency += "FINISHED";
            }

            _menuText = new string[]
                {
                    livesNumber,
                    shootPower,
                    shootFrequency
                };
        }

    }
}
