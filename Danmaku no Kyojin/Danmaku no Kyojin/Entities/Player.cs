using System.Globalization;
using Microsoft.Xna.Framework.Audio;
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
using Danmaku_no_Kyojin.Camera;

namespace Danmaku_no_Kyojin.Entities
{
    public class Player : BulletLauncherEntity
    {
        #region Fields
        public int ID { get; set; }
        private Config.Controller _controller;

        private Viewport _viewport;

        private Texture2D _bulletSprite;
        private Texture2D _hitboxSprite;

        private float _hitboxRadius;

        private Vector2 _originPosition;
        private float _velocity;
        private Vector2 _direction;
        public bool SlowMode { get; set; }
        private float _velocitySlowMode;
        private Vector2 _distance;

        // Bullet Time
        public bool BulletTime { get; set; }
        private Texture2D _bulletTimeBarLeft;
        private Texture2D _bulletTimeBarContent;
        private Texture2D _bulletTimeBarRight;
        private TimeSpan _bulletTimeTimer;
        private bool _bulletTimeReloading;

        private int _lives;
        public bool IsInvincible { get; set; }
        private TimeSpan _invincibleTime;

        // HUD
        private int _score;
        private Texture2D _lifeIcon;

        // Audio
        private SoundEffect _shootSound = null;
        private SoundEffect _deadSound = null;

        // Camera
        private Camera2D _camera;

        public int Score
        {
            get { return _score; }
        }

        public Camera2D Camera
        {
            get { return _camera; }
        }

        #endregion

        public Player(DnK game, Viewport viewport, int id, Config.Controller controller, Vector2 position)
            : base(game)
        {
            _viewport = viewport;
            ID = id;
            _controller = controller;
            _originPosition = position;
            Position = _originPosition;
            Center = Vector2.Zero;
        }

        public override void Initialize()
        {
            _velocity = (float)(Config.PlayerMaxVelocity * Improvements.SpeedData[PlayerData.SpeedIndex].Key);
            _velocitySlowMode = Config.PlayerMaxSlowVelocity;
            _direction = Vector2.Zero;
            Rotation = 0f;
            _distance = Vector2.Zero;

            _lives = Improvements.LivesNumberData[PlayerData.LivesNumberIndex].Key;

            IsInvincible = false;
            _invincibleTime = Improvements.InvicibleTimeData[PlayerData.InvicibleTimeIndex].Key;

            BulletTime = false;

            BulletFrequence = new TimeSpan(0);

            IsAlive = true;

            _score = 0;

            _bulletTimeTimer = Config.DefaultBulletTimeTimer;

            _hitboxRadius = (float)Math.PI * 1.5f * 2;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Sprite = Game.Content.Load<Texture2D>("Graphics/Entities/ship");
            _bulletSprite = this.Game.Content.Load<Texture2D>("Graphics/Entities/ship_bullet");
            _hitboxSprite = this.Game.Content.Load<Texture2D>("Graphics/Pictures/hitbox");
            Center = new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);
            CollisionBox = new CollisionCircle(this, new Vector2(Sprite.Height / 6f, Sprite.Height / 6f), _hitboxRadius/2f);

            _lifeIcon = Game.Content.Load<Texture2D>("Graphics/Pictures/life");

            _bulletTimeBarLeft = Game.Content.Load<Texture2D>("Graphics/Pictures/bulletTimeBarLeft");
            _bulletTimeBarContent = Game.Content.Load<Texture2D>("Graphics/Pictures/bulletTimeBarContent");
            _bulletTimeBarRight = Game.Content.Load<Texture2D>("Graphics/Pictures/bulletTimeBarRight");

            if (_shootSound == null)
                _shootSound = Game.Content.Load<SoundEffect>(@"Audio/SE/hit");
            if (_deadSound == null)
                _deadSound = Game.Content.Load<SoundEffect>(@"Audio/SE/dead");

            _camera = new Camera2D(_viewport, 1f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_lives <= 0)
                IsAlive = false;

            if (IsInvincible)
            {
                _invincibleTime -= gameTime.ElapsedGameTime;

                if (_invincibleTime.Milliseconds <= 0)
                {
                    _invincibleTime = Config.PlayerInvicibleTimer;
                    Position = _originPosition;
                    IsInvincible = false;
                }
            }
            else
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                _direction = Vector2.Zero;

