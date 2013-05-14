using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.Collisions
{
    public abstract class CollisionElement
    {
        protected Entity Parent { get; set; }

        public abstract bool Intersects(CollisionElement collisionElement);

        public abstract void Draw(SpriteBatch sp, Vector2 position);

        protected CollisionElement(Entity parent)
        {
            Parent = parent;
        }
    }
}
