using System;
using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework.Graphics;
using Danmaku_no_Kyojin.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Danmaku_no_Kyojin.Collisions
{
    public class CollisionCircle : CollisionElement
    {
        #region Fields

        public float Radius { get; set; }

        private List<Vector2> _axes;

        #endregion

        public CollisionCircle(Entity parent, Vector2 relativePosition, float radius) : base(parent, relativePosition)
        {
            Radius = radius;

            _axes = new List<Vector2>();
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
            return element.Intersects(this);
        }

        private bool Intersects(CollisionCircle element)
        {
            float dx = element.GetCenter().X - GetCenter().X;
            float dy = element.GetCenter().Y - GetCenter().Y;
            float radii = Radius + element.Radius;

            if ((dx * dx) + (dy * dy) < radii * radii)
            {
                return true;
            }

            return false;
        }

        public override void Draw(SpriteBatch sp)
        {
            sp.DrawCircle(GetCenter().X, GetCenter().Y, Radius, 10, Color.White);

            for (int i = 0; i < _axes.Count; i++)
            {
                float a = _axes[i].Y / _axes[i].X;
                var x = new Vector2(GetCenter().X - (float)(Radius * Math.Cos(a)), GetCenter().Y - (float)(Radius * Math.Sin(a)));
                var y = new Vector2(GetCenter().X + (float)(Radius * Math.Cos(a)), GetCenter().Y + (float)(Radius * Math.Sin(a)));

                sp.DrawLine(x, y, Color.Red);                
            }
        }

        public override Vector2 GetCenter()
        {
            return new Vector2(
                Parent.X + RelativePosition.X * (float)(Math.Sin(Parent.GetRotation()) * -1),
                Parent.Y + RelativePosition.Y * (float)(Math.Cos(Parent.GetRotation())));
        }

        public Vector2 Project(Vector2 axis)
        {
            if (!_axes.Contains(axis))
                _axes.Add(axis);

            float a = axis.Y / axis.X;

            float min = Vector2.Dot(new Vector2(GetCenter().X - (float)(Radius * Math.Sin(a)), GetCenter().Y - (float)(Radius * Math.Cos(a))), axis);
            float max = Vector2.Dot(new Vector2(GetCenter().X + (float)(Radius * Math.Sin(a)), GetCenter().Y + (float)(Radius * Math.Cos(a))), axis);

            return new Vector2(min, max);
        }
    }
}