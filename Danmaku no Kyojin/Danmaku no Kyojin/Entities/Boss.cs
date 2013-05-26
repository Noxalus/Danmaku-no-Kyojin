using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Collisions;
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
        private int _currentPattern = 0;

        private float _speed;

        private Vector2 _motion;

        public float GetRank() { return 0; }

        private const float TotalHealth = 500000000f;
        private float _health;
        private Texture2D _healthBar;

        private Laser _laser;

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public Boss(DnK game)
            : base(game)
        {
            Position = Vector2.Zero;
            _motion = new Vector2(1, 0);
            _speed = 1;

            _health = TotalHealth;

            IsAlive = true;

            _laser = new Laser(Game, new Vector2(50, 300), new Vector2(350, 100), 8);
        }

        public override void Initialize()
        {
            MoverManager = new MoverManager(Game, Game.GameplayScreen.Players[0].GetPosition);

            _laser.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Sprite = Game.Content.Load<Texture2D>("Graphics/Entities/enemy");
            _healthBar = Game.Content.Load<Texture2D>("Graphics/Pictures/pixel");

            Center = new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);

            List<Point> vertices = new List<Point>()
                {
                    new Point(0, (int)(Sprite.Height / 2.74f)),
                    new Point(Sprite.Width / 2, 0),
                    new Point(Sprite.Width, (int)(Sprite.Height / 2.74f)),
                    new Point(Sprite.Width / 2, Sprite.Height)
                };

            CollisionBox = new CollisionConvexPolygon(this, Vector2.Zero, vertices);

            Position = new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2f,
                20 + Sprite.Height / 2f);

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

            if (Position.X > Game.GraphicsDevice.Viewport.Width - Sprite.Width - (Speed * dt) ||
                Position.X < Sprite.Width + (Speed * dt))
                _motion *= -1;

            //Position += _motion * Speed * dt;

            if (_health <= 0)
            {
                IsAlive = false;
                MoverManager.movers.Clear();
            }


            //check input to increment/decrement the current bullet pattern
            if (InputHandler.KeyPressed(Keys.A))
            {
                //decrement the pattern
                if (0 >= _currentPattern)
                {
                    //if it is at the beginning, move to the end
                    _currentPattern = _myPatterns.Count - 1;
                }
                else
                {
                    _currentPattern--;
                }

                AddBullet(true);
            }
            else if (InputHandler.KeyPressed(Keys.X))
            {
                //increment the pattern
                if ((_myPatterns.Count - 1) <= _currentPattern)
                {
                    //if it is at the beginning, move to the end
                    _currentPattern = 0;
                }
                else
                {
                    _currentPattern++;
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

            MoverManager.Update(gameTime);
            MoverManager.FreeMovers();

            _laser.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Center, 1f, SpriteEffects.None, 0f);

            Game.SpriteBatch.Draw(_healthBar, new Rectangle(
                (int)Position.X - Sprite.Width / 2, (int)Position.Y + Sprite.Height / 2 + 20,
                (int)(100f * (_health / TotalHealth)), 10), Color.Blue);

            /*
            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, _patternNames[_currentPattern], new Vector2(1, Game.GraphicsDevice.Viewport.Height - 24), Color.Black);
            Game.SpriteBatch.DrawString(ControlManager.SpriteFont, _patternNames[_currentPattern], new Vector2(0, Game.GraphicsDevice.Viewport.Height - 25), Color.White);
            */

            foreach (Mover mover in MoverManager.movers)
            {
                mover.Draw(gameTime);
            }

            /*
            Game.SpriteBatch.End();
            
            Game.SpriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            */
            _laser.Draw(gameTime);
            /*
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin();
            */
            base.Draw(gameTime);
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;
        }

        private void AddBullet(bool clear)
        {
            if (true)
            {
                //clear out all the bulelts
                MoverManager.movers.Clear();
            }

            //add a new bullet in the center of the screen
            _mover = (Mover)MoverManager.CreateBullet();
            _mover.X = Position.X;
            _mover.Y = Position.Y - 5;
            _mover.SetBullet(_myPatterns[_currentPattern].RootNode);
        }
    }
}
