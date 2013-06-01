using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Screens
{
    public class GameOverScreen : BaseGameState
    {
        #region Field region

        private string _message;

        #endregion

        #region Constructor region

        public GameOverScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            
        }

        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            _message = "This functionnality is not implemented yet !";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyPressed(Keys.Escape))
                StateManager.ChangeState(GameRef.TitleScreen);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();

            base.Draw(gameTime);

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _message,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_message).X / 2, 
                    Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(_message).Y / 2), 
                Color.White);

            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.End();
        }

        #endregion
    }
}
