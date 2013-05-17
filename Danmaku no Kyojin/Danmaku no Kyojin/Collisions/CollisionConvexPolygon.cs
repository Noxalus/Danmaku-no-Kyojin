using System;
using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Danmaku_no_Kyojin.Utils;

namespace Danmaku_no_Kyojin.Collisions
{
    class CollisionConvexPolygon : CollisionElement
    {
        #region Fields

        private List<Point> Vertices { get; set; }
        public bool IsFilled { get; set; }

        #endregion

        public CollisionConvexPolygon(Entity parent, Vector2 relativePosition, List<Point> vertices)
            : base(parent, relativePosition)
        {
            Parent = parent;
            Vertices = vertices;
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
            Vector2 previousPosition = GetPosition(Vertices[0]);
            Vector2 position;

            for (int i = 1; i <= Vertices.Count; i++)
            {
                if (i == Vertices.Count)
                    position = GetPosition(Vertices[0]);
                else
                    position = GetPosition(Vertices[i]);

                sp.DrawLine(
                    previousPosition.X,
                    previousPosition.Y,
                    position.X,
                    position.Y, Color.White);

                if (i >= 0)
                {
                    float a = (position.Y - previousPosition.Y) / (position.X - previousPosition.X);
                    Vector2 Q = Vector2.Normalize(position - previousPosition);
                    
                    /*
                    sp.DrawLine(
                        previousPosition.X - 2000 * Q.X, previousPosition.Y - Q.Y * 2000,
                        previousPosition.X + 2000 * Q.X, previousPosition.Y + Q.Y * 2000,
                        Color.Red);
                    */
                    sp.DrawLine(
                        previousPosition.X, previousPosition.Y,
                        (previousPosition.X) + Q.Y * 2000, (previousPosition.Y) - Q.X * 2000,
                        Color.Red);
                }

                previousPosition = position;
            }
        }

        private Vector2 GetPosition(Point vertex)
        {
            return new Vector2(
                Parent.GetOrigin().X + vertex.X + (float)(Math.Sin(Parent.GetRotation()) * -1),
                Parent.GetOrigin().Y + vertex.Y + (float)(Math.Cos(Parent.GetRotation()))
            );
        }
    }
}
