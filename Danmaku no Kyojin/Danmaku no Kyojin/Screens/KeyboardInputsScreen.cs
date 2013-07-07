using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Screens
{
    public class KeyboardInputsScreen : BaseGameState
    {
        #region Field region

        private string _title;
        private string[] _messages;

        private Texture2D _background;
        private SpriteFont _titleFont;

        #endregion

        #region Constructor region

        public KeyboardInputsScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _title = "Options";
            _messages = new string[]
                {
                    "This functionnality is not implemented yet !",
                    "[Press Escape to go back to the title screen]"
                };
        }

        #endregion

        #region XNA Method region

        public override void Initialize()
        {
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

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();

            GameRef.SpriteBatch.Draw(_background, new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y), Color.Yellow);

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

            for (int i = 0; i < _messages.Length; i++)
            {
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, _messages[i],
                new Vector2(
                    Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_messages[i]).X / 2,
                    Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(_messages[i]).Y / 2 + 20 * i),
                Color.White);
            }

            GameRef.SpriteBatch.End();

            ControlManager.Draw(GameRef.SpriteBatch);

            base.Draw(gameTime);
        }

        #endregion
    }
}
