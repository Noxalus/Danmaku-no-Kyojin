using System.Collections.Generic;
using System.Globalization;
using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

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

        private SoundEffect _buySound;

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
            _background = GameRef.Content.Load<Texture2D>("Graphics/Pictures/background");

            _titleFont = Game.Content.Load<SpriteFont>("Graphics/Fonts/TitleFont");

            _buySound = Game.Content.Load<SoundEffect>(@"Audio/SE/buy");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.PressedCancel())
                StateManager.ChangeState(GameRef.TitleScreen);

            if (InputHandler.PressedUp())
            {
                GameRef.Select.Play();

                _menuIndex--;

                if (_menuIndex < 0)
                    _menuIndex = _menuText.Length;
            }

            if (InputHandler.PressedDown())
            {
                GameRef.Select.Play();

                _menuIndex = (_menuIndex + 1) % (_menuText.Length + 1);
            }

            if (InputHandler.PressedAction())
            {
                if (_menuIndex == _menuText.Length)
                {
                    GameRef.Choose.Play();
                    StateManager.ChangeState(GameRef.TitleScreen);
                }

                bool error = false;
                switch (_menuIndex)
                {
                    // Lives
                    case 0:
                        if (!_finished["livesNumber"] &&
                            PlayerData.LivesNumberIndex < Improvements.LivesNumberData.Count - 1 &&
                            PlayerData.Credits >= Improvements.LivesNumberData[PlayerData.LivesNumberIndex + 1].Value)
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
                        if (!_finished["shootType"] &&
                            PlayerData.ShootTypeIndex < Improvements.ShootTypeData.Count - 1 &&
                            PlayerData.Credits >= Improvements.ShootTypeData[PlayerData.ShootTypeIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.ShootTypeData[PlayerData.ShootTypeIndex + 1].Value;
                            PlayerData.ShootTypeIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 2:
                        if (!_finished["shootPower"] &&
                            PlayerData.ShootPowerIndex < Improvements.ShootPowerData.Count - 1 &&
                            PlayerData.Credits >= Improvements.ShootPowerData[PlayerData.ShootPowerIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.ShootPowerData[PlayerData.ShootPowerIndex + 1].Value;
                            PlayerData.ShootPowerIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 3:
                        if (!_finished["shootFrequency"] &&
                            PlayerData.ShootFrequencyIndex < Improvements.ShootFrequencyData.Count - 1 &&
                            PlayerData.Credits >= Improvements.ShootFrequencyData[PlayerData.ShootFrequencyIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.ShootFrequencyData[PlayerData.ShootFrequencyIndex + 1].Value;
                            PlayerData.ShootFrequencyIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 4:
                        if (!_finished["timerInitialTime"] &&
                            PlayerData.TimerInitialTimeIndex < Improvements.TimerInitialTimeData.Count - 1 &&
                            PlayerData.Credits >= Improvements.TimerInitialTimeData[PlayerData.TimerInitialTimeIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.TimerInitialTimeData[PlayerData.TimerInitialTimeIndex + 1].Value;
                            PlayerData.TimerInitialTimeIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 5:
                        if (!_finished["timerExtraTime"] &&
                            PlayerData.TimerExtraTimeIndex < Improvements.TimerExtraTimeData.Count - 1 &&
                            PlayerData.Credits >= Improvements.TimerExtraTimeData[PlayerData.TimerExtraTimeIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.TimerExtraTimeData[PlayerData.TimerExtraTimeIndex + 1].Value;
                            PlayerData.TimerExtraTimeIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 6:
                        if (!_finished["bulletTimeDivisor"] &&
                            PlayerData.InvicibleTimeIndex < Improvements.InvicibleTimeData.Count - 1 &&
                            PlayerData.Credits >= Improvements.InvicibleTimeData[PlayerData.InvicibleTimeIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.InvicibleTimeData[PlayerData.InvicibleTimeIndex + 1].Value;
                            PlayerData.InvicibleTimeIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 7:
                        if (!_finished["slowMode"] &&
                            !PlayerData.SlowModeEnabled &&
                            PlayerData.Credits >= Improvements.SlowModePrice)
                        {
                            PlayerData.Credits -= Improvements.SlowModePrice;
                            PlayerData.SlowModeEnabled = true;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 8:
                        if (!_finished["bulletTime"] &&
                            !PlayerData.BulletTimeEnabled &&
                            PlayerData.Credits >= Improvements.BulletTimePrice)
                        {
                            PlayerData.Credits -= Improvements.BulletTimePrice;
                            PlayerData.BulletTimeEnabled = true;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 9:
                        if (!_finished["bulletTimeTimer"] &&
                            PlayerData.BulletTimeTimerIndex < Improvements.BulletTimeTimerData.Count - 1 &&
                            PlayerData.Credits >= Improvements.BulletTimeTimerData[PlayerData.BulletTimeTimerIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.BulletTimeTimerData[PlayerData.BulletTimeTimerIndex + 1].Value;
                            PlayerData.BulletTimeTimerIndex++;
                        }
                        else
                        {
                            error = true;
                        }
                        break;
                    case 10:
                        if (!_finished["bulletTimeDivisor"] &&
                            PlayerData.BulletTimeDivisorIndex < Improvements.BulletTimeDivisorData.Count - 1 &&
                            PlayerData.Credits >= Improvements.BulletTimeDivisorData[PlayerData.BulletTimeDivisorIndex + 1].Value)
                        {
                            PlayerData.Credits -= Improvements.BulletTimeDivisorData[PlayerData.BulletTimeDivisorIndex + 1].Value;
                            PlayerData.BulletTimeDivisorIndex++;
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
                {
                    _error = "";
                    _buySound.Play();
                }

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
            int lineHeight = 40;
            Color color;
            for (int i = 0; i < _menuText.Length; i++)
            {
                color = Color.White;
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _menuText[i], new Vector2(menuTextOrigin.X + 1, menuTextOrigin.Y + lineHeight * i + 1), Color.Black);
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _menuText[i], new Vector2(menuTextOrigin.X, menuTextOrigin.Y + lineHeight * i), Color.White);

                if (_menuIndex == i)
                    color = Color.Red;

                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Buy", new Vector2(menuTextOrigin.X + 500 + 1, menuTextOrigin.Y + lineHeight * i + 1), Color.Black);
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "Buy", new Vector2(menuTextOrigin.X + 500, menuTextOrigin.Y + lineHeight * i), color);
            }

            // Back
            color = Color.White;
            if (_menuIndex == _menuText.Length)
                color = Color.Red;

            string back = "Back to title";

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, back, new Vector2(
                Config.Resolution.X / 2f - ControlManager.SpriteFont.MeasureString(back).X / 2 + 1, 
                menuTextOrigin.Y + lineHeight * _menuText.Length + 1), 
            Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, back, new Vector2(
                Config.Resolution.X / 2f - ControlManager.SpriteFont.MeasureString(back).X / 2,
                menuTextOrigin.Y + lineHeight * _menuText.Length), 
            color);

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

            string shootType = "Shoot type: ";
            if (PlayerData.ShootTypeIndex < Improvements.ShootTypeData.Count - 1)
            {
                shootType += Improvements.ShootTypeData[PlayerData.ShootTypeIndex + 1].Key + " (" +
                              Improvements.ShootTypeData[PlayerData.ShootTypeIndex + 1].Value + "$)";
                _finished.Add("shootType", false);
            }
            else
            {
                _finished.Add("shootType", true);
                shootType += "FINISHED";
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

            string timerInitialTime = "Timer initial time: ";
            if (PlayerData.TimerInitialTimeIndex < Improvements.TimerInitialTimeData.Count - 1)
            {
                timerInitialTime += Improvements.TimerInitialTimeData[PlayerData.TimerInitialTimeIndex + 1].Key + " (" +
                                  Improvements.TimerInitialTimeData[PlayerData.TimerInitialTimeIndex + 1].Value + "$)";
                _finished.Add("timerInitialTime", false);
            }
            else
            {
                _finished.Add("timerInitialTime", true);
                timerInitialTime += "FINISHED";
            }

            string timerExtraTime = "Timer extra time: ";
            if (PlayerData.TimerExtraTimeIndex < Improvements.TimerExtraTimeData.Count - 1)
            {
                timerExtraTime += Improvements.TimerExtraTimeData[PlayerData.TimerExtraTimeIndex + 1].Key + " (" +
                                  Improvements.TimerExtraTimeData[PlayerData.TimerExtraTimeIndex + 1].Value + "$)";
                _finished.Add("timerExtraTime", false);
            }
            else
            {
                _finished.Add("timerExtraTime", true);
                timerExtraTime += "FINISHED";
            }

            string invicibleTime = "Invicible time: ";
            if (PlayerData.InvicibleTimeIndex < Improvements.InvicibleTimeData.Count - 1)
            {
                invicibleTime += Improvements.InvicibleTimeData[PlayerData.InvicibleTimeIndex + 1].Key + " (" +
                                  Improvements.InvicibleTimeData[PlayerData.InvicibleTimeIndex + 1].Value + "$)";
                _finished.Add("invicibleTime", false);
            }
            else
            {
                _finished.Add("invicibleTime", true);
                invicibleTime += "FINISHED";
            }

            string slowMode = "Slow mode: ";
            if (!PlayerData.SlowModeEnabled)
            {
                slowMode += "UNLOCK (" + Improvements.SlowModePrice + "$)";
                _finished.Add("slowMode", false);
            }
            else
            {
                _finished.Add("slowMode", true);
                slowMode += "UNLOCKED";
            }

            string bulletTime = "Bullet time: ";
            if (!PlayerData.BulletTimeEnabled)
            {
                bulletTime += "UNLOCK (" + Improvements.BulletTimePrice + "$)";
                _finished.Add("bulletTime", false);
            }
            else
            {
                _finished.Add("bulletTime", true);
                bulletTime += "UNLOCKED";
            }

            string bulletTimeTimer = "Bullet time timer: ";
            if (PlayerData.BulletTimeTimerIndex < Improvements.BulletTimeTimerData.Count - 1)
            {
                bulletTimeTimer += Improvements.BulletTimeTimerData[PlayerData.BulletTimeTimerIndex + 1].Key + " (" +
                                  Improvements.BulletTimeTimerData[PlayerData.BulletTimeTimerIndex + 1].Value + "$)";
                _finished.Add("bulletTimeTimer", false);
            }
            else
            {
                _finished.Add("bulletTimeTimer", true);
                bulletTimeTimer += "FINISHED";
            }

            string bulletTimeDivisor = "Bullet time divisor: ";
            if (PlayerData.BulletTimeDivisorIndex < Improvements.BulletTimeDivisorData.Count - 1)
            {
                bulletTimeDivisor += "x1/" + (Improvements.BulletTimeDivisorData[PlayerData.BulletTimeDivisorIndex + 1].Key) + " (" +
                                  Improvements.BulletTimeDivisorData[PlayerData.BulletTimeDivisorIndex + 1].Value + "$)";
                _finished.Add("bulletTimeDivisor", false);
            }
            else
            {
                _finished.Add("bulletTimeDivisor", true);
                bulletTimeDivisor += "FINISHED";
            }

            _menuText = new string[]
                {
                    livesNumber,
                    shootType,
                    shootPower,
                    shootFrequency,
                    timerInitialTime,
                    timerExtraTime,
                    invicibleTime,
                    slowMode,
                    bulletTime,
                    bulletTimeTimer,
                    bulletTimeDivisor
                };
        }

    }
}
