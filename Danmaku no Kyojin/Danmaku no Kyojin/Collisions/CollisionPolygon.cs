using System;
using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Danmaku_no_Kyojin.Utils;

namespace Danmaku_no_Kyojin.Collisions
{
    class CollisionPolygon : CollisionElement
    {
        #region Fields

        private List<Point> Vertices { get; set; }
        public bool IsFilled { get; set; }

        #endregion

        public CollisionPolygon(Entity parent, Vector2 relativePosition, List<Point> vertices)
            : base(parent, relativePosition)
        {
            Parent = parent;
            Vertices = vertices;
        }

        public override bool Intersects(CollisionElement collisionElement)
        {
            if (collisionElement is CollisionPolygon)
                return Intersects(collisionElement as CollisionPolygon);

            if (collisionElement is CollisionCircle)
                return Intersects(collisionElement as CollisionCircle);

            return collisionElement.Intersects(this);
        }

        private bool Intersects(CollisionPolygon boundingSquare)
        {
            return false;
        }

        private bool Intersects(CollisionCircle boundingCircle)
        {
            return false;
        }

        public override void Draw(SpriteBatch sp)
        {
            Point previousVertex = Vertices[0];
            Vector2 previousPosition = GetPosition(previousVertex);
            Vector2 position;

            for (int i = 1; i < Vertices.Count; i++)
            {
                position = GetPosition(Vertices[i]);
                sp.DrawLine(
                    previousPosition.X,
                    previousPosition.Y,
                    position.X,
                    position.Y, Color.White);

                previousVertex = Vertices[i];
                previousPosition = position;
            }

            position = GetPosition(Vertices[0]);

            sp.DrawLine(
                previousPosition.X,
                previousPosition.Y,
                position.X,
                position.Y, Color.White);
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
