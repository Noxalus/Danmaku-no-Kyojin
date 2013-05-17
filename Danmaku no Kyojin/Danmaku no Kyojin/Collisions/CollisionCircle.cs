using System;
using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework.Graphics;
using Danmaku_no_Kyojin.Utils;
using Microsoft.Xna.Framework;

namespace Danmaku_no_Kyojin.Collisions
{
    public class CollisionCircle : CollisionElement
    {
        #region Fields

        public float Radius { get; set; }

        #endregion

        public CollisionCircle(Entity parent, Vector2 relativePosition, float radius) : base(parent, relativePosition)
        {
            Radius = radius;
        }

        public override bool Intersects(CollisionElement collisionElement)
        {
            if (collisionElement is CollisionConvexPolygon)
                return Intersects(collisionElement as CollisionConvexPolygon);

            if (collisionElement is CollisionCircle)
                return Intersects(collisionElement as CollisionCircle);

            return collisionElement.Intersects(this);
        }

        private bool Intersects(CollisionConvexPolygon element)
        {
            return false;
        }

        private bool Intersects(CollisionCircle element)
        {
            return false;
        }

        public override void Draw(SpriteBatch sp)
        {
            sp.DrawCircle(GetCenter().X, GetCenter().Y, Radius, 10, Color.White);
        }

        private Vector2 GetCenter()
        {
            return new Vector2(
                Parent.X + RelativePosition.X * (float)(Math.Sin(Parent.GetRotation()) * -1),
                Parent.Y + RelativePosition.Y * (float)(Math.Cos(Parent.GetRotation())));
        }
    }
}