using System.Globalization;
using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Danmaku_no_Kyojin.Screens
{
    public class TitleScreen : BaseGameState
    {
        #region Field region

        private Texture2D _logo;
        private readonly string[] _menuText;
        private readonly string[] _menuDescription;
        private int _menuIndex;
        private int _passStep;

        private Texture2D _backgroundImage;
        private Rectangle _backgroundMainRectangle;
        private Rectangle _backgroundRightRectangle;
        private Rectangle _backgroundTopRectangle;
        private Rectangle _backgroundTopRightRectangle;

        #endregion

        #region Constructor region

        public TitleScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _menuText = new string[] { "1 Player", "2 Players", "Shop", "Leaderboard", "Options", "Exit" };
            _menuDescription = new string[] { 
                "Playing game with only one player", 
                "Playing game with your best friend", 
                "Get new abilities to crush more enemies",
                "A list of scores to know who have the biggest one", 
                "You can change inputs here", 
                "Warning: I've never tested this button !", 
            };

            _menuIndex = 0;
        }

        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            _backgroundMainRectangle = new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y);
            _backgroundRightRectangle = new Rectangle(Config.Resolution.X, 0, Config.Resolution.X, Config.Resolution.Y);
            _backgroundTopRectangle = new Rectangle(0, -Config.Resolution.Y, Config.Resolution.X, Config.Resolution.Y);
            _backgroundTopRightRectangle = new Rectangle(Config.Resolution.X, -Config.Resolution.Y, Config.Resolution.X, Config.Resolution.Y);

            // Music
            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(GameRef.Content.Load<Song>("Audio/Musics/Menu"));
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _logo = GameRef.Content.Load<Texture2D>(@"Graphics/Pictures/logo");
            _backgroundImage = GameRef.Content.Load<Texture2D>(@"Graphics/Pictures/background");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyPressed(Keys.Escape))
                Game.Exit();

            if (InputHandler.PressedUp())
            {
                _menuIndex--;

                if (_menuIndex < 0)
                    _menuIndex = _menuText.Length - 1;

                GameRef.Select.Play();
            }

            if (InputHandler.PressedDown())
            {
                _menuIndex = (_menuIndex + 1) % _menuText.Length;
                GameRef.Select.Play();
            }

            if (InputHandler.PressedAction())
            {
                GameRef.Choose.Play();

                // 1 Player
                if (_menuIndex == 0)
                {
                    Config.PlayersNumber = 1;
                    StateManager.ChangeState(GameRef.GameplayScreen);
                }
                // 2 Players
                else if (_menuIndex == 1)
                {
                    Config.PlayersNumber = 2;
                    StateManager.ChangeState(GameRef.GameplayScreen);
                }
                // Improvements
                else if (_menuIndex == 2)
                    StateManager.ChangeState(GameRef.ImprovementScreen);
                // Leaderbord
                else if (_menuIndex == 3)
                    StateManager.ChangeState(GameRef.LeaderboardScreen);
                // Options
                else if (_menuIndex == 4)
                    StateManager.ChangeState(GameRef.OptionsScreen);
                // Exit
                else if (_menuIndex == 5)
                    Game.Exit();
            }

            if (_backgroundMainRectangle.X + _backgroundImage.Width <= 0)
                _backgroundMainRectangle.X = _backgroundRightRectangle.X + _backgroundImage.Width;

            if (_backgroundRightRectangle.X + _backgroundImage.Width <= 0)
                _backgroundRightRectangle.X = _backgroundMainRectangle.X + _backgroundImage.Width;

            /*
            if (_backgroundMainRectangle.Y + _backgroundImage.Height <= 0)
                _backgroundMainRectangle.Y = _backgroundTopRectangle.Y - _backgroundImage.Height;

            if (_backgroundTopRectangle.Y + _backgroundImage.Height <= 0)
                _backgroundTopRectangle.Y = _backgroundMainRectangle.Y - _backgroundImage.Height;
            */

            if (_backgroundMainRectangle.Y > Config.Resolution.Y)
                _backgroundMainRectangle.Y = _backgroundTopRectangle.Y - _backgroundImage.Height;

            if (_backgroundTopRectangle.Y > Config.Resolution.Y)
                _backgroundTopRectangle.Y = _backgroundMainRectangle.Y - _backgroundImage.Height;


            /*
            _backgroundMainRectangle.X -= 1;
            _backgroundRightRectangle.X -= 1;
            */

            if (!Config.Cheat)
            {
                if ((_passStep == 0 || _passStep == 1) && InputHandler.KeyPressed(Keys.Up))
                {
                    _passStep++;
                }
                else if ((_passStep == 2 || _passStep == 3) && InputHandler.KeyPressed(Keys.Down))
                {
                    _passStep++;
                }
                else if ((_passStep == 4 || _passStep == 6) && InputHandler.KeyPressed(Keys.Left))
                {
                    _passStep++;
                }
                else if ((_passStep == 5 || _passStep == 7) && InputHandler.KeyPressed(Keys.Right))
                {
                    _passStep++;
                }
                else if (_passStep == 8 && InputHandler.KeyPressed(Keys.B))
                {
                    _passStep++;
                }
                else if (_passStep == 9 && InputHandler.KeyPressed(Keys.A))
                {
                    _passStep++;
                }

                if (_passStep == 10)
                {
                    Config.Cheat = true;
                }
            }
            else
            {
                if (InputHandler.KeyDown(Keys.F9))
                    Config.Debug = !Config.Debug;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.Begin();


            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundMainRectangle, Color.White);
            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundRightRectangle, Color.White);
            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundTopRectangle, Color.White);
            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundTopRightRectangle, Color.White);

            GameRef.SpriteBatch.Draw(_logo, new Vector2(
                                                (GameRef.Graphics.GraphicsDevice.Viewport.Width / 2) - (_logo.Width / 2),
                                                100), Color.White);

            for (int i = 0; i < _menuText.Length; i++)
            {
                Color textColor = Color.White;

                if (i == _menuIndex)
                    textColor = Color.OrangeRed;

                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _menuText[i], new Vector2(
                  Game.GraphicsDevice.Viewport.Width / 2f - (ControlManager.SpriteFont.MeasureString(_menuText[i]).X / 2f) + 1,
                  Game.GraphicsDevice.Viewport.Height / 2f - 50 + (20 * i) + 1), Color.Black);
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _menuText[i], new Vector2(
                  Game.GraphicsDevice.Viewport.Width / 2f - (ControlManager.SpriteFont.MeasureString(_menuText[i]).X / 2f),
                  Game.GraphicsDevice.Viewport.Height / 2f - 50 + (20 * i)), textColor);
            }

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "[" + _menuDescription[_menuIndex] + "]", new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2f - (ControlManager.SpriteFont.MeasureString(_menuDescription[_menuIndex]).X / 2f) - 4 + 1,
                Game.GraphicsDevice.Viewport.Height - 60 + 1), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "[" + _menuDescription[_menuIndex] + "]", new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2f - (ControlManager.SpriteFont.MeasureString(_menuDescription[_menuIndex]).X / 2f) - 4,
                Game.GraphicsDevice.Viewport.Height - 60), Color.White);

            string credits = "Credits: " + PlayerData.Credits.ToString(CultureInfo.InvariantCulture);

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, credits, new Vector2(1, Game.GraphicsDevice.Viewport.Height - ControlManager.SpriteFont.MeasureString(credits).Y + 1), Color.Black);
            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, credits, new Vector2(0, Game.GraphicsDevice.Viewport.Height - ControlManager.SpriteFont.MeasureString(credits).Y), Color.White);

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);

        }

        #endregion
    }
}
