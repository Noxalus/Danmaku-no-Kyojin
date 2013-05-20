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

        private Texture2D _bulletSprite;

        private float _velocity;
        private Vector2 _direction;
        public bool SlowMode { get; set; }
        private float _velocitySlowMode;
        private Vector2 _distance;

        // Bullet Time
        public bool BulletTime { get; set; }

        private int _lives;
        public bool IsInvincible { get; set; }
        private TimeSpan _invincibleTime;

        private TimeSpan _bulletFrequence;

        #endregion

        public Player(DnK game, int id, ref List<BaseBullet> bullets, Vector2 position)
            : base(game, ref bullets)
        {
            ID = id;
            Position = position;
            _velocity = Config.PlayerMaxVelocity;
            _velocitySlowMode = Config.PlayerMaxSlowVelocity;
            _direction = Vector2.Zero;
            Rotation = 0f;
            Center = Vector2.Zero;
            _distance = Vector2.Zero;

            _lives = Config.PlayerLives;
            IsInvincible = false;
            _invincibleTime = Config.PlayerInvicibleTimer;

            BulletTime = false;

            _bulletFrequence = new TimeSpan(0);

            IsAlive = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Sprite = this.Game.Content.Load<Texture2D>("Graphics/Entities/ship2");
            _bulletSprite = this.Game.Content.Load<Texture2D>("Graphics/Entities/ship_bullet");
            Center = new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);
            CollisionBox = new CollisionCircle(this, new Vector2(Sprite.Height / 6f, Sprite.Height / 6f), 20/*(float)Math.PI*/);
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

                Rotation = (float)Math.Atan2(_distance.Y, _distance.X) - MathHelper.PiOver2;

                // Debug
                if (InputHandler.KeyDown(Keys.R))
                    Rotation = 0;

                if (InputHandler.MouseState.LeftButton == ButtonState.Pressed)
                {
                    Fire(gameTime);
                }
            }
            else if (ID == 2)
            {
                _direction.X = InputHandler.GamePadStates[0].ThumbSticks.Left.X;
                _direction.Y = (-1) * InputHandler.GamePadStates[0].ThumbSticks.Left.Y;

                SlowMode = (InputHandler.ButtonDown(Config.PlayerGamepadInput[0], PlayerIndex.One)) ? true : false;
                BulletTime = (InputHandler.ButtonDown(Config.PlayerGamepadInput[1], PlayerIndex.One)) ? true : false;

                if (InputHandler.GamePadStates[0].ThumbSticks.Right.Length() > 0)
                {
                    Rotation =
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

            Position.X = MathHelper.Clamp(Position.X, Sprite.Width / 2f, Config.Resolution.X - Sprite.Width / 2f);
            Position.Y = MathHelper.Clamp(Position.Y, Sprite.Height / 2f, Config.Resolution.Y - Sprite.Height / 2f);
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsInvincible)
                Game.Graphics.GraphicsDevice.Clear(Color.Red);

            Game.SpriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Center, 1f, SpriteEffects.None, 0f);

            //Game.SpriteBatch.DrawString(ControlManager.SpriteFont, Rotation + " (X" + Math.Cos(Rotation) + "|Y: " + Math.Sin(Rotation) + ")", new Vector2(0, 300), Color.Black);
            //Game.SpriteBatch.DrawString(ControlManager.SpriteFont, _distance.ToString(), new Vector2(0, 20), Color.Black);

            // Text
            string lives = string.Format("P{0}'s lives: {1}", ID, _lives);

            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(1, 61 + 20 * (ID - 1)), Color.Black);
            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(0, 60 + 20 * (ID - 1)), Color.White);

            base.Draw(gameTime);
        }

        private void Fire(GameTime gameTime)
        {
            if (_bulletFrequence.TotalMilliseconds > 0)
                _bulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                _bulletFrequence = Config.PlayerBulletFrequence;

                Vector2 direction = new Vector2((float)Math.Sin(Rotation), (float)Math.Cos(Rotation) * -1);
                Bullet bullet = new Bullet(Game, _bulletSprite, Position, direction, Config.PlayerBulletVelocity);
                bullet.Power = 1f;
                bullet.WaveMode = false;

                Vector2 directionLeft = direction;
                Vector2 positionLeft = Position;
                Vector2 directionRight = direction;
                Vector2 positionRight = Position;

                if (!SlowMode)
                {
                    directionLeft = new Vector2((float)Math.Sin(Rotation - Math.PI / 4),
                                                (float)Math.Cos(Rotation - Math.PI / 4) * -1);
                    directionRight = new Vector2((float)Math.Sin(Rotation + Math.PI / 4),
                                                 (float)Math.Cos(Rotation + Math.PI / 4) * -1);
                }
                else
                {
                    positionLeft.X -= 50f;
                    positionRight.X += 50f;
                }

                Bullet bulletLeft = new Bullet(Game, _bulletSprite, positionLeft, directionLeft, Config.PlayerBulletVelocity);
                bulletLeft.Power = 0.5f;

                Bullet bulletRight = new Bullet(Game, _bulletSprite, positionRight, directionRight, Config.PlayerBulletVelocity);
                bulletRight.Power = 0.5f;

                AddBullet(bullet);
                AddBullet(bulletLeft);
                AddBullet(bulletRight);
            }
        }

        public void Hit()
        {
            _lives--;

            IsInvincible = true;
        }
    }
}
