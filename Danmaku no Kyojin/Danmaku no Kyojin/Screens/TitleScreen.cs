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
        private string[] menuString;
        private int indexMenu;
        private Vector2 menuPosition;
        private bool enableMenu;

        #endregion

        #region Constructor region

        public TitleScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            menuString = new string[] {"Jouer", "Options", "Crédits", "Quitter"};
            indexMenu = 0;
            enableMenu = false;
        }

        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            menuPosition = new Vector2(Config.Resolution.X / 2, 3*Config.Resolution.Y / 4 - 40);
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

            if (InputHandler.KeyPressed(Keys.Enter))
            {
                StateManager.ChangeState(GameRef.GameplayScreen);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();

            base.Draw(gameTime);

            GameRef.SpriteBatch.Draw(_logo, new Vector2(
                                                (GameRef.Graphics.GraphicsDevice.Viewport.Width/2) - (_logo.Width/2),
                                                0), Color.White);

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, "[Press Enter to start]", new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2 - 125, Game.GraphicsDevice.Viewport.Height - 60), Color.Black);

            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.End();
        }

        #endregion
    }
}
