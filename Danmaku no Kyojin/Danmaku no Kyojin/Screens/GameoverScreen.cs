using System.Globalization;
using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Screens
{
    public class GameOverScreen : BaseGameState
    {
        #region Field region

        private string _message;
        public int WaveNumber { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }

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
            _message = "GAME OVER";

            /*
            WaveNumber = 0;
            Player1Score = 0;
            Player2Score = 0;
            */
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyPressed(Keys.Escape) || InputHandler.KeyPressed(Keys.Enter))
                StateManager.ChangeState(GameRef.TitleScreen);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.Begin();

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _message,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_message).X / 2, 
                    Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(_message).Y / 2), 
                Color.White);

            // Scores
            string waveNumber = "Wave number: " + WaveNumber.ToString(CultureInfo.InvariantCulture);
            string player1Score = "P1 Score: " + Player1Score.ToString(CultureInfo.InvariantCulture);
            string player2Score = "P2 Score: " + Player2Score.ToString(CultureInfo.InvariantCulture);
            string totalScore = "Total score: " + (Player1Score + Player2Score + Improvements.ScoreByEnemyData[PlayerData.ScoreByEnemyIndex].Key * WaveNumber).ToString(CultureInfo.InvariantCulture);

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, waveNumber,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(waveNumber).X / 2,
                    Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(waveNumber).Y / 2 + 20),
                Color.White);

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, player1Score,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(player1Score).X / 2,
                    Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(player1Score).Y / 2 + 40),
                Color.White);

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, player2Score,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(player2Score).X / 2,
                    Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(player2Score).Y / 2 + 60),
                Color.White);

            GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, totalScore,
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(totalScore).X / 2,
                    Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(totalScore).Y / 2 + 80),
                Color.White);

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
