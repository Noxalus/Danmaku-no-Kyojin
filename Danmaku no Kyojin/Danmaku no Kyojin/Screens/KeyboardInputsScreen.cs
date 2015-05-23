using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Danmaku_no_Kyojin.Screens
{
    public class KeyboardInputsScreen : BaseGameState
    {
        #region Field region

        private string _title;

        private Texture2D _background;
        private SpriteFont _titleFont;

        private int _menuIndex;

        #endregion

        #region Constructor region

        public KeyboardInputsScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _title = "Keyboard";
        }

        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            _menuIndex = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _background = GameRef.Content.Load<Texture2D>("Graphics/Pictures/background");
            _titleFont = Game.Content.Load<SpriteFont>("Graphics/Fonts/TitleFont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.PressedCancel())
                StateManager.ChangeState(GameRef.OptionsScreen);

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

            int i = 0;
            foreach (KeyValuePair<string, Keys> pair in Config.PlayerKeyboardInputs)
            {
                Color textColor = Color.White;

                if (i == _menuIndex)
                    textColor = Color.OrangeRed;

                string text = pair.Key + ": " + pair.Value;

                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, text,
                    new Vector2(
                        Game.GraphicsDevice.Viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(text).X / 2,
                        Game.GraphicsDevice.Viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(text).Y / 2 + 50 * i - 50),
                    textColor);

                i++;
            }

            GameRef.SpriteBatch.End();

            ControlManager.Draw(GameRef.SpriteBatch);

            base.Draw(gameTime);
        }

        #endregion
    }
}
