using System;
using Danmaku_no_Kyojin.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.Entities.Boss
{
    class BossCore : SpriteEntity
    {
        private Entity _parent;
        private AnimatedSprite _animatedSprite;

        public BossCore(DnK gameRef, Entity parent)
            : base(gameRef)
        {
            _parent = parent;
        }

        protected override void LoadContent()
        {
            Sprite = GameRef.Content.Load<Texture2D>("Graphics/Entities/boss_core");

            // TODO: Upadte AnimatedSprite class to take a "rectangle" order
            var animation = new Animation(7, 50, 50, 0, 0, 5);
            _animatedSprite = new AnimatedSprite(Sprite, animation, Position) {IsAnimating = true};

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
            var position = _parent.Origin;

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

            _animatedSprite.Position = Position;
        }

        public override void Draw(GameTime gameTime)
        {
            //GameRef.SpriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0f);
            _animatedSprite.Draw(gameTime, GameRef.SpriteBatch, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0f);

            base.Draw(gameTime);
        }
    }
}
