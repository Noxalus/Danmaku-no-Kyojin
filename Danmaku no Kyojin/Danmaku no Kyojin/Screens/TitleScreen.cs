using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Screens
{
    public class TitleScreen : BaseGameState
    {
        #region Field region

        private Texture2D _logo;
        private string[] menuText;
        private string[] menuDescription;
        private int menuIndex;

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
            menuText = new string[] { "1 Player", "2 Players", "Improvements", "Leaderboard", "Options", "Exit" };
            menuDescription = new string[] { 
                "Playing game with only one player", 
                "Playing game with your best friend", 
                "Get new abilities to burst more enemies",
                "A list of scores to know who have the biggest one", 
                "You can change inputs here", 
                "Warning: I've never tested this button !", 
            };

            menuIndex = 0;
        }

        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            _backgroundMainRectangle = new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y);
            _backgroundRightRectangle = new Rectangle(Config.Resolution.X, 0, Config.Resolution.X, Config.Resolution.Y);
            _backgroundTopRectangle = new Rectangle(0, -Config.Resolution.Y, Config.Resolution.X, Config.Resolution.Y);
            _backgroundTopRightRectangle = new Rectangle(Config.Resolution.X, -Config.Resolution.Y, Config.Resolution.X, Config.Resolution.Y);

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
                menuIndex--;

                if (menuIndex < 0)
                    menuIndex = menuText.Length - 1;
            }

            if (InputHandler.PressedDown())
            {
                menuIndex = (menuIndex + 1) % menuText.Length;
            }

            if (InputHandler.KeyPressed(Keys.Enter))
            {
                // 1 Player
                if (menuIndex == 0)
                {
                    Config.PlayersNumber = 1;
                    StateManager.ChangeState(GameRef.GameplayScreen);
                }
                // 2 Players
                else if (menuIndex == 1)
                {
                    Config.PlayersNumber = 2;
                    StateManager.ChangeState(GameRef.GameplayScreen);
                }
                // Improvements
                else if (menuIndex == 2)
                    StateManager.ChangeState(GameRef.LeaderboardScreen);
                // Leaderbord
                else if (menuIndex == 3)
                    StateManager.ChangeState(GameRef.LeaderboardScreen);
                // Options
                else if (menuIndex == 3)
                    StateManager.ChangeState(GameRef.OptionsScreen);
                // Exit
                else if (menuIndex == 4)
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


            _backgroundMainRectangle.X -= 1;
            _backgroundRightRectangle.X -= 1;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();

            base.Draw(gameTime);

            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundMainRectangle, Color.White);
            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundRightRectangle, Color.White);
            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundTopRectangle, Color.White);
            GameRef.SpriteBatch.Draw(_backgroundImage, _backgroundTopRightRectangle, Color.White);

            GameRef.SpriteBatch.Draw(_logo, new Vector2(
                                                (GameRef.Graphics.GraphicsDevice.Viewport.Width / 2) - (_logo.Width / 2),
                                                0), Color.White);

            for (int i = 0; i < menuText.Length; i++)
            {
                Color textColor = Color.GreenYellow;

                if (i == menuIndex)
                    textColor = Color.OrangeRed;

                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, menuText[i], new Vector2(
                  Game.GraphicsDevice.Viewport.Width / 2f - (ControlManager.SpriteFont.MeasureString(menuText[i]).X / 2f), 
                  Game.GraphicsDevice.Viewport.Height / 2f - 50 + (20 * i)), textColor);
            }

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "[" + menuDescription[menuIndex] + "]", new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2f - (ControlManager.SpriteFont.MeasureString(menuDescription[menuIndex]).X / 2f) - 4, 
                Game.GraphicsDevice.Viewport.Height - 60), Color.GreenYellow);

            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.End();
        }

        #endregion
    }
}
