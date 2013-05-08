using Danmaku_no_Kyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Collisions
{
    public class CollisionCircle : ColisionElement
    {
        #region Fields

        private DnK _gameRef;

        public float Radius { get; set; }
        public sealed override Entity Parent { get; set; }

        Texture2D _drawableCircle;

        Texture2D _pixel;

        #endregion

        public CollisionCircle(DnK gameRef, Entity parent, float radius)
        {
            _gameRef = gameRef;

            Radius = radius;
            Parent = parent;

            _drawableCircle = CreateDrawableCircle();

            // Load
            _pixel = _gameRef.Content.Load<Texture2D>("Graphics/Pictures/pixel");
        }

        public override bool Intersects(ColisionElement collisionElement)
        {
            if (collisionElement is CollisionPolygon)
                return Intersects(collisionElement as CollisionPolygon);

            if (collisionElement is CollisionCircle)
                return Intersects(collisionElement as CollisionCircle);

            return collisionElement.Intersects(this);
        }

        private bool Intersects(CollisionPolygon collisionPolygon)
        {
            return false;
        }

        private bool Intersects(CollisionCircle boundingCircle)
        {
            return false;
        }

        public override void Draw()
        {
            _gameRef.SpriteBatch.Draw(_drawableCircle, Parent.GetPosition(), Color.Red);
        }

        public override void DrawDebug(Vector2 position, float rotation, Vector2 entitySize)
        {
            Vector2 formerPosition = position;
            position.X = position.X - Radius/2;
            position.Y = position.Y - Radius/2;

            Vector2 center = new Vector2(entitySize.X / 2, entitySize.Y / 2);

            _gameRef.SpriteBatch.Draw(_pixel, new Rectangle((int)position.X, (int)position.Y, (int)Radius, (int)Radius), null,
                Color.Red, rotation, center, SpriteEffects.None, 0f);
            //_gameRef.SpriteBatch.Draw(_pixel, new Rectangle((int)position.X, (int)position.Y, (int)_radius, (int)_radius), Color.Red);
        }

        private Texture2D CreateDrawableCircle()
        {
            int outerRadius = (int)(Radius * 2 + 2); // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(_gameRef.Graphics.GraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / Radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(Radius + Radius * Math.Cos(angle));
                int y = (int)Math.Round(Radius + Radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);

            return texture;
        }
    }
}
