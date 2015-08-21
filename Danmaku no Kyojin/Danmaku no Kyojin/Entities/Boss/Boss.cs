using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Controls;
using Danmaku_no_Kyojin.Particles;
using Danmaku_no_Kyojin.Shapes;
using Danmaku_no_Kyojin.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Danmaku_no_Kyojin.Entities.Boss
{
    class Boss
    {
        private DnK _gameRef;

        private TimeSpan _timer;
        private int _defeatCounter;

        // Bullet engine
        public MoverManager MoverManager { get; set; }
        // A list of all the bulletml samples we have loaded
        private List<BulletPattern> _completeBulletPatterns = new List<BulletPattern>();
        // The names of all the bulletml patterns that are loaded, stored so we can display what is being fired
        private List<string> _patternNames = new List<string>();

        private bool _ready;

        // Players
        private List<Player> _players;

        // Structure
        private BossPart _mainPart;
        private bool _mainPartIsDead;
        private readonly List<BossPart> _parts;
        private BossCore _core;
        private int _iteration;
        private float _step;

        // Previous structure
        private BossPart _previousBossPart;

        // To debug
        private int _currentPartIndex;

        public List<BossPart> Parts
        {
            get { return _parts; }
        }

        public BossCore BossCore
        {
            get { return _core; }
        }

        public Boss(DnK gameRef, List<Player> players, int iteration = 50, float step = 25)
        {
            _gameRef = gameRef;
            _defeatCounter = 0;
            MoverManager = new MoverManager(gameRef);
            GameManager.GameDifficulty = Config.GameDifficultyDelegate;
            _players = players;
            _iteration = iteration;
            _step = step;
            _previousBossPart = null;

            _parts = new List<BossPart>();
            _currentPartIndex = 0;

            int targetPlayerId = _gameRef.Rand.Next(0, _players.Count);
            MoverManager.Initialize(_players[targetPlayerId].GetPosition);
        }

        public void Initialize()
        {
            LoadContent();
        }

        private void LoadContent()
        {
            //Get all the xml files
            foreach (var source in Directory.GetFiles(@"Content\Data\Patterns", "*.xml", SearchOption.AllDirectories))
            {
                //store the name
                _patternNames.Add(source);

                //load the pattern
                var pattern = new BulletPattern();
                pattern.ParseXML(source);
                _completeBulletPatterns.Add(pattern);
            }

            Reset();
        }

        private void Reset()
        {
            MoverManager.movers.Clear();

            _mainPartIsDead = false;
            _ready = true;
            _timer = Config.BossInitialTimer;

            if (_previousBossPart == null)
            {
                _mainPart = new BossPart(
                    _gameRef, this, MoverManager, _completeBulletPatterns, new Color(0f, 0.75f, 0f, 0.65f),
                    4242f, _iteration, _step, null, null, true
                );

                _mainPart.Initialize();
            }
            else
            {
                _mainPart = _previousBossPart;
                _mainPart.IterateStructure(_iteration);
            }

            _previousBossPart = (BossPart)_mainPart.Clone();

            _parts.Add(_mainPart);

            int targetPlayerId = _gameRef.Rand.Next(0, _players.Count);
            _core = new BossCore(_gameRef, _mainPart, _players[targetPlayerId].GetPosition, MoverManager, _completeBulletPatterns);
            _core.Initialize();
        }

        private void ParticleExplosion(Vector2 position)
        {
            float hue1 = _gameRef.Rand.NextFloat(0, 6);
            float hue2 = (hue1 + _gameRef.Rand.NextFloat(0, 2)) % 6f;
            Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
            Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);

            for (int j = 0; j < 50; j++)
            {
                float speed = 18f * (1f - 1 / _gameRef.Rand.NextFloat(1f, 10f));
                var state = new ParticleState()
                {
                    Velocity = _gameRef.Rand.NextVector2(speed, speed),
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 1f
                };

                Color color = Color.Lerp(color1, color2, _gameRef.Rand.NextFloat(0, 1));

                _gameRef.ParticleManager.CreateParticle(_gameRef.LineParticle, position,
                    color, 190, 1.5f, state);
            }
        }

        public void Update(GameTime gameTime)
        {
            _timer -= gameTime.ElapsedGameTime;
            if (_timer < TimeSpan.Zero)
            {
                _timer = Config.BossInitialTimer;
            }

            // If the main part is destroyed, we destroy all parts
            // (or if the boss core is destroyed instead)
            if (!_core.IsAlive)
            {
                // Destroy all parts
                foreach (var part in Parts)
                {
                    part.TakeDamage(9999);
                }

                _defeatCounter++;
                Reset();
            }

            _core.Update(gameTime);

            if (!_mainPartIsDead && !_mainPart.IsAlive)
            {
                _mainPartIsDead = true;
                _core.Activate();
            }

            for (int i = 0; i < _parts.Count; i++)
            {
                _parts[i].Update(gameTime);

                if (!_parts[i].IsAlive)
                    _parts.Remove(_parts[i]);
            }

            // Check bullets to players collision
            for (int i = 0; i < MoverManager.movers.Count; i++)
            {
                foreach (var player in _players)
                {
                    if (MoverManager.movers[i].Intersects(player))
                    {
                        // Destroyed by shield?
                        if (player.IsInvincible)
                        {
                            ParticleExplosion(MoverManager.movers[i].Position);
                            MoverManager.RemoveBullet(MoverManager.movers[i]);
                        }
                        else
                            player.Hit();
                    }
                }
            }

            if (Config.Debug)
            {
                /*
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
                else */

                if (_parts.Count > 0)
                {
                    var currentBossPart = _parts[_currentPartIndex];
                    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds * 100;

                    if (InputHandler.KeyDown(Keys.Space))
                    {
                        currentBossPart.GenerateStructure();
                    }
                    else if (InputHandler.KeyPressed(Keys.OemPlus))
                    {
                        currentBossPart.IterateStructure();
                    }
                    else if (InputHandler.KeyDown(Keys.O))
                    {
                        currentBossPart.IterateStructure();
                    }

                    // Motion

                    if (InputHandler.KeyPressed(Keys.N))
                        _currentPartIndex = (_currentPartIndex + 1) % _parts.Count;

                    var acceleration = 0.1f;
                    if (InputHandler.KeyDown(Keys.I))
                        currentBossPart.ApplyImpulse(new Vector2(0, -1), new Vector2(acceleration));
                    if (InputHandler.KeyDown(Keys.L))
                        currentBossPart.ApplyImpulse(new Vector2(1, 0), new Vector2(acceleration));
                    if (InputHandler.KeyDown(Keys.K))
                        currentBossPart.ApplyImpulse(new Vector2(0, 1), new Vector2(acceleration));
                    if (InputHandler.KeyDown(Keys.J))
                        currentBossPart.ApplyImpulse(new Vector2(-1, 0), new Vector2(acceleration));

                    // Left vector
                    if (InputHandler.KeyPressed(Keys.F))
                    {
                        var direction = new Vector2(
                            (float)Math.Cos(currentBossPart.Rotation) * -1,
                            (float)-Math.Sin(currentBossPart.Rotation)
                        );
                       
                        //var direction = new Vector2(
                        //    (float)Math.Cos(currentBossPart.Rotation + (Math.PI / 2f) * -1),
                        //    (float)Math.Sin(currentBossPart.Rotation + (Math.PI / 2f) * 1)
                        //);

                        currentBossPart.ApplyImpulse(direction, new Vector2(acceleration));
                    }

                    // Right vector
                    if (InputHandler.KeyPressed(Keys.G))
                    {
                        var direction = new Vector2(
                            (float)-Math.Cos(currentBossPart.Rotation) * -1,
                            (float)Math.Sin(currentBossPart.Rotation)
                        );

                        direction = new Vector2(-direction.Y, direction.X);

                        currentBossPart.ApplyImpulse(direction, new Vector2(acceleration));
                    }

                    if (InputHandler.KeyDown(Keys.PageUp))
                    {
                        currentBossPart.Rotation += dt * 0.01f;
                    }
                    else if (InputHandler.KeyDown(Keys.PageDown))
                    {
                        currentBossPart.Rotation -= dt * 0.01f;
                    }

                    if (InputHandler.KeyPressed(Keys.H))
                    {
                        currentBossPart.DisplayHpSwitch();
                    }
                }
            }

            MoverManager.Update(gameTime);
            MoverManager.FreeMovers();
        }

        public bool Intersects(Entity entity)
        {
            return _parts.Any(part => part.Intersects(entity)) | _core.Intersects(entity);
        }

        public void Draw(GameTime gameTime, Matrix viewMatrix)
        {
            foreach (var part in _parts)
                part.Draw(gameTime, viewMatrix);

            _core.Draw(gameTime);

            MoverManager.Draw(gameTime);
        }

        public bool IsReady()
        {
            return _ready;
        }
    }
}
