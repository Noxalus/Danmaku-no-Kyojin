using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Screens
{
    public class GameplayScreen : BaseGameState
    {
        private Texture2D _logo;
        private Ship _ship;

        public GameplayScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
        }

        public override void Initialize()
        {
            _ship = new Ship(GameRef, new Vector2(GameRef.Graphics.GraphicsDevice.Viewport.Width / 2, GameRef.Graphics.GraphicsDevice.Viewport.Height - 150));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _logo = GameRef.Content.Load<Texture2D>("Graphics/Pictures/logo");
            _ship.LoadContent(GameRef.Content);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            _ship.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            ControlManager.Draw(GameRef.SpriteBatch);

            GameRef.SpriteBatch.Begin();

            GameRef.SpriteBatch.Draw(_logo, new Vector2((GameRef.Graphics.GraphicsDevice.Viewport.Width / 2) - (_logo.Width / 2), 0), Color.White);
            _ship.Draw();

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