                if (_controller == Config.Controller.Keyboard)
                {
                    // Keyboard
                    if (InputHandler.KeyDown(Config.PlayerKeyboardInputs["Up"]))
                        _direction.Y = -1;
                    if (InputHandler.KeyDown(Config.PlayerKeyboardInputs["Right"]))
                        _direction.X = 1;
                    if (InputHandler.KeyDown(Config.PlayerKeyboardInputs["Down"]))
                        _direction.Y = 1;
                    if (InputHandler.KeyDown(Config.PlayerKeyboardInputs["Left"]))
                        _direction.X = -1;

                    SlowMode = (PlayerData.SlowModeEnabled && (InputHandler.KeyDown(Config.PlayerKeyboardInputs["Slow"]))) ? true : false;
                    BulletTime = (PlayerData.BulletTimeEnabled && (!_bulletTimeReloading && InputHandler.MouseState.RightButton == ButtonState.Pressed)) ? true : false;

                    if (_direction != Vector2.Zero)
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
                else if (_controller == Config.Controller.GamePad)
                {
                    _direction.X = InputHandler.GamePadStates[0].ThumbSticks.Left.X;
                    _direction.Y = (-1) * InputHandler.GamePadStates[0].ThumbSticks.Left.Y;

                    SlowMode = (PlayerData.SlowModeEnabled && (InputHandler.ButtonDown(Config.PlayerGamepadInput[0], PlayerIndex.One))) ? true : false;
                    BulletTime = (PlayerData.BulletTimeEnabled && (!_bulletTimeReloading && InputHandler.ButtonDown(Config.PlayerGamepadInput[1], PlayerIndex.One))) ? true : false;

                    if (InputHandler.GamePadStates[0].ThumbSticks.Right.Length() > 0)
                    {
                        Rotation =
                            (float)
                            Math.Atan2(InputHandler.GamePadStates[0].ThumbSticks.Right.Y * (-1),
                                       InputHandler.GamePadStates[0].ThumbSticks.Right.X) + MathHelper.PiOver2;

                        Fire(gameTime);
                    }
                }

                if (BulletTime)
                {
                    _bulletTimeTimer -= gameTime.ElapsedGameTime;

                    if (_bulletTimeTimer <= TimeSpan.Zero)
                    {
                        _bulletTimeReloading = true;
                        _bulletTimeTimer = TimeSpan.Zero;
                    }

                }

                if (_bulletTimeReloading)
                {
                    _bulletTimeTimer += gameTime.ElapsedGameTime;

                    if (_bulletTimeTimer >= Config.DefaultBulletTimeTimer)
                    {
                        _bulletTimeReloading = false;
                        _bulletTimeTimer = Config.DefaultBulletTimeTimer;
                    }
                }

                UpdatePosition(dt);
            }

            _camera.Update(this.Position);
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

            Position.X = MathHelper.Clamp(Position.X, Sprite.Width / 2f, Config.GameArea.X - Sprite.Width / 2f);
            Position.Y = MathHelper.Clamp(Position.Y, Sprite.Height / 2f, Config.GameArea.Y - Sprite.Height / 2f);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsInvincible)
            {
                Game.SpriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Center, 1f, SpriteEffects.None, 0f);

