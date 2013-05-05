using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Collisions
{
    class CollisionPolygon : ColisionElement
    {
        #region Fields

        private DnK _gameRef;

        public List<Point> Vertices { get; set; }
        public bool IsFilled { get; set; }
        public sealed override Entity Parent { get; set; }

        private Texture2D _drawablePolygon;

        #endregion

        public CollisionPolygon(DnK gameRef, Entity parent, Point[] vertices)
        {
            _gameRef = gameRef;

            Parent = parent;
            Vertices = vertices.ToList();
            //_drawablePolygon = CreateDrawablePolygon();
        }

        public override bool Intersects(ColisionElement collisionElement)
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

        public override void Draw()
        {
            //_gameRef.SpriteBatch.Draw(_drawablePolygon, Parent.Position, Color.White);
        }

        public override void DrawDebug(Vector2 position, float rotation, Vector2 entitySize)
        {
            //_gameRef.SpriteBatch.Draw(_drawablePolygon, position, Color.Red);
        }

        private Texture2D CreateDrawablePolygon()
        {
            /*
            Texture2D box = new Texture2D(_gameRef.Graphics.GraphicsDevice, _rectangle.Width, _rectangle.Height);

            Color[] data = new Color[_rectangle.Width * _rectangle.Height];

            for (int x = 0; x < _rectangle.Width; x++)
            {
                for (int y = 0; y < _rectangle.Height; y++)
                {
                    if ((y == 0 || y == (_rectangle.Height - 1)) || (x == 0 || x == _rectangle.Width - 1))
                    data[x + (y * _rectangle.Width)] = Color.Red;
                }
            }

            box.SetData(data);

            return box;
            */
            throw new NotImplementedException();
        }
    }
}
