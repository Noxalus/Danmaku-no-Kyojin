using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    public abstract class BaseBullet : Entity
    {
        protected Vector2 Direction;
        protected float Velocity;
        public Texture2D Sprite { get; set; }
        public float Power { get; set; }

        protected BaseBullet(DnK game, Texture2D sprite, Vector2 position, Vector2 direction, float velocity)
            : base(game)
        {
            Sprite = sprite;
            Position = position;
            Direction = direction;
            Velocity = velocity;
        }
    }
}
