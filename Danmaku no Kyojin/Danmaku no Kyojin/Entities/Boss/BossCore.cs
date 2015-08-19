using System;
using Danmaku_no_Kyojin.Collisions;
using Danmaku_no_Kyojin.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.Entities.Boss
{
    class BossCore : SpriteEntity
    {
        private readonly Entity _parent;
        private AnimatedSprite _animatedSprite;
        private float _hp;

        public BossCore(DnK gameRef, Entity parent, float initialHp = 100f)
            : base(gameRef)
        {
            _parent = parent;
            _hp = initialHp;
        }

        protected override void LoadContent()
        {
            Sprite = GameRef.Content.Load<Texture2D>("Graphics/Entities/boss_core");

            // TODO: Upadte AnimatedSprite class to take a "rectangle" order
            var animation = new Animation(6, 42, 38, 0, 0, 5);
            Origin = new Vector2(21, 19);
            Scale = new Vector2(1f, 1f);
            _animatedSprite = new AnimatedSprite(Sprite, animation, Position) {IsAnimating = false};
            CollisionBoxes.Add(new CollisionCircle(_parent, Vector2.Zero, 20));
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _animatedSprite.Update(gameTime);
            UpdatePosition();

            base.Update(gameTime);
        }

        private void UpdatePosition()
        {
            var position = _parent.Position;

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

            _animatedSprite.Position = _parent.Position;
        }

        public override bool Intersects(Entity entity)
        {
            if (!entity.Intersects(this))
                return false;

            var bullet = entity as BaseBullet;
            if (bullet != null)
            {
                _hp -= bullet.Power;

                if (_hp < 0)
                    IsAlive = false;
            }

            return true;
        }

        public override void Draw(GameTime gameTime)
        {
            _animatedSprite.Draw(gameTime, GameRef.SpriteBatch, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0f);

            base.Draw(gameTime);
        }
    }
}
