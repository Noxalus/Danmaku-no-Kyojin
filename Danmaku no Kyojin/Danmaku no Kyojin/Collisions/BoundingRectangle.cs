using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Collisions
{
    class BoundingRectangle : BoundingElement
    {
        #region Fields

        private DnK _gameRef;

        private Vector2 _position;
        private Rectangle _rectangle;

        private Texture2D _drawableRectangle;

        #endregion

        #region Accessors

        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }

        #endregion

        public BoundingRectangle(DnK gameRef, Vector2 position, Rectangle rectangle)
        {
            _gameRef = gameRef;

            _position = position;
            _rectangle = rectangle;
            _drawableRectangle = CreateDrawableRectangle();
        }

        public Vector2 GetPosition()
        {
            return _position;
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        public void Update()
        {
        }

        public bool Intersects(BoundingElement boundingElement)
        {
            if (boundingElement is BoundingRectangle)
                return Intersects(boundingElement as BoundingRectangle);
            else if (boundingElement is BoundingCircle)
                return Intersects(boundingElement as BoundingCircle);
            else
                return false;
        }

        private bool Intersects(BoundingRectangle boundingSquare)
        {
            return false;
        }

        private bool Intersects(BoundingCircle boundingCircle)
        {
            return false;
        }

        public void Draw()
        {
            _gameRef.SpriteBatch.Draw(_drawableRectangle, _position, Color.White);
        }

        public void DrawDebug(Vector2 position, float rotation, Vector2 entitySize)
        {
            _gameRef.SpriteBatch.Draw(_drawableRectangle, position, Color.Red);
        }

        private Texture2D CreateDrawableRectangle()
        {
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
        }
    }
}
