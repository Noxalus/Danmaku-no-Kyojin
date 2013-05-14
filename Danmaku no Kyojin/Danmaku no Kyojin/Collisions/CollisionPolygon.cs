using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Collisions
{
    class CollisionPolygon : CollisionElement
    {
        #region Fields

        public List<Point> Vertices { get; set; }
        public bool IsFilled { get; set; }

        #endregion

        public CollisionPolygon(Entity parent, Point[] vertices) : base(parent)
        {
            Parent = parent;
            Vertices = vertices.ToList();
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

        public override void Draw(SpriteBatch sp, Vector2 position)
        {
            throw new NotImplementedException();
        }
    }
}