                // Draw Hitbox
                if (SlowMode)
                {
                    Game.SpriteBatch.Draw(_hitboxSprite, new Rectangle(
                        (int)(CollisionBox.GetCenter().X - _hitboxRadius / 2f),
                        (int)(CollisionBox.GetCenter().Y - _hitboxRadius / 2f),
                        (int)_hitboxRadius,
                        (int)_hitboxRadius),
                        Color.White);
                }
            }

            base.Draw(gameTime);
        }

        public void DrawString(GameTime gameTime)
        {
            // Text
            string lives = string.Format("P{0}", ID);
            string score = string.Format("{0:000000000000}", _score);

            if (ID == 1)
            {
                // Lives
                int hudY = 40;

                if (PlayerData.BulletTimeEnabled)
                    hudY = 80;

                Game.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(1, Config.Resolution.Y - hudY + 1), Color.Black);
                Game.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(0, Config.Resolution.Y - hudY), Color.White);

                for (int i = 0; i < _lives; i++)
                {
                    Game.SpriteBatch.Draw(_lifeIcon, new Vector2(
                        ControlManager.SpriteFont.MeasureString(lives).X + i * _lifeIcon.Width + 10, Config.Resolution.Y - (hudY - 7)), Color.White);
                }

                // Bullet time bar
                if (PlayerData.BulletTimeEnabled)
                {
                    int bulletTimeBarWidth =
                        (int)
                        (100 * (float)(_bulletTimeTimer.TotalMilliseconds / Config.DefaultBulletTimeTimer.TotalMilliseconds));

                    // Text
                    Game.SpriteBatch.DrawString(ControlManager.SpriteFont,
                                                bulletTimeBarWidth.ToString(CultureInfo.InvariantCulture),
                                                new Vector2(1, Config.Resolution.Y - 39), Color.Black);
                    Game.SpriteBatch.DrawString(ControlManager.SpriteFont,
                                                bulletTimeBarWidth.ToString(CultureInfo.InvariantCulture),
                                                new Vector2(0, Config.Resolution.Y - 40), Color.White);

                    // Bar
                    Game.SpriteBatch.Draw(_bulletTimeBarLeft,
                                          new Rectangle(0, Config.Resolution.Y - 50, _bulletTimeBarLeft.Width, _bulletTimeBarLeft.Height),
                                          Color.White);
                    Game.SpriteBatch.Draw(_bulletTimeBarContent,
                                          new Rectangle(_bulletTimeBarLeft.Width, Config.Resolution.Y - 50, bulletTimeBarWidth,
                                                        _bulletTimeBarContent.Height), Color.White);
                    Game.SpriteBatch.Draw(_bulletTimeBarRight,
                                          new Rectangle(_bulletTimeBarLeft.Width + bulletTimeBarWidth, Config.Resolution.Y - 50,
                                                        _bulletTimeBarRight.Width, _bulletTimeBarRight.Height),
                                          Color.White);
                }

                // Score
                Game.SpriteBatch.DrawString(ControlManager.SpriteFont, score, new Vector2(1, Config.Resolution.Y - 20), Color.Black);
                Game.SpriteBatch.DrawString(ControlManager.SpriteFont, score, new Vector2(0, Config.Resolution.Y - 21), Color.White);
            }
            else if (ID == 2)
            {
                // Lives
                int hudY = 40;

                if (PlayerData.BulletTimeEnabled)
                    hudY = 80;

                Game.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(
                    Config.Resolution.X - ControlManager.SpriteFont.MeasureString(lives).X + 1, 
                    Config.Resolution.Y - hudY + 1), 
                Color.Black);
                Game.SpriteBatch.DrawString(ControlManager.SpriteFont, lives, new Vector2(
                    Config.Resolution.X - ControlManager.SpriteFont.MeasureString(lives).X, 
                    Config.Resolution.Y - hudY), 
                Color.White);

                for (int i = 0; i < _lives; i++)
                {
                    Game.SpriteBatch.Draw(_lifeIcon, new Vector2(
                        Config.Resolution.X - (ControlManager.SpriteFont.MeasureString(lives).X * 2) - i * _lifeIcon.Width + 10, 
                        Config.Resolution.Y - (hudY - 7)), 
                    Color.White);
                }

                // Bullet time bar
                if (PlayerData.BulletTimeEnabled)
                {
                    int bulletTimeBarWidth =
                        (int)
                        (100 * (float)(_bulletTimeTimer.TotalMilliseconds / Config.DefaultBulletTimeTimer.TotalMilliseconds));

                    // Text
                    Game.SpriteBatch.DrawString(ControlManager.SpriteFont,
                                                bulletTimeBarWidth.ToString(CultureInfo.InvariantCulture), new Vector2(
                                                    Config.Resolution.X - ControlManager.SpriteFont.MeasureString("100").X, 
                                                    Config.Resolution.Y - 39), 
                                                Color.Black);
                    Game.SpriteBatch.DrawString(ControlManager.SpriteFont,
                                                bulletTimeBarWidth.ToString(CultureInfo.InvariantCulture), new Vector2(
                                                    Config.Resolution.X - ControlManager.SpriteFont.MeasureString("100").X,
                                                    Config.Resolution.Y - 40), 
                                                Color.White);

                    // Bar
                    Game.SpriteBatch.Draw(_bulletTimeBarLeft,
                                          new Rectangle(Config.Resolution.X - bulletTimeBarWidth - _bulletTimeBarRight.Width - _bulletTimeBarLeft.Width, Config.Resolution.Y - 50, _bulletTimeBarLeft.Width, _bulletTimeBarLeft.Height),
                                          Color.White);
                    Game.SpriteBatch.Draw(_bulletTimeBarContent,
                                          new Rectangle(Config.Resolution.X - bulletTimeBarWidth - _bulletTimeBarRight.Width, Config.Resolution.Y - 50, bulletTimeBarWidth,
                                                        _bulletTimeBarContent.Height), Color.White);
                    Game.SpriteBatch.Draw(_bulletTimeBarRight,
                                          new Rectangle(Config.Resolution.X - _bulletTimeBarRight.Width, Config.Resolution.Y - 50,
                                                        _bulletTimeBarRight.Width, _bulletTimeBarRight.Height),
                                          Color.White);
                }


                // Score
                Game.SpriteBatch.DrawString(ControlManager.SpriteFont, score, new Vector2(
                    Config.Resolution.X - ControlManager.SpriteFont.MeasureString("000000000000").X + 1,
                    Config.Resolution.Y - 20),
                Color.Black);
                Game.SpriteBatch.DrawString(ControlManager.SpriteFont, score, new Vector2(
                    Config.Resolution.X - ControlManager.SpriteFont.MeasureString("000000000000").X,
                    Config.Resolution.Y - 21),
                Color.White);
            }
        }

        private void Fire(GameTime gameTime)
        {
            if (BulletFrequence.TotalMilliseconds > 0)
                BulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                BulletFrequence = TimeSpan.FromTicks((long)(Improvements.ShootFrequencyData[PlayerData.ShootFrequencyIndex].Key * Config.PlayerShootFrequency.Ticks));

                var direction = new Vector2((float)Math.Sin(Rotation), (float)Math.Cos(Rotation) * -1);

                // Straight
                if (PlayerData.ShootTypeIndex != 1)
                {
                    var bullet = new Bullet(Game, _bulletSprite, Position, direction, Config.PlayerBulletVelocity);
                    bullet.WaveMode = false;

                    AddBullet(bullet);
                }

                // Front sides 1/2 diagonal
                if (PlayerData.ShootTypeIndex > 0)
                {
                    Vector2 directionLeft = direction;
                    Vector2 positionLeft = new Vector2(Position.X - 25f * (float)Math.Cos(Rotation), Position.Y - 25f * (float)Math.Sin(Rotation));
                    Vector2 directionRight = direction;
                    Vector2 positionRight = new Vector2(Position.X + 25f * (float)Math.Cos(Rotation), Position.Y + 25f * (float)Math.Sin(Rotation));
                    ;

                    if (!SlowMode)
                    {
                        directionLeft = new Vector2((float)Math.Sin(Rotation - Math.PI / 4), (float)Math.Cos(Rotation - Math.PI / 4) * -1);
                        directionRight = new Vector2((float)Math.Sin(Rotation + Math.PI / 4), (float)Math.Cos(Rotation + Math.PI / 4) * -1);
                    }

                    var bulletLeft = new Bullet(Game, _bulletSprite, positionLeft, directionLeft, Config.PlayerBulletVelocity);

                    var bulletRight = new Bullet(Game, _bulletSprite, positionRight, directionRight, Config.PlayerBulletVelocity);

                    AddBullet(bulletLeft);
                    AddBullet(bulletRight);
                }

                // Front sides 1/4 diagonal
                if (PlayerData.ShootTypeIndex >= 3)
                {
                    Vector2 directionLeft = direction;
                    Vector2 positionLeft = new Vector2(Position.X - 10f * (float)Math.Cos(Rotation), Position.Y - 10f * (float)Math.Sin(Rotation));
                    Vector2 directionRight = direction;
                    Vector2 positionRight = new Vector2(Position.X + 10f * (float)Math.Cos(Rotation), Position.Y + 10f * (float)Math.Sin(Rotation));

                    if (!SlowMode)
                    {
                        directionLeft = new Vector2((float)Math.Sin(Rotation - Math.PI / 8), (float)Math.Cos(Rotation - Math.PI / 8) * -1);
                        directionRight = new Vector2((float)Math.Sin(Rotation + Math.PI / 8), (float)Math.Cos(Rotation + Math.PI / 8) * -1);
                    }

                    var bulletLeft = new Bullet(Game, _bulletSprite, positionLeft, directionLeft, Config.PlayerBulletVelocity);
                    bulletLeft.Power = 0.5f;

                    var bulletRight = new Bullet(Game, _bulletSprite, positionRight, directionRight, Config.PlayerBulletVelocity);
                    bulletRight.Power = 0.5f;

                    AddBullet(bulletLeft);
                    AddBullet(bulletRight);
                }

                // Behind
                if (PlayerData.ShootTypeIndex >= 2)
                {
                    var directionBehind = new Vector2((float)Math.Sin(Rotation) * -1, (float)Math.Cos(Rotation));

                    var bullet = new Bullet(Game, _bulletSprite, Position, directionBehind, Config.PlayerBulletVelocity);
                    bullet.Power = Improvements.ShootPowerData[PlayerData.ShootPowerIndex].Key;
                    bullet.WaveMode = false;

                    AddBullet(bullet);
                }

                _shootSound.Play();
            }
        }

        public void Hit()
        {
            if (!IsInvincible)
            {
                _lives--;
                _deadSound.Play();

                IsInvincible = true;
            }
        }

        public void AddScore(int value)
        {
            _score += value;
        }
    }
}
