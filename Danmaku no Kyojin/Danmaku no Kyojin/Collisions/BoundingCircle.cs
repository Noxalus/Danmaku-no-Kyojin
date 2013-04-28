using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Collisions
{
    class BoundingCircle : BoundingElement
    {
        #region Fields

        private DnK _gameRef;
        private Vector2 _position;
        private float _radius;

        Texture2D _drawableCircle;

        #endregion

        public BoundingCircle(DnK gameRef, Vector2 position, float radius)
        {
            _gameRef = gameRef;

            _position = position;
            _radius = radius;
            _drawableCircle = CreateDrawableCircle();
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

        public bool Intersects(BoundingRectangle boundingSquare)
        {
            return false;
        }

        public bool Intersects(BoundingCircle boundingCircle)
        {
            return false;
        }

        public void Draw()
        {
            _gameRef.SpriteBatch.Draw(_drawableCircle, _position, Color.Red);
        }

        private Texture2D CreateDrawableCircle()
        {
            int outerRadius = (int)(_radius * 2 + 2); // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(_gameRef.Graphics.GraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / _radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(_radius + _radius * Math.Cos(angle));
                int y = (int)Math.Round(_radius + _radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);

            return texture;
        }
    }
}
