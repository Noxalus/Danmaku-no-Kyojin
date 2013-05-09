using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    class Boss : Entity
    {
        private float _speed;

        private Texture2D _sprite;

        private Vector2 _motion;

        private const float TotalHealth = 500f;
        private float _health;
        private Texture2D _healthBar;

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

        private Rectangle GetCollisionBox()
        {
            return new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                _sprite.Width, _sprite.Height);
        }

        public Boss(DnK game)
            : base(game)
        {
            Position = Vector2.Zero;
            _motion = new Vector2(1, 0);
            _speed = 1;

            _health = TotalHealth;

            IsAlive = true;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _sprite = Game.Content.Load<Texture2D>("Graphics/Entities/enemy");
            _healthBar = Game.Content.Load<Texture2D>("Graphics/Pictures/pixel");

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

            if (_health <= 0)
            {
                IsAlive = false;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(_sprite, new Rectangle((int)Position.X, (int)Position.Y, _sprite.Width, _sprite.Height), Color.White);
            Game.SpriteBatch.Draw(_healthBar, new Rectangle(
                (int)Position.X, (int)Position.Y + _sprite.Height + 20, 
                (int)(100f * (_health / TotalHealth)), 10), Color.Blue);

            base.Draw(gameTime);
        }

        public bool CheckCollision(Vector2 position, Point size)
        {
            Rectangle bullet = new Rectangle(
                    (int)position.X - size.X / 2, (int)position.Y - size.Y / 2,
                    size.X, size.Y);

            Rectangle collisionBox = GetCollisionBox();

            if (collisionBox.Intersects(bullet))
            {
                return true;
            }

            return false;
        }
        
        public void TakeDamage(float damage)
        {
            _health -= damage;
        }
    }
}
