using System;
using System.Collections.Generic;
using Danmaku_no_Kyojin.BulletEngine;
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
        private Texture2D _eyeOrbit;
        private float _hp;
        private bool _activated;
        private PositionDelegate _playerPositionDelegate;
        private MoverManager _moverManager;
        private List<BulletPattern> _bulletPatterns;

        public float Hp
        {
            get{ return _hp; }
        }

        public BossCore(
            DnK gameRef, 
            Entity parent, 
            PositionDelegate playerPositionDelegate, 
            MoverManager moverManager,
            List<BulletPattern> bulletPatterns,
            float initialHp = 100f)
            : base(gameRef)
        {
            _parent = parent;
            _hp = initialHp;
            _activated = false;
            _playerPositionDelegate = playerPositionDelegate;
            _moverManager = moverManager;
            _bulletPatterns = bulletPatterns;
        }

        protected override void LoadContent()
        {
            Sprite = GameRef.Content.Load<Texture2D>("Graphics/Entities/boss_core");
            _eyeOrbit = GameRef.Content.Load<Texture2D>("Graphics/Entities/eye_orbit");

            // TODO: Upadte AnimatedSprite class to take a "rectangle" order
            var animation = new Animation(6, 42, 38, 0, 0, 2);
            Origin = new Vector2(21, 19);
            Scale = new Vector2(1.5f, 1.5f);
            _animatedSprite = new AnimatedSprite(Sprite, animation, Position) {IsAnimating = false};
            CollisionBoxes.Add(new CollisionCircle(_parent, Vector2.Zero, 25));
            base.LoadContent();
        }

        public void Activate()
        {
            _animatedSprite.Play();
            _activated = true;
            FirePattern();
        }

        public override void Update(GameTime gameTime)
        {
            _animatedSprite.Update(gameTime);
            UpdatePosition();

            if (_activated)
            {
                // Update eye orbit rotation according to Player's position
                var playerPosition = _playerPositionDelegate.Invoke();

                Rotation = (float)Math.Atan2(_parent.Position.Y - playerPosition.Y, _parent.Position.X - playerPosition.X) - MathHelper.PiOver2;

                // Bullet pattern
                if (_moverManager.movers.Count == 0)
                    FirePattern();
            }

            base.Update(gameTime);
        }

        private void UpdatePosition()
        {
            Position = _parent.Position;
            _animatedSprite.Position = Position;
        }

        private void FirePattern()
        {
            if (_bulletPatterns.Count > 0)
            {
                // Add a new bullet in the center of the screen
                var mover = (Mover)_moverManager.CreateBullet();
                mover.X = Position.X;
                mover.Y = Position.Y;
                var randomIndex = GameRef.Rand.Next(_bulletPatterns.Count - 1);
                mover.SetBullet(_bulletPatterns[randomIndex].RootNode);
            }
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
            if (_activated)
                GameRef.SpriteBatch.Draw(_eyeOrbit, Position, new Rectangle(0, 0, _eyeOrbit.Width, _eyeOrbit.Height), Color.White, Rotation, new Vector2(_eyeOrbit.Width / 2f, _eyeOrbit.Height / 2f), Scale, SpriteEffects.None, 1f);

            _animatedSprite.Draw(gameTime, GameRef.SpriteBatch, Color.White, 0f, Origin, Scale, SpriteEffects.None, 0f);

            base.Draw(gameTime);
        }
    }
}
