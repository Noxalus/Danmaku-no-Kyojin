using Danmaku_no_Kyojin.Entities;

namespace Danmaku_no_Kyojin.Collisions
{
    public abstract class CollisionElement
    {
        protected Entity Parent { get; set; }

        public abstract bool Intersects(CollisionElement collisionElement);

        protected CollisionElement(Entity parent)
        {
            Parent = parent;
        }
    }
}
