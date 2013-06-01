using System.Collections.Generic;
using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Danmaku_no_Kyojin.Entities
{
    class Bullet : BaseBullet
    {
        public bool WaveMode { get; set; }
        private float _distance;

        public Bullet(DnK game, Texture2D sprite, Vector2 position, Vector2 direction, float velocity)
            : base(game, sprite, position, direction, velocity)
        {
            Rotation = 0;
            Center = new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);
            _distance = 0;

            WaveMode = false;

            List<Point> vertices = new List<Point>()
                {
                    new Point(0, 0),
                    new Point(sprite.Width, 0),
                    new Point(sprite.Width, sprite.Height),
                    new Point(0, sprite.Height),
                };

            CollisionBox = new CollisionConvexPolygon(this, Vector2.Zero, vertices);

            Power = Improvements.ShootPowerData[PlayerData.ShootPowerIndex].Key;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (WaveMode)
            {
                _distance += 0.75f;
                Direction.X = (float)Math.Cos(_distance);
            }

            Rotation = (Rotation + 0.25f) % 360;

            Position += Direction * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Game.SpriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Center, 1f, SpriteEffects.None, 0f);
        }
    }
}
