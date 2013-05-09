using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Danmaku_no_Kyojin.Entities
{
    class Bullet : BaseBullet
    {
        private float _rotation;
        private Vector2 _center;

        public bool WaveMode { get; set; }
        private float _distance;

        public Bullet(DnK game, Texture2D sprite, Vector2 position, Vector2 direction, float velocity)
            : base(game, sprite, position, direction, velocity)
        {
            _rotation = 0;
            _center = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
            _distance = 0;

            WaveMode = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (WaveMode)
            {
                _distance += 0.75f;
                Direction.X = (float)Math.Cos(_distance);
            }

            _rotation = (_rotation + 0.25f) % 360;

            Position += Direction * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Game.SpriteBatch.Draw(Sprite, Position, null, Color.White, _rotation, _center, 1f, SpriteEffects.None, 0f);
        }
    }
}
