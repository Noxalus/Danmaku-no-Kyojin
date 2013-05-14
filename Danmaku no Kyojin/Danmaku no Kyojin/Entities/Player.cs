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
    public class Player : BulletLauncherEntity
    {
        #region Fields
        public int ID { get; set; }

        // Specific to the sprite
        private Texture2D _sprite;
        private Vector2 _center;
        private Texture2D _bulletSprite;

        private float _velocity;
        private Vector2 _direction;
        public bool SlowMode { get; set; }
        private float _velocitySlowMode;
        private float _rotation;
        private Vector2 _distance;

        // Bullet Time
        public bool BulletTime { get; set; }

        private int _lives;
        public bool IsInvincible { get; set; }
        private TimeSpan _invincibleTime;

        private TimeSpan _bulletFrequence;

        #endregion

        private Rectangle GetCollisionBox()
        {
            return new Rectangle(
                (int)(Position.X - _sprite.Width / 8 + ((_sprite.Width / 7.6f) * Math.Sin(_rotation)) * -1),
                (int)(Position.Y - _sprite.Height / 8 + ((_sprite.Height / 6.6f) * Math.Cos(_rotation))),
                _sprite.Width / 4, _sprite.Height / 4);
        }

        public Player(DnK game, int id, ref List<BaseBullet> bullets, Vector2 position)
            : base(game, ref bullets)
        {
            ID = id;
            Position = position;
            _velocity = Config.PlayerMaxVelocity;
            _velocitySlowMode = Config.PlayerMaxSlowVelocity;
            _direction = Vector2.Zero;
            _rotation = 0f;
            _center = Vector2.Zero;
            _distance = Vector2.Zero;

            _lives = 5;
            IsInvincible = false;
            _invincibleTime = Config.PlayerInvicibleTimer;

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
                    _invincibleTime = Config.PlayerInvicibleTimer;
                    IsInvincible = false;
                }
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _direction = Vector2.Zero;

            if (ID == 1)
            {
                // Keyboard
                if (InputHandler.KeyDown(Config.PlayerKeyboardInput[0]))
                    _direction.Y = -1;
                if (InputHandler.KeyDown(Config.PlayerKeyboardInput[1]))
                    _direction.X = 1;
                if (InputHandler.KeyDown(Config.PlayerKeyboardInput[2]))
                    _direction.Y = 1;
                if (InputHandler.KeyDown(Config.PlayerKeyboardInput[3]))
                    _direction.X = -1;

                SlowMode = (InputHandler.KeyDown(Config.PlayerKeyboardInput[4])) ? true : false;
                BulletTime = (InputHandler.MouseState.RightButton == ButtonState.Pressed) ? true : false;

                if (_direction.X != 0 && _direction.Y != 0)
                {
                    _velocitySlowMode = Config.PlayerMaxSlowVelocity / 2;
                    _velocity = Config.PlayerMaxVelocity / 2;
                }
                else
                {
                    _velocitySlowMode = Config.PlayerMaxSlowVelocity;
                    _velocity = Config.PlayerMaxVelocity;
                }

                // Mouse
                _distance.X = Position.X - InputHandler.MouseState.X;
                _distance.Y = Position.Y - InputHandler.MouseState.Y;

                _rotation = (float)Math.Atan2(_distance.Y, _distance.X) - MathHelper.PiOver2;

                if (InputHandler.MouseState.LeftButton == ButtonState.Pressed)
                {
                    Fire(gameTime);
                }
            }
            else if (ID == 2)
            {
                _direction.X = InputHandler.GamePadStates[0].ThumbSticks.Left.X;
                _direction.Y = (-1) * InputHandler.GamePadStates[0].ThumbSticks.Left.Y;

                SlowMode = (InputHandler.ButtonDown(Buttons.LeftTrigger, PlayerIndex.One)) ? true : false;
                BulletTime = (InputHandler.ButtonDown(Buttons.RightTrigger, PlayerIndex.One)) ? true : false;

                if (InputHandler.GamePadStates[0].ThumbSticks.Right.Length() > 0)
                {
                    _rotation =
                        (float)
                        Math.Atan2(InputHandler.GamePadStates[0].ThumbSticks.Right.Y * (-1),
                                   InputHandler.GamePadStates[0].ThumbSticks.Right.X) + MathHelper.PiOver2;

                    Fire(gameTime);
                }
            }

            UpdatePosition(dt);
        }

        private void UpdatePosition(float dt)
        {
            if (SlowMode)
            {
                Position.X += _direction.X * _velocitySlowMode * dt;
                Position.Y += _direction.Y * _velocitySlowMode * dt;
            }
            else
            {
                Position.X += _direction.X * _velocity * dt;
                Position.Y += _direction.Y * _velocity * dt;
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

            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(1, 61), Color.Black);
            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(0, 60), Color.White);

            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, InputHandler.GamePadStates[0].ThumbSticks.Right.Length().ToString(), new Vector2(1, 101), Color.Black);
            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, InputHandler.GamePadStates[0].ThumbSticks.Right.Length().ToString(), new Vector2(0, 100), Color.White);

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

        private void Fire(GameTime gameTime)
        {
            if (_bulletFrequence.TotalMilliseconds > 0)
                _bulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                _bulletFrequence = Config.PlayerBulletFrequence;

                Vector2 direction = new Vector2((float)Math.Sin(_rotation), (float)Math.Cos(_rotation) * -1);
                Bullet bullet = new Bullet(Game, _bulletSprite, Position, direction, _velocity * 3);
                bullet.Power = 1f;
                bullet.WaveMode = false;

                Vector2 directionLeft = direction;
                Vector2 positionLeft = Position;
                Vector2 directionRight = direction;
                Vector2 positionRight = Position;

                if (!SlowMode)
                {
                    directionLeft = new Vector2((float)Math.Sin(_rotation - Math.PI / 4),
                                                (float)Math.Cos(_rotation - Math.PI / 4) * -1);
                    directionRight = new Vector2((float)Math.Sin(_rotation + Math.PI / 4),
                                                 (float)Math.Cos(_rotation + Math.PI / 4) * -1);
                }
                else
                {
                    positionLeft.X -= 50f;
                    positionRight.X += 50f;
                }

                Bullet bulletLeft = new Bullet(Game, _bulletSprite, positionLeft, directionLeft, _velocity * 3);
                bulletLeft.Power = 0.5f;

                Bullet bulletRight = new Bullet(Game, _bulletSprite, positionRight, directionRight, _velocity * 3);
                bulletRight.Power = 0.5f;

                AddBullet(bullet);
                AddBullet(bulletLeft);
                AddBullet(bulletRight);
            }
        }
    }
}
