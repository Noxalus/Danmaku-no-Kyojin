using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    class Enemy : Entity
    {
        private float _speed;

        private Texture2D _sprite;

        private Vector2 _motion;

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public Texture2D Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }

        public Enemy(DnK game)
            : base(game)
        {
            Position = Vector2.Zero;
            _motion = new Vector2(1, 0);
            _speed = 1;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _sprite = this.Game.Content.Load<Texture2D>("Graphics/Entities/enemy");

            Position = new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2 - _sprite.Width / 2,
                20);

            base.LoadContent();

        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds * 100;

            if (Position.X > Game.GraphicsDevice.Viewport.Width - _sprite.Width - (Speed * dt) ||
                Position.X < _sprite.Width + (Speed * dt))
                _motion *= -1;

            //Position += _motion * Speed * dt;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(_sprite, new Rectangle((int)Position.X, (int)Position.Y, _sprite.Width, _sprite.Height), Color.White);

            base.Draw(gameTime);
        }
    }
}
