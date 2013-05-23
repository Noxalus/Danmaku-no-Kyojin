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
        private Vector2 menuPosition;
        private bool enableMenu;

        #endregion

        #region Constructor region

        public TitleScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            menuText = new string[] { "1 Player", "2 Players", "Leaderboard", "Options", "Exit" };
            menuDescription = new string[] { 
                "Playing game with only one player", 
                "Playing game with your best friend", 
                "A list of scores to know who have the biggest one", 
                "You can change inputs here", 
                "Warning: I've never tested this button !", 
            };
            menuIndex = 0;
            enableMenu = false;
        }

        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            menuPosition = new Vector2(Config.Resolution.X / 2, 3 * Config.Resolution.Y / 4 - 40);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _logo = GameRef.Content.Load<Texture2D>(@"Graphics/Pictures/logo");

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
                // Leaderbord
                else if (menuIndex == 2)
                    StateManager.ChangeState(GameRef.LeaderboardScreen);
                // Options
                else if (menuIndex == 3)
                    StateManager.ChangeState(GameRef.OptionsScreen);
                // Exit
                else if (menuIndex == 4)
                    Game.Exit();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();

            base.Draw(gameTime);

            GameRef.SpriteBatch.Draw(_logo, new Vector2(
                                                (GameRef.Graphics.GraphicsDevice.Viewport.Width / 2) - (_logo.Width / 2),
                                                0), Color.White);

            for (int i = 0; i < menuText.Length; i++)
            {
                Color textColor = Color.Black;

                if (i == menuIndex)
                    textColor = Color.Red;

                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, menuText[i], new Vector2(
                  Game.GraphicsDevice.Viewport.Width / 2 - (ControlManager.SpriteFont.MeasureString(menuText[i]).X / 2f), 
                  Game.GraphicsDevice.Viewport.Height / 2 - 50 + (20 * i)), textColor);
            }

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "[" + menuDescription[menuIndex] + "]", new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2 - (ControlManager.SpriteFont.MeasureString(menuDescription[menuIndex]).X / 2f) - 4, Game.GraphicsDevice.Viewport.Height - 60), Color.Black);

            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.End();
        }

        #endregion
    }
}
