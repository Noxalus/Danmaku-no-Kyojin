using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private Dictionary<CollisionConvexPolygon, float> _collisionBoxesHp; 

        // Turrets
        private List<Turret> _turrets;

        // Shape
        private readonly PolygonShape _polygonShape;

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

        public float GetArea()
        {
            return (_structure != null) ? _structure.GetArea() : 0f;
        }

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

            _accelerationDecreaseFactor = 0.00075f;

            _turrets = turrets ?? new List<Turret>();
            _polygonShape = polygonShape;
            _collisionBoxesHp = new Dictionary<CollisionConvexPolygon, float>();
        }

        public override void Initialize()
        {
            GenerateStructure();

            Position = new Vector2(Config.GameArea.X / 2f, Config.GameArea.Y / 4f);
            Direction = Vector2.Zero;
            Velocity = (Config.PlayerMaxVelocity / 2) / 100f;
            Acceleration = Vector2.Zero;

            _currentPatternIndex = GameRef.Rand.Next(0, _bulletPatterns.Count);

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

            Size = _structure.GetSize().ToPoint();
            Origin = _structure.Origin;

            ComputeCollisionBoxes();
            GenerateTurrets();
        }

        public void IterateStructure(int iterationNumber = 1)
        {
            _structure.Iterate(iterationNumber);

            Size = _structure.GetSize().ToPoint();
            Origin = _structure.Origin;

            ComputeCollisionBoxes();
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
                            _collisionBoxesHp[collisionConvexPolygon] -= bullet.Power;

                            if (_collisionBoxesHp[collisionConvexPolygon] <= 0)
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

                                Split(collisionConvexPolygon);
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

        private void Split(CollisionConvexPolygon box)
        {
            var newPolygonShape = _structure.Split(box);
            var center = box.GetCenter();

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
                var boxLocalPosition = box.GetLocalPosition();
                
                // TODO: Fix part detection
                // Left (1) or right (-1) part?
                var factor = (boxLocalPosition.X > Origin.X) ? 1 : -1;
                
                // If the break out part is not large enough => we don't create another part
                // TODO: Use area instead of vertex number
                if (newPolygonShape.Vertices != null && newPolygonShape.GetArea() > Config.MinBossPartArea)
                {
                    var bossPart = new BossPart(
                        GameRef, _bossRef, _moverManager, null, _color,
                        _health, 0, 25f, null, newPolygonShape
                    );

                    bossPart.Initialize();

                    var bossPartSize = newPolygonShape.GetSize();
                    var boxWorldPosition = box.GetWorldPosition();

                    // Compute new boss part's world position
                    var worldPosition = Vector2.Zero;
                    worldPosition.Y = boxWorldPosition.Y - boxLocalPosition.Y;
                
                    // Left part
                    if (factor == 1)
                        worldPosition.X = boxWorldPosition.X - bossPartSize.X;
                    // Right part
                    else if (factor == -1)
                        worldPosition.X = boxWorldPosition.X + _step;

                    // Update world position according to parent rotation
                    bossPart.Scale = Scale;
                    bossPart.Rotation = Rotation;

                    var newLocalPosition = Vector2.Zero;
                    newLocalPosition.X = (Position.X - Origin.X) + (boxLocalPosition.X);
                    newLocalPosition.Y = (Position.Y - Origin.Y);

                    if (factor == -1)
                        newLocalPosition.X -= bossPartSize.X;
                    else
                        newLocalPosition.X += _step;

                    var newOrigin = newPolygonShape.GetCenter();
                    newLocalPosition += newOrigin;

                    var rotationOrigin = Position;
                    newLocalPosition = Vector2.Transform(newLocalPosition - rotationOrigin, Matrix.CreateRotationZ(Rotation)) + rotationOrigin;

                    bossPart.Origin = newOrigin;
                    bossPart.Position = newLocalPosition;
                    _bossRef.Parts.Add(bossPart);

                    // Give to this new BossPart an impulsion to (pseudo) random direction due to explosion
                    var random = (float)(GameRef.Rand.NextDouble() * (1f - 0.75f)) + 0.75f;
                    bossPart.ApplyImpulse(new Vector2(factor, random * factor), new Vector2(random / 5f));
                    ApplyImpulse(new Vector2(-factor, random * -factor), new Vector2(random / 5f));
                }

                // Remove destroyed bounding boxes
                if (factor == -1)
                    CollisionBoxes.RemoveAll(bb => bb.GetCenter().X < boxLocalPosition.X);
                else
                    CollisionBoxes.RemoveAll(bb => bb.GetCenter().X > boxLocalPosition.X);

                // This is the bounding that the player has destroyed, so we remove it from the list
                CollisionBoxes.Remove(box);
            }
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

            Acceleration = acceleration;
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
                        var text = _collisionBoxesHp[collisionConvexPolygon].ToString();
                        var position = collisionConvexPolygon.GetCenterInWorldSpace();

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

        // Create multiple collision boxes from BossStructure
        private void ComputeCollisionBoxes()
        {
            CollisionBoxes.Clear();
            _collisionBoxesHp.Clear();

            // TODO: Handle the case where there is 2 vertices with Y = 0
            var vertices = _structure.GetVertices();
            var bottomVertices = new List<Vector2>(); // the lowest vertex for each step
            var topVertices = new List<Vector2>(); // the highest vertex for each step
            var currentStep = 0f;

            for (int i = 0; i < vertices.Length; i++)
            {
                // Bottom part
                if (vertices[i].Y >= 0)
                {
                    if (vertices[i].X > currentStep)
                    {
                        bottomVertices.Add(vertices[i - 1]);
                        bottomVertices.Add(vertices[i]);
                    }
                }
                // Top part
                else
                {
                    if (vertices[i].X < currentStep)
                    {
                        topVertices.Add(vertices[i - 1]);
                        topVertices.Add(vertices[i]);
                    }
                }

                currentStep = vertices[i].X;
            }

            if (bottomVertices.Count != topVertices.Count)
            {
                // Left part
                if (bottomVertices.First().Y.Equals(0f))
                {
                    topVertices.Add(vertices[vertices.Length - 1]);
                    topVertices.Add(bottomVertices.First());
                    topVertices.Reverse();
                }
                // Right part
                else if (bottomVertices.Last().Y.Equals(0f))
                {
                    topVertices.Reverse();
                    topVertices.Add(bottomVertices.Last());
                }
            }
            else
                topVertices.Reverse();

            if (bottomVertices.Count == topVertices.Count)
            {
                for (int i = 1; i < bottomVertices.Count; i += 2)
                {
                    var boxVertices = new List<Vector2>
                    {
                        bottomVertices[i],
                        bottomVertices[i - 1],
                        topVertices[i - 1],
                        topVertices[i]
                    };

                    var collisionBox = new CollisionConvexPolygon(this, Vector2.Zero, boxVertices);
                    CollisionBoxes.Add(collisionBox);
                    _collisionBoxesHp.Add(collisionBox, 100f);

                }
            }
        }

        /*
        private void ComputeCollisionBoxesHp()
        {
            _leftCollisionBoxes.Sort((box1, box2) => box1.GetMinX().CompareTo(box2.GetMinX()));
            _rightCollisionBoxes.Sort((box1, box2) => box1.GetMaxX().CompareTo(box2.GetMaxX()));

            const int initialHp = 20;
            const int hpStep = 5;
            var hp = initialHp;
            foreach (var collisionBox in _leftCollisionBoxes)
            {
                collisionBox.HealthPoint = hp;
                hp += hpStep;
            }

            hp = initialHp;
            for (int i = 0; i < _rightCollisionBoxes.Count; i++)
            {
                var collisionBox = _rightCollisionBoxes[_rightCollisionBoxes.Count - 1 - i];
                collisionBox.HealthPoint = hp;
                hp += hpStep;
            }
        }
        */

        public void DisplayHpSwitch()
        {
            _displayHp = !_displayHp;
        }

        private void ParticleExplosion(Vector2 position)
        {
            float hue1 = GameRef.Rand.NextFloat(0, 6);
            float hue2 = (hue1 + GameRef.Rand.NextFloat(0, 2)) % 6f;
            Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
            Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);

            for (int j = 0; j < 120; j++)
            {
                float speed = 18f * (1f - 1 / GameRef.Rand.NextFloat(1f, 10f));
                var state = new ParticleState()
                {
                    Velocity = GameRef.Rand.NextVector2(speed, speed),
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1f
                };

                Color color = Color.Lerp(color1, color2, GameRef.Rand.NextFloat(0, 1));

                GameRef.ParticleManager.CreateParticle(GameRef.LineParticle, position,
                    color, 190, 1.5f, state);
            }
        }
    }
}
