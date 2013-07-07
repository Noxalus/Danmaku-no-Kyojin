using System.Diagnostics;
using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Collisions;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Xna.Framework.Audio;

namespace Danmaku_no_Kyojin.Entities
{
    class Boss : Entity
    {
        // Bullet engine
        private Texture2D _bulletSprite;
        private TimeSpan _timer;
        Mover _mover;
        public MoverManager MoverManager { get; set; }

        // A list of all the bulletml samples we have loaded
        private List<BulletPattern> _myPatterns = new List<BulletPattern>();

        // The names of all the bulletml patterns that are loaded, stored so we can display what is being fired
        private List<string> _patternNames = new List<string>();

        // The current Bullet ML pattern to use to shoot bullets
        private int _currentPattern = 0;

        private float _speed;
        private bool _ready;
        private float _rotationIncrementor;

        private Vector2 _motion;

        public float GetRank() { return 0; }

        private const float InitialHealth = 30f;
        private float _totalHealth;
        private float _health;
        private Texture2D _healthBar;

        // Audio
        private SoundEffect _deadSound = null;

        public int DefeatNumber { get; set; }

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public Boss(DnK game)
            : base(game)
        {
            MoverManager = new MoverManager(Game);
            GameManager.GameDifficulty = Config.GameDifficultyDelegate;
            DefeatNumber = 0;
            _speed = 1f;
        }

        public override void Initialize()
        {
            int targetPlayerId = 0;
            if (Config.PlayersNumber == 2)
                targetPlayerId = GameplayScreen.Rand.Next(0, 2);

            MoverManager.Initialize(Game.GameplayScreen.Players[targetPlayerId].GetPosition);

            MoverManager.movers.Clear();

            Position = Vector2.Zero;
            _motion = new Vector2(1, 0);
            _ready = false;

            _totalHealth = InitialHealth * ((DefeatNumber + 1) / 2f);
            _health = _totalHealth;

            IsAlive = true;

            _timer = Config.BossInitialTimer;

            Rotation = 0;
            _rotationIncrementor = 0.001f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Sprite = Game.Content.Load<Texture2D>(@"Graphics/Entities/enemy");
            _healthBar = Game.Content.Load<Texture2D>(@"Graphics/Pictures/pixel");
            Center = new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);

            // Audio
            if (_deadSound == null)
                _deadSound = Game.Content.Load<SoundEffect>(@"Audio/SE/boss_dead");

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
                -Sprite.Height);

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

            _currentPattern = GameplayScreen.Rand.Next(0, _patternNames.Count);
            AddBullet(true);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds * 100;

            if (Position.Y < (75 + Sprite.Height / 2f))
            {
                Position.Y += 1 * Speed * dt;

                if (Position.Y >= (75 + Sprite.Height / 2f))
                    _ready = true;
            }
            else
            {
                Rotation += _rotationIncrementor * dt;
                if (_rotationIncrementor < 1f)
                    _rotationIncrementor += 0.001f;


                if (_health <= 0)
                {
                    IsAlive = false;
                    _deadSound.Play();
                    MoverManager.movers.Clear();
                }


                if (Config.Debug)
                {
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
                }

                _timer -= gameTime.ElapsedGameTime;
                if (_timer < TimeSpan.Zero)
                {
                    _timer = Config.BossInitialTimer;
                    /*
                    if (MoverManager.movers.Count < 10)
                        AddBullet(false);
                    */
                }

                if (MoverManager.movers.Count <= 1)
                    AddBullet(false);

                MoverManager.Update(gameTime);
                MoverManager.FreeMovers();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Center, 1f, SpriteEffects.None, 0f);

            Game.SpriteBatch.Draw(_healthBar, new Rectangle(
                (int)Position.X - Sprite.Width / 2, (int)Position.Y + Sprite.Height / 2 + 20,
                (int)(100f * (_health / _totalHealth)), 10), Color.Blue);

            foreach (Mover mover in MoverManager.movers)
            {
                mover.Draw(gameTime);
            }

            /*
            Game.SpriteBatch.End();
            
            Game.SpriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            */
            //_laser.Draw(gameTime);
            /*
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin();
            */
            base.Draw(gameTime);
        }

        public void TakeDamage(float damage)
        {
            if(_ready)
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
            _mover.X = Position.X;
            _mover.Y = Position.Y - 5;
            _mover.SetBullet(_myPatterns[_currentPattern].RootNode);
        }

        public string GetCurrentPatternName()
        {
            return Path.GetFileNameWithoutExtension(_patternNames[_currentPattern].ToUpper());
        }

        public bool IsReady()
        {
            return _ready;
        }
    }
}
