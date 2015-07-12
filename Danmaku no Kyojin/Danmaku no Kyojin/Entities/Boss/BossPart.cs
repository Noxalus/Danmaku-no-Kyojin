using System;
using System.Collections.Generic;
using System.IO;
using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Collisions;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Particles;
using Danmaku_no_Kyojin.Shapes;
using Danmaku_no_Kyojin.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Entities.Boss
{
    // BossPart class contains motion + turrets logic and can be splitted in 2 BossPart
    class BossPart : Entity
    {
        private BossStructure _structure;
        private MoverManager _moverManager;
        private Mover _mover;
        private List<BulletPattern> _bulletPatterns;
        private int _currentPatternIndex; 

        private float _health;
        private bool _displayHp;
        private static readonly Color InitialColor = new Color(0f, 0.75f, 0f, 0.65f);
        private static readonly Color HitColor = new Color(0f, 0.75f, 0f, 0.75f);
        private Color _color;
        private TimeSpan _changeColorTimer;

        private int _iteration;
        private float _step;

        // Turrets
        private List<Turret> _turrets;

        // Shape
        private PolygonShape _polygonShape;

        // Sprites
        private Texture2D _healthBar;

        // Sounds
        private SoundEffect _deadSound;

        public BossPart(
            DnK gameRef, 
            MoverManager moverManager, 
            List<BulletPattern> bulletPatterns, 
            Color color, 
            float initialHealth,
            int iteration = 50, 
            float step = 25,
            List<Turret> turrets = null,
            PolygonShape polygonShape = null)
            : base(gameRef)
        {
            _structure = new BossStructure(gameRef, this);
            _moverManager = moverManager;
            _bulletPatterns = bulletPatterns;

            _health = initialHealth;
            _displayHp = false;
            _color = color;

            _iteration = iteration;
            _step = step;

            _turrets = turrets ?? new List<Turret>();
        }

        public override void Initialize()
        {
            CollisionBoxes.AddRange(_structure.CollisionBoxes);
            Size = new Point((int)_structure.GetSize().X, (int)_structure.GetSize().Y);
            Position = new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y / 4f);
            Origin = _structure.Origin;

            _currentPatternIndex = GameRef.Rand.Next(0, _bulletPatterns.Count);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _healthBar = GameRef.Pixel;

            // Audio
            if (_deadSound == null)
                _deadSound = GameRef.Content.Load<SoundEffect>(@"Audio/SE/boss_dead");

            GenerateStructure();

            base.LoadContent();
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

        private void GenerateTurrets()
        {
            /*
            _turrets.Clear();

            foreach (var vertex in _structure.Vertices)
            {
                if (true) //GameRef.Rand.NextDouble() > 0.75f)
                {
                    var patternIndex = GameRef.Rand.Next(_bulletPatterns.Count);
                    var color = patternIndex == 0 ? Color.DarkBlue : Color.DarkRed;

                    var turret = new Turret(GameRef, this, _players[0], new Vector2(vertex.X, vertex.Y), _bulletPatterns[patternIndex], color);
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

        public override void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds * 100;

            if (Config.Debug)
            {
                //check input to increment/decrement the current bullet pattern
                if (InputHandler.KeyPressed(Keys.A))
                {
                    //decrement the pattern
                    if (0 >= _currentPatternIndex)
                    {
                        //if it is at the beginning, move to the end
                        _currentPatternIndex = _bulletPatterns.Count - 1;
                    }
                    else
                    {
                        _currentPatternIndex--;
                    }

                    AddBullet(true);
                }
                else if (InputHandler.KeyPressed(Keys.X))
                {
                    //increment the pattern
                    if ((_bulletPatterns.Count - 1) <= _currentPatternIndex)
                    {
                        //if it is at the beginning, move to the end
                        _currentPatternIndex = 0;
                    }
                    else
                    {
                        _currentPatternIndex++;
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

                    var bossRectangleVertices = new List<Vector2>()
                    {
                        new Vector2(0, -_structure.GetSize().Y / 2),
                        new Vector2(_structure.GetSize().X, -_structure.GetSize().Y / 2),                          
                        new Vector2(_structure.GetSize().X, _structure.GetSize().Y -_structure.GetSize().Y / 2),                          
                        new Vector2(0, _structure.GetSize().Y -_structure.GetSize().Y / 2)                          
                    };

                    CollisionBoxes = _structure.CollisionBoxes;
                    //CollisionBoxes.Add(new CollisionConvexPolygon(this, Vector2.Zero, bossRectangleVertices));
                    Size = new Point((int)_structure.GetSize().X, (int)_structure.GetSize().Y);
                    Position = new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y / 2f);
                    Origin = _structure.Origin;

                    GenerateTurrets();
                }
                else if (InputHandler.KeyPressed(Keys.OemPlus))
                {
                    _structure.Iterate();

                    CollisionBoxes = _structure.CollisionBoxes;
                    Size = new Point((int)_structure.GetSize().X, (int)_structure.GetSize().Y);
                    Position = new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y / 2f);
                    Origin = _structure.Origin;

                    GenerateTurrets();
                }
                else if (InputHandler.KeyDown(Keys.P))
                {
                    _structure.Iterate();

                    CollisionBoxes = _structure.CollisionBoxes;
                    Size = new Point((int)_structure.GetSize().X, (int)_structure.GetSize().Y);
                    Position = new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y / 2f);
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

                if (InputHandler.KeyPressed(Keys.H))
                {
                    _displayHp = !_displayHp;
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

            foreach (var turret in _turrets)
            {
                turret.Update(gameTime);
            }

            if (_changeColorTimer.TotalMilliseconds > 0)
            {
                _changeColorTimer -= gameTime.ElapsedGameTime;

                if (_changeColorTimer.Milliseconds <= 0)
                {
                    _changeColorTimer = TimeSpan.Zero;
                    _color = InitialColor;
                }
            }

            base.Update(gameTime);
        }

        public override bool Intersects(Entity entity)
        {
            foreach (var collisionBox in CollisionBoxes)
            {
                var collisionConvexPolygon = collisionBox as CollisionConvexPolygon;

                if (collisionConvexPolygon == null)
                    return base.Intersects(entity);

                foreach (var entityCollisionBox in entity.CollisionBoxes)
                {
                    if (entityCollisionBox.Intersects(collisionBox))
                    {
                        var bullet = entity as BaseBullet;

                        if (bullet != null)
                        {
                            collisionConvexPolygon.HealthPoint -= bullet.Power;

                            if (collisionConvexPolygon.HealthPoint <= 0)
                            {
                                float hue1 = GameRef.Rand.NextFloat(0, 6);
                                float hue2 = (hue1 + GameRef.Rand.NextFloat(0, 2)) % 6f;
                                Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
                                Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);

                                for (int i = 0; i < 120; i++)
                                {
                                    float speed = 18f * (1f - 1 / GameRef.Rand.NextFloat(1f, 10f));
                                    var state = new ParticleState()
                                    {
                                        Velocity = GameRef.Rand.NextVector2(speed, speed),
                                        Type = ParticleType.Enemy,
                                        LengthMultiplier = 1f
                                    };

                                    Color color = Color.Lerp(color1, color2, GameRef.Rand.NextFloat(0, 1));

                                    GameRef.ParticleManager.CreateParticle(GameRef.LineParticle, bullet.Position,
                                        color, 190, 1.5f, state);
                                }

                                var newPolygonShape = _structure.Split(collisionConvexPolygon);

                                var center = collisionConvexPolygon.GetCenter();

                                // TODO: Part is dead?
                                // A boss part is dead when its center is dead?
                                // or when the number of sub-parts is less than a number?
                                /*
                                if (center.X > (Size.X / 2f - 2 * _step) + Position.X - Origin.X &&
                                    center.X < (Size.X / 2f + 2 * _step) + Position.X - Origin.X)
                                {
                                    TakeDamage(99999);
                                }
                                else
                                {
                                    // TODO: Instanciate a new BossStructure
                                    // This should be done into Boss class
                                    // Idea => execute a signal to be catched by Boss class?
                                    //var bossPart = new BossPart(GameRef, newPolygonShape);
                                }
                                */
                            }
                        }

                        _color = HitColor;
                        _changeColorTimer = new TimeSpan(0, 0, 0, 0, 40);

                        return true;
                    }
                }
            }

            return false;
        }

        private void AddBullet(bool clear)
        {
            // Add a new bullet in the center of the screen
            _mover = (Mover)_moverManager.CreateBullet();
            _mover.X = Position.X;
            _mover.Y = Position.Y - 5;
            _mover.SetBullet(_bulletPatterns[_currentPatternIndex].RootNode);
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;

            if (_health < 0)
            {
                float hue1 = GameRef.Rand.NextFloat(0, 6);
                float hue2 = (hue1 + GameRef.Rand.NextFloat(0, 2)) % 6f;
                Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
                Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);

                for (int i = 0; i < 5000; i++)
                {
                    float speed = 18f * (1f - 1 / GameRef.Rand.NextFloat(1f, 10f));
                    var state = new ParticleState()
                    {
                        Velocity = GameRef.Rand.NextVector2(speed, speed),
                        Type = ParticleType.Enemy,
                        LengthMultiplier = 1f
                    };

                    Color color = Color.Lerp(color1, color2, GameRef.Rand.NextFloat(0, 1));

                    GameRef.ParticleManager.CreateParticle(GameRef.LineParticle, new Vector2(Position.X, Position.Y - Size.Y / 2f),
                        color, 190, 1.5f, state);
                }

                IsAlive = false;
            }
        }

        public void Draw(GameTime gameTime, Matrix viewMatrix)
        {
            _structure.Draw(viewMatrix, Position, _color, Rotation, Origin, Scale);

            foreach (var mover in _moverManager.movers)
            {
                mover.Draw(gameTime);
            }

            // Draw collision boxes HP
            if (_displayHp)
            {
                for (int i = 0; i < CollisionBoxes.Count; i++)
                {
                    var collisionBox = CollisionBoxes[i];
                    var collisionConvexPolygon = collisionBox as CollisionConvexPolygon;
                    if (collisionConvexPolygon != null)
                    {
                        var text = collisionConvexPolygon.HealthPoint.ToString();
                        var position = collisionConvexPolygon.GetCenter();

                        position.X -= ControlManager.SpriteFont.MeasureString(text).X / 2;
                        position.Y -= ControlManager.SpriteFont.MeasureString(text).Y / 2;

                        GameRef.SpriteBatch.DrawString(
                            ControlManager.SpriteFont,
                            text,
                            position,
                            Color.White);
                    }
                }
            }

            foreach (var turret in _turrets)
            {
                turret.Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
