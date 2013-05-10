using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace Danmaku_no_Kyojin.Entities
{
    class Boss : Entity
    {
        // Bullet engine
        private Texture2D _bulletSprite;
        int timer = 0;
        Mover _mover;
        public MoverManager MoverManager { get; set; }

        /// <summary>
        /// A list of all the bulletml samples we have loaded
        /// </summary>
        private List<BulletPattern> _myPatterns = new List<BulletPattern>();

        /// <summary>
        /// The names of all the bulletml patterns that are loaded, stored so we can display what is being fired
        /// </summary>
        private List<string> _patternNames = new List<string>();

        /// <summary>
        /// The current Bullet ML pattern to use to shoot bullets
        /// </summary>
        private int _CurrentPattern = 0;

        private float _speed;

        private Texture2D _sprite;

        private Vector2 _motion;

        public float GetRank() { return 0; }

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
            MoverManager = new MoverManager(Game.GameplayScreen.Player.GetPosition);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _bulletSprite = Game.Content.Load<Texture2D>(@"Graphics/Sprites/ball");
            _sprite = Game.Content.Load<Texture2D>("Graphics/Entities/enemy");
            _healthBar = Game.Content.Load<Texture2D>("Graphics/Pictures/pixel");

            Position = new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2 - _sprite.Width / 2,
                20);

            //Get all the xml files
            foreach (var source in Directory.GetFiles(@"Content\XML\", "*.xml", SearchOption.AllDirectories))
            {
                //store the name
                _patternNames.Add(source);

                //load the pattern
                BulletPattern pattern = new BulletPattern();
                pattern.ParseXML(source);
                _myPatterns.Add(pattern);
            }

            GameManager.GameDifficulty = this.GetRank;

            AddBullet(true);

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


            //check input to increment/decrement the current bullet pattern
            if (InputHandler.KeyPressed(Keys.A))
            {
                //decrement the pattern
                if (0 >= _CurrentPattern)
                {
                    //if it is at the beginning, move to the end
                    _CurrentPattern = _myPatterns.Count - 1;
                }
                else
                {
                    _CurrentPattern--;
                }

                AddBullet(true);
            }
            else if (InputHandler.KeyPressed(Keys.X))
            {
                //increment the pattern
                if ((_myPatterns.Count - 1) <= _CurrentPattern)
                {
                    //if it is at the beginning, move to the end
                    _CurrentPattern = 0;
                }
                else
                {
                    _CurrentPattern++;
                }

                AddBullet(true);
            }
            else if (InputHandler.KeyPressed(Keys.LeftControl))
            {
                AddBullet(false);
            }

            timer++;
            if (timer > 60)
            {
                timer = 0;
                if (_mover.used == false)
                {
                    AddBullet(true);
                }
            }

            MoverManager.Update();
            MoverManager.FreeMovers();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(_sprite, new Rectangle((int)Position.X, (int)Position.Y, _sprite.Width, _sprite.Height), Color.White);
            Game.SpriteBatch.Draw(_healthBar, new Rectangle(
                (int)Position.X, (int)Position.Y + _sprite.Height + 20,
                (int)(100f * (_health / TotalHealth)), 10), Color.Blue);

            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, _patternNames[_CurrentPattern], new Vector2(1, Game.GraphicsDevice.Viewport.Height - 24), Color.Black);
            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, _patternNames[_CurrentPattern], new Vector2(0, Game.GraphicsDevice.Viewport.Height - 25), Color.White);

            foreach (Mover mover in MoverManager.movers)
            {
                Game.SpriteBatch.Draw(_bulletSprite,
                                         new Vector2(
                                             mover.pos.X - _bulletSprite.Width / 2,
                                             mover.pos.Y - _bulletSprite.Height / 2),
                                         Color.White);

                if (Config.DisplayCollisionBoxes)
                {
                    Rectangle bulletRectangle = new Rectangle(
                        (int)mover.pos.X - _bulletSprite.Width / 2,
                        (int)mover.pos.Y - _bulletSprite.Height / 2,
                        _bulletSprite.Width,
                        _bulletSprite.Height);
                    Game.SpriteBatch.Draw(DnK._pixel, bulletRectangle, Color.White);
                }
            }

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

        private void AddBullet(bool clear)
        {
            if (clear)
            {
                //clear out all the bulelts
                MoverManager.movers.Clear();
            }

            //add a new bullet in the center of the screen
            _mover = (Mover)MoverManager.CreateBullet();
            _mover.pos = new Vector2(Position.X + _sprite.Width / 2, Position.Y + _sprite.Height / 2 - 5);
            _mover.SetBullet(_myPatterns[_CurrentPattern].RootNode);
        }
    }
}
