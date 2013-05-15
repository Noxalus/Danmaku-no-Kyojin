using System;
using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.Collisions
{
    public abstract class CollisionElement
    {
        protected Entity Parent { get; set; }

        public Vector2 RelativePosition { get; set; }

        public abstract bool Intersects(CollisionElement collisionElement);

        public abstract void Draw(SpriteBatch sp);

        protected CollisionElement(Entity parent, Vector2 relativePosition)
        {
            Parent = parent;
            RelativePosition = relativePosition;
        }
    }
}
