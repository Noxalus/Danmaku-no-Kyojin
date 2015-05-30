using System.Net.Configuration;
using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Collisions;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Xna.Framework.Audio;
using System.Linq;

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

        // Players
        private List<Player> _players;

        // Structure
        private BossStructure _structure;
        private int _iteration;
        private float _step;

        // Turrets
        private List<Turret> _turrets; 

        // Audio
        private SoundEffect _deadSound = null;

        public int DefeatNumber { get; set; }

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public List<Turret> Turrets
        {
            get { return _turrets; }
        }

        public Boss(DnK gameRef, List<Player> players, int iteration = 50, float step = 25)
            : base(gameRef)
        {
            MoverManager = new MoverManager(GameRef);
            GameManager.GameDifficulty = Config.GameDifficultyDelegate;
            DefeatNumber = 0;
            _speed = 1f;
            _players = players;
            _iteration = iteration;
            _step = step;
            _turrets = new List<Turret>();
        }

        public override void Initialize()
        {
            int targetPlayerId = GameRef.Rand.Next(0, _players.Count);

            MoverManager.Initialize(_players[targetPlayerId].GetPosition);

            MoverManager.movers.Clear();

            Position = Vector2.Zero;
            _motion = new Vector2(1, 0);
            _ready = true;

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
            // TODO: Design a core sprite
            //Sprite = GameRef.Content.Load<Texture2D>(@"Graphics/Entities/enemy");
            _healthBar = GameRef.Pixel;
            
            // Audio
            if (_deadSound == null)
                _deadSound = GameRef.Content.Load<SoundEffect>(@"Audio/SE/boss_dead");

            //Get all the xml files
            foreach (var source in Directory.GetFiles(@"Content\XML\Patterns", "*.xml", SearchOption.AllDirectories))
            {
                //store the name
                _patternNames.Add(source);

                //load the pattern
                var pattern = new BulletPattern();
                pattern.ParseXML(source);
                _myPatterns.Add(pattern);
            }

            _currentPattern = GameRef.Rand.Next(0, _patternNames.Count);
            AddBullet(true);

            GenerateStructure();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds * 100;

            if (false/*Position.Y < (75 + Sprite.Height / 2f)*/)
            {
                Y += 1 * Speed * dt;

                /*
                if (Position.Y >= (75 + Sprite.Height / 2f))
                    _ready = true;
                */
            }
            else
            {
                /*
                Rotation += _rotationIncrementor * dt;
                if (_rotationIncrementor < 1f)
                    _rotationIncrementor += 0.001f;
                */
                if (_health <= 0)
                {
                    IsAlive = false;
                    //_deadSound.Play();
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
                    else if (InputHandler.KeyDown(Keys.Space))
                    {
                        _structure.GenerateBaseStructure();

                        CollisionBoxes = _structure.CollisionBoxes;//.Add(new CollisionConvexPolygon(this, Vector2.Zero, _structure.Vertices));
                        Size = new Point((int)_structure.GetSize().X, (int)_structure.GetSize().Y);
                        Position = new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y / 4f);
                        Origin = _structure.Origin;

                        GenerateTurrets();
                    }
                    else if (InputHandler.KeyPressed(Keys.OemPlus))
                    {
                        _structure.Iterate();

                        CollisionBoxes = _structure.CollisionBoxes;
                        Size = new Point((int)_structure.GetSize().X, (int)_structure.GetSize().Y);
                        Position = new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y / 4f);
                        Origin = _structure.Origin;

                        GenerateTurrets();
                    }

                    var velocity = 0f;
                    var direction = Vector2.Zero;
                    // Keyboard
                    if (InputHandler.KeyDown(Keys.I))
                        direction.Y = -1;
                    if (InputHandler.KeyDown(Keys.L))
                        direction.X = 1;
                    if (InputHandler.KeyDown(Keys.K))
                        direction.Y = 1;
                    if (InputHandler.KeyDown(Keys.J))
                        direction.X = -1;

                    if (InputHandler.KeyDown(Keys.PageUp))
                    {
                        Rotation += dt * 0.01f;
                    }
                    else if (InputHandler.KeyDown(Keys.PageDown))
                    {
                        Rotation -= dt * 0.01f;
                    }

                    if (direction != Vector2.Zero)
                    {
                        velocity = Config.PlayerMaxVelocity / 2;
                    }
                    else
                    {
                        velocity = Config.PlayerMaxVelocity;
                    }

                    velocity /= 100;

                    X += direction.X * velocity * dt;
                    Y += direction.Y * velocity * dt;

                    X = MathHelper.Clamp(Position.X, Size.X / 2f, Config.GameArea.X - Size.X / 2f);
                    Y = MathHelper.Clamp(Position.Y, Size.Y / 2f, Config.GameArea.Y - Size.Y / 2f);
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

            foreach (var turret in _turrets)
            {
                turret.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, Matrix viewMatrix)
        {
            /*
            GameRef.SpriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Origin, 1f, SpriteEffects.None, 0f);
            GameRef.SpriteBatch.Draw(_healthBar, new Rectangle(
                (int)Position.X - Sprite.Width / 2, (int)Position.Y + Sprite.Height / 2 + 20,
                (int)(100f * (_health / _totalHealth)), 10), Color.Blue);
            */

            _structure.Draw(viewMatrix, Position, Origin, Rotation, Scale);

            foreach (var mover in MoverManager.movers)
            {
                mover.Draw(gameTime);
            }

            /*
            GameRef.SpriteBatch.End();
            
            GameRef.SpriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            */
            //_laser.Draw(gameTime);
            /*
            GameRef.SpriteBatch.End();

            GameRef.SpriteBatch.Begin();
            */

            foreach (var turret in _turrets)
            {
                turret.Draw(gameTime);
            }

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
            /*
            _mover = (Mover)MoverManager.CreateBullet();
            _mover.X = Position.X;
            _mover.Y = Position.Y - 5;
            _mover.SetBullet(_myPatterns[_currentPattern].RootNode);
            */
        }

        public string GetCurrentPatternName()
        {
            return Path.GetFileNameWithoutExtension(_patternNames[_currentPattern].ToUpper());
        }

        public bool IsReady()
        {
            return _ready;
        }

        private void GenerateStructure()
        {
            _structure = new BossStructure(GameRef, this, _iteration, _step);

            CollisionBoxes = _structure.CollisionBoxes;
            Size = new Point((int)_structure.GetSize().X, (int)_structure.GetSize().Y);
            Position = new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y / 4f);
            Origin = _structure.Origin;

            GenerateTurrets();
        }

        private void GenerateCollisionBoxes()
        {
        }

        private void GenerateTurrets()
        {
            /*
            _turrets.Clear();

            foreach (var vertex in _structure.Vertices)
            {
                if (true) //GameRef.Rand.NextDouble() > 0.75f)
                {
                    var patternIndex = GameRef.Rand.Next(_myPatterns.Count);
                    var color = patternIndex == 0 ? Color.DarkBlue : Color.DarkRed;

                    var turret = new Turret(GameRef, this, _players[0], new Vector2(vertex.X, vertex.Y), _myPatterns[patternIndex], color);
                    turret.Initialize();
                    _turrets.Add(turret);
                }
            }
            */
        }

        public void DestroyTurret(Turret turret, BaseBullet bullet)
        {
            /*
            _turrets.Remove(turret);

            _structure.Vertices.Add(new Vector2());
            var index = _structure.Vertices.FindIndex(vertex => vertex == turret.InitialPosition);
            
            var newPosition = turret.InitialPosition + bullet.Direction * 20f;
            _structure.Vertices[index] = newPosition;

            var newTurret = new Turret(GameRef, this, newPosition, _myPatterns[0]);
            newTurret.Initialize();
            _turrets.Add(newTurret);

            var allVertices = new HashSet<Vector2>(_structure.Vertices).ToList();
            */
            //_polygonShape = new PolygonShape(GameRef.GraphicsDevice, allVertices.ToArray());
        }
    }
}
