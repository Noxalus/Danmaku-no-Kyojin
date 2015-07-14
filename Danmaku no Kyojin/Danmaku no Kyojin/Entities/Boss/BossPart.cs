using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Boss _bossRef;
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

        private float _accelerationDecreaseFactor;

        private BossCore _core;

        // Turrets
        private List<Turret> _turrets;

        // Shape
        private PolygonShape _polygonShape;

        // Sprites
        private Texture2D _healthBar;

        // Sounds
        private SoundEffect _deadSound;

        // Direction of the motion
        public Vector2 Direction { get; set; }
        // Speed as pixel/frame
        public float Velocity { get; set; }
        // Speed modification on X and Y
        public Vector2 Acceleration { get; set; }
        
        public BossPart(
            DnK gameRef,
            Boss bossRef,
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
            _bossRef = bossRef;
            _moverManager = moverManager;
            _bulletPatterns = bulletPatterns ?? new List<BulletPattern>();

            _health = initialHealth;
            _displayHp = false;
            _color = color;

            _iteration = iteration;
            _step = step;

            _accelerationDecreaseFactor = 0.0075f;

            _turrets = turrets ?? new List<Turret>();
            _polygonShape = polygonShape;
        }

        public override void Initialize()
        {
            GenerateStructure();

            CollisionBoxes.AddRange(_structure.CollisionBoxes);
            Size = new Point((int)_structure.GetSize().X, (int)_structure.GetSize().Y);
            Position = new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y / 4f);
            Origin = _structure.Origin;

            _currentPatternIndex = GameRef.Rand.Next(0, _bulletPatterns.Count);
            Direction = Vector2.Zero;
            Velocity = (Config.PlayerMaxVelocity / 2) / 100f;
            Acceleration = Vector2.Zero;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _healthBar = GameRef.Pixel;

            // Audio
            if (_deadSound == null)
                _deadSound = GameRef.Content.Load<SoundEffect>(@"Audio/SE/boss_dead");

            _core = new BossCore(GameRef, this);
            _core.Initialize();

            base.LoadContent();
        }

        public void GenerateStructure()
        {
            _structure = new BossStructure(GameRef, this, _iteration, _step, _polygonShape);

            CollisionBoxes = _structure.CollisionBoxes;
            Size = _structure.GetSize().ToPoint();
            Origin = _structure.Origin;

            GenerateTurrets();
        }

        public void IterateStructure(int iterationNumber = 1)
        {
            _structure.Iterate(iterationNumber);

            CollisionBoxes = _structure.CollisionBoxes;
            Size = _structure.GetSize().ToPoint();
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
            
            // Part motion
            if (Direction == Vector2.Zero)
            {
                Acceleration = Vector2.Zero;
            }

            X += Direction.X * (Velocity * Acceleration.X) * dt;
            Y += Direction.Y * (Velocity * Acceleration.Y) * dt;

            if (Math.Abs(Acceleration.X) < 0.01f && Math.Abs(Acceleration.Y) < 0.01f)
            {
                Acceleration = Vector2.Zero;
                Direction = Vector2.Zero;
            }

            if (Acceleration.X > 0)
            {
                Acceleration = new Vector2(Acceleration.X - (_accelerationDecreaseFactor * dt), Acceleration.Y);
            }
            else if (Acceleration.X < 0)
            {
                Acceleration = new Vector2(Acceleration.X + (_accelerationDecreaseFactor * dt), Acceleration.Y);
            }

            if (Acceleration.Y > 0)
            {
                Acceleration = new Vector2(Acceleration.X, Acceleration.Y - (_accelerationDecreaseFactor * dt));
            }
            else if (Acceleration.Y < 0)
            {
                Acceleration = new Vector2(Acceleration.X, Acceleration.Y + (_accelerationDecreaseFactor * dt));
            }

            // Turrets
            foreach (var turret in _turrets)
            {
                turret.Update(gameTime);
            }

            // Color highlight timer
            if (_changeColorTimer.TotalMilliseconds > 0)
            {
                _changeColorTimer -= gameTime.ElapsedGameTime;

                if (_changeColorTimer.Milliseconds <= 0)
                {
                    _changeColorTimer = TimeSpan.Zero;
                    _color = InitialColor;
                }
            }

            _core.Update(gameTime);

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
                                if (center.X > (Size.X / 2f - 2 * _step) + Position.X - Origin.X &&
                                    center.X < (Size.X / 2f + 2 * _step) + Position.X - Origin.X)
                                {
                                    TakeDamage(99999);
                                }
                                else
                                {
                                    // If the break out part is not large enough => we don't create another part
                                    if (newPolygonShape.Vertices.Length > 10)
                                    {
                                        var bossPart = new BossPart(GameRef, _bossRef, _moverManager, null, _color,
                                            _health, 0, 25f, null, newPolygonShape);
                                        bossPart.Initialize();

                                        var bossPartSize = newPolygonShape.GetSize();

                                        // Left (1) or right (-1) part?
                                        var factor = (Position.X > collisionConvexPolygon.GetCenter().X) ? 1 : -1;
                                        bossPart.Position = new Vector2(
                                            (collisionConvexPolygon.GetCenter().X + (_step/ 8f) * factor) - (bossPartSize.X / 2f + _step) * factor,
                                            Position.Y - Origin.Y
                                        );

                                        var angleCos = (float)Math.Cos(Rotation);
                                        var angleSin = (float)Math.Sin(Rotation);

                                        // Translate point back to origin  
                                        //bossPart.Position = new Vector2(bossPart.Position.X - Origin.X, bossPart.Position.Y - Origin.Y);

                                        // Rotate point
                                        var newX = bossPart.Position.X * angleCos - bossPart.Position.Y * angleSin;
                                        var newY = bossPart.Position.X * angleSin + bossPart.Position.Y * angleCos;

                                        // Translate point back
                                        bossPart.Position = new Vector2(newX, newY);

                                        bossPart.Rotation = Rotation;
                                        
                                        // TODO: Compute the Origin point for this new BossPart => It will be the polygon's center, not the center of the box
                                        bossPart.Origin = newPolygonShape.GetCenter();

                                        // TODO: Add bounding boxes to this BossPart (move bounding box computation + logic from BossStructure to BossPart?)

                                        // TODO: Give to this new BossPart an impulsion to (pseudo) random direction due to explosion

                                        _bossRef.Parts.Add(bossPart);

                                        var random = (float)(GameRef.Rand.NextDouble() * (1f - 0.75f)) + 0.75f;
                                        Debug.WriteLine("Random: " + random);
                                        bossPart.ApplyImpulse(new Vector2(-factor, random * (-factor)), new Vector2(random));
                                        ApplyImpulse(new Vector2(factor, random * factor), new Vector2(random));
                                    }

                                    // This is the bounding that the player has destroyed, so we remove it from the list
                                    //entity.CollisionBoxes.Remove(collisionConvexPolygon);
                                }
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
            if (_bulletPatterns.Count > 0)
            {
                // Add a new bullet in the center of the screen
                _mover = (Mover) _moverManager.CreateBullet();
                _mover.X = Position.X;
                _mover.Y = Position.Y - 5;
                _mover.SetBullet(_bulletPatterns[_currentPatternIndex].RootNode);
            }
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

        public void ApplyImpulse(Vector2 direction, Vector2 acceleration)
        {
            Direction = (direction + Direction);
            Direction.Normalize();

            Acceleration += acceleration;
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

            _core.Draw(gameTime);

            base.Draw(gameTime);
        }

        public void DisplayHpSwitch()
        {
            _displayHp = !_displayHp;
        }
    }
}
