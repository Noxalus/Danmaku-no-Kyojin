using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    public abstract class BaseBullet : SpriteEntity
    {
        public Vector2 Direction;
        protected Vector2 Velocity;
        public float Power { get; set; }

        protected BaseBullet(DnK gameRef, Texture2D sprite, Vector2 position, Vector2 direction, Vector2 velocity)
            : base(gameRef)
        {
            Sprite = sprite;
            Position = position;
            Direction = direction;
            Velocity = velocity;
        }
    }
}
