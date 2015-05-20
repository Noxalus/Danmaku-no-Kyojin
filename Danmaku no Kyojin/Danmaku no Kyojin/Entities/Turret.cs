using System;
using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.Entities
{
    class Turret : SpriteEntity
    {
        private Vector2 _initialPosition;
        private Boss _parent;
        private BulletPattern _pattern;
        private TimeSpan _timer;
        private Color _color;

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public Vector2 InitialPosition
        {
            get { return _initialPosition; }
        }

        public Turret(DnK game, Boss parent, Vector2 initialPosition, BulletPattern partern) : base(game)
        {
            _initialPosition = initialPosition;
            X = InitialPosition.X;
            Y = InitialPosition.Y;
            _parent = parent;
            _pattern = partern;
            _timer = new TimeSpan(0, 0, (int)(Game.Rand.NextDouble() * 30));
            _color = Color.White;
        }

        public void LoadContent()
        {
            Sprite = Game.Content.Load<Texture2D>(@"Graphics/Sprites/ball");

            CollisionBoxes.Add(new CollisionCircle(this, Vector2.Zero, Sprite.Width / 2f));
        }

        public override void Update(GameTime gameTime)
        {
            // Update position according to parent
            UpdatePosition();

            _timer -= gameTime.ElapsedGameTime;

            if (_timer.Milliseconds <= 0)
            {
                var mover = (Mover)_parent.MoverManager.CreateBullet();
                mover.X = Position.X;
                mover.Y = Position.Y;
                mover.SetBullet(_pattern.RootNode);

                _timer = new TimeSpan(0, 0, (int)(Game.Rand.NextDouble() * 30));
            }
        }

        private void UpdatePosition()
        {
            var position = new Vector2(InitialPosition.X, InitialPosition.Y);

            var angleCos = (float)Math.Cos(_parent.Rotation);
            var angleSin = (float)Math.Sin(_parent.Rotation);

            // Translate point back to origin  
            position.X -= _parent.Origin.X;
            position.Y -= _parent.Origin.Y;

            // Rotate point
            var newX = position.X * angleCos - position.Y * angleSin;
            var newY = position.X * angleSin + position.Y * angleCos;

            // Translate point back
            position.X = newX + _parent.Position.X;
            position.Y = newY + _parent.Position.Y;

            Position = position;
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(Sprite, Position, null, _color, Rotation, Origin, 1f, SpriteEffects.None, 0f);

            base.Draw(gameTime);
        }
    }
}
