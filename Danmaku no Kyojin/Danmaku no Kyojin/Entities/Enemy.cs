using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    class Enemy : DrawableGameComponent
    {
        private DnK _gameRef;

        public Vector2 _position;

        private Texture2D _sprite;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Texture2D Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }

        public Enemy(DnK game)
            : base(game)
        {
            _gameRef = game;

            _position = Vector2.Zero;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _sprite = this.Game.Content.Load<Texture2D>("Graphics/Sprites/konata");

            _position = new Vector2(
                _gameRef.Graphics.GraphicsDevice.Viewport.Width / 2 - _sprite.Width / 2,
                20);

            base.LoadContent();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _gameRef.SpriteBatch.Begin();

            _gameRef.SpriteBatch.Draw(_sprite, new Rectangle((int)Position.X, (int)Position.Y, _sprite.Width, _sprite.Height), Color.White);
            _gameRef.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
