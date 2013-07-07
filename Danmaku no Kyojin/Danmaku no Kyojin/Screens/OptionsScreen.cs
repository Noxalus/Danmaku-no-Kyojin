﻿using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Screens
{
    public class GamepadInputsScreen : BaseGameState
    {
        #region Field region

        private string _title;
        private readonly string[] _menuText;
        private int _menuIndex;

        private Texture2D _background;
        private SpriteFont _titleFont;

        #endregion

        #region Constructor region

        public GamepadInputsScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            _title = "Options";
            _menuText = new string[]
                {
                    "1P Control",
                    "2P Control",
                    "Sound volume",
                    "Music volume"
                };
        }

        #endregion

        #region XNA Method region

        public override void Initialize()
        {
            base.Initialize();

            _menuIndex = 0;
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

            GameRef.SpriteBatch.End();

            ControlManager.Draw(GameRef.SpriteBatch);

            base.Draw(gameTime);
        }

        #endregion
    }
}
