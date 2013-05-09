using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Danmaku_no_Kyojin.Controls;
using Microsoft.Xna.Framework.Input;
using Danmaku_no_Kyojin.Collisions;
using System.Diagnostics;

namespace Danmaku_no_Kyojin.Entities
{
    public class Ship : BulletLauncherEntity
    {
        #region Fields

        // Specific to the sprite
        private Texture2D _sprite;
        private Vector2 _center;
        private Texture2D _bulletSprite;

        private float _velocity;
        public bool SlowMode { get; set; }
        private float _velocitySlowMode;
        private float _rotation;
        private Vector2 _distance;

        // Bullet Time
        public bool BulletTime { get; set; }

        private int _lives;
        public bool IsInvincible { get; set; }
        private TimeSpan _invincibleTime;
        private TimeSpan _invicibleMaxTime;

        private TimeSpan _bulletFrequence;

        #endregion
        
        private Rectangle GetCollisionBox()
        {
            return new Rectangle(
                (int)Position.X - _sprite.Width / 8,
                (int)Position.Y - _sprite.Height / 8, 
                _sprite.Width / 4, _sprite.Height / 4);
        }

        public Ship(DnK game, ref List<BaseBullet> bullets, Vector2 position)
            : base(game, ref bullets)
        {
            Position = position;
            _velocity = 400f;
            _velocitySlowMode = 125f;
            _rotation = 0f;
            _center = Vector2.Zero;
            _distance = Vector2.Zero;

            _lives = 5;
            IsInvincible = false;
            _invicibleMaxTime = new TimeSpan(5 * 10000000);
            _invincibleTime = _invicibleMaxTime;

            BulletTime = false;

            _bulletFrequence = new TimeSpan(0);

            IsAlive = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _sprite = this.Game.Content.Load<Texture2D>("Graphics/Entities/ship2");
            _bulletSprite = this.Game.Content.Load<Texture2D>("Graphics/Entities/ship_bullet");
            _center = new Vector2(_sprite.Width / 2, _sprite.Height / 2);
            BoundingElement = new CollisionCircle((DnK)this.Game, this, 20);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_lives <= 0)
                IsAlive = false;

            if (IsInvincible)
            {
                _invincibleTime -= gameTime.ElapsedGameTime;

                if (_invincibleTime.Seconds <= 0)
                {
                    _invincibleTime = _invicibleMaxTime;
                    IsInvincible = false;
                }
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 motion = Vector2.Zero;

            // Keyboard
            if (InputHandler.KeyDown(Keys.D))
                motion.X = 1;
            if (InputHandler.KeyDown(Keys.Q))
                motion.X = -1;
            if (InputHandler.KeyDown(Keys.Z))
                motion.Y = -1;
            if (InputHandler.KeyDown(Keys.S))
                motion.Y = 1;

            SlowMode = (InputHandler.KeyDown(Keys.LeftShift)) ? true : false;

            BulletTime = (InputHandler.MouseState.RightButton == ButtonState.Pressed) ? true : false;

            // Mouse
            _distance.X = Position.X - InputHandler.MouseState.X;
            _distance.Y = Position.Y - InputHandler.MouseState.Y;

            _rotation = (float)Math.Atan2(_distance.Y, _distance.X) - MathHelper.PiOver2;


            if (_bulletFrequence.TotalMilliseconds > 0)
                _bulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                if (InputHandler.MouseState.LeftButton == ButtonState.Pressed)
                {
                    _bulletFrequence = Config.PlayerBulletFrequence;

                    Vector2 direction = new Vector2((float) Math.Sin(_rotation), (float) Math.Cos(_rotation)*-1);
                    Bullet bullet = new Bullet(Game, _bulletSprite, Position, direction, _velocity * 3);
                    bullet.Power = 0.1f;
                    AddBullet(bullet);
                }
            }

            UpdatePosition(motion, dt);
        }

        private void UpdatePosition(Vector2 motion, float dt)
        {
            if (SlowMode)
            {
                Position.X += motion.X * _velocitySlowMode * dt;
                Position.Y += motion.Y * _velocitySlowMode * dt;
            }
            else
            {
                Position.X += motion.X * _velocity * dt;
                Position.Y += motion.Y * _velocity * dt;
            }

            _center.X = _sprite.Width / 2;
            _center.Y = _sprite.Height / 2;
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsInvincible)
                Game.Graphics.GraphicsDevice.Clear(Color.Red);

            Game.SpriteBatch.Draw(_sprite, Position, null, Color.White, _rotation, _center, 1f, SpriteEffects.None, 0f);
            
            if (Config.DisplayCollisionBoxes)
                Game.SpriteBatch.Draw(DnK._pixel, GetCollisionBox(), Color.White);

            //_boundingElement.DrawDebug(_position, _rotation, new Vector2(_sprite.Width, _sprite.Height));
            //Game.SpriteBatch.DrawString(ControlManager.SpriteFont, _rotation + " (X" + Math.Cos(_rotation) + "|Y: " + Math.Sin(_rotation) + ")", new Vector2(0, 300), Color.Black);
            //Game.SpriteBatch.DrawString(ControlManager.SpriteFont, _distance.ToString(), new Vector2(0, 20), Color.Black);

            string lives = string.Format("Lives: {0}", _lives);

            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(1, 41), Color.Black);
            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(0, 40), Color.White);

            base.Draw(gameTime);
        }

        public bool CheckCollision(Vector2 position, Point size)
        {
            Rectangle bullet = new Rectangle(
                    (int)position.X - size.X / 2, (int)position.Y - size.Y / 2,
                    size.X, size.Y);

            Rectangle ship = GetCollisionBox();

            if (ship.Intersects(bullet))
            {
                Debug.Print("Collision !");
                _lives--;

                IsInvincible = true;

                return true;
            }

            return false;
        }
    }
}
